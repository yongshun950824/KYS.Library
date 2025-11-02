using System;
using System.Data;
using System.Linq;
using KYS.Library.Helpers;
using NUnit.Framework;

namespace KYS.TestProject.HelpersUnitTests;

public class SqlHelperUnitTest
{
    [TestCase(null, "test")]
    [TestCase("<connection string>", null)]
    public void GetDataSet_WithInvalidArguments_ShouldThrowException(string connectionString, string commandText)
    {
        // Arrange
        CommandType commandType = CommandType.Text;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SqlHelper.GetDataSet(connectionString, commandType, commandText));
    }


    [TestCase(null, "test")]
    [TestCase("<connection string>", null)]
    public void GetDataTable_WithInvalidArguments_ShouldThrowException(string connectionString, string commandText)
    {
        // Arrange
        CommandType commandType = CommandType.Text;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SqlHelper.GetDataTable(connectionString, commandType, commandText));
    }

    [Test]
    public void BuildStructuredSqlParameter_ShouldReturnValidSqlParameter()
    {
        // Arrange
        var values = new[] { 1, 2, 3 };

        // Act
        var param = SqlHelper.BuildStructuredSqlParameter("dbo.IntListType", "Value", "@Ids", values);

        // Assert
        Assert.AreEqual("@Ids", param.ParameterName);
        Assert.AreEqual(SqlDbType.Structured, param.SqlDbType);
        Assert.AreEqual("dbo.IntListType", param.TypeName);

        // Verify DataTable
        var dt = (DataTable)param.Value;
        Assert.AreEqual(1, dt.Columns.Count);
        Assert.AreEqual("Value", dt.Columns[0].ColumnName);
        CollectionAssert.AreEqual(values, dt.Rows.Cast<DataRow>().Select(r => r["Value"]));
    }

    [TestCase(null, "Col", "@p")]
    [TestCase("Type", null, "@p")]
    [TestCase("Type", "Col", null)]
    public void BuildStructuredSqlParameter_ShouldThrow_WhenArgumentsInvalid(string tableTypeName, string columnName, string paramName)
    {
        // Arrange
        var values = new[] { 1 };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            SqlHelper.BuildStructuredSqlParameter(tableTypeName, columnName, paramName, values));
    }

    [Test]
    public void BuildStructuredSqlParameter_ShouldHandleEmptyValues()
    {
        // Act
        var param = SqlHelper.BuildStructuredSqlParameter("dbo.IntListType", "Value", "@Ids", Enumerable.Empty<int>());

        // Assert
        var dt = (DataTable)param.Value;
        Assert.AreEqual(0, dt.Rows.Count);
        Assert.AreEqual("Value", dt.Columns[0].ColumnName);
    }

    [Test]
    public void BuildStructuredSqlParameter_ShouldSupportDifferentTypes()
    {
        var values = new[] { "Alice", "Bob" };

        var param = SqlHelper.BuildStructuredSqlParameter("dbo.StringListType", "Name", "@Names", values);

        var dt = (DataTable)param.Value;
        Assert.AreEqual(typeof(string), dt.Columns["Name"].DataType);
    }
}
