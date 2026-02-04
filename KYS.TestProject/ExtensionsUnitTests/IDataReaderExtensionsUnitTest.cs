using System;
using System.Data;
using System.Globalization;
using KYS.Library.Extensions;
using Moq;
using NUnit.Framework;

namespace KYS.TestProject.ExtensionsUnitTests;

public class IDataReaderExtensionsUnitTest
{
    [SetUp]
    public void Setup()
    {

    }

    private Mock<IDataReader> SetupMock(string columnName, object value)
    {
        var mockReader = new Mock<IDataReader>();
        mockReader.Setup(r => r.GetOrdinal(columnName)).Returns(0);
        mockReader.Setup(r => r.GetValue(0)).Returns(value);
        mockReader.Setup(r => r.IsDBNull(0)).Returns(value == null || value == DBNull.Value);

        return mockReader;
    }

    [Test]
    public void SafeGetValue_WithStringColumn_ShouldReturnValue()
    {
        // Arrange
        string columnName = "Name";
        string expectedValue = "Hello World";
        var reader = SetupMock(columnName, "Hello World").Object;

        // Act
        var result = reader.SafeGetValue<string>(columnName);

        // Assert
        Assert.AreEqual(expectedValue, result);
    }

    [Test]
    public void SafeGetValue_WithDbNullColumnAndNoProvideDefaultValue_ShouldReturnDefaultValue()
    {
        // Arrange
        string columnName = "Age";
        int expectedValue = 0;
        var reader = SetupMock(columnName, DBNull.Value).Object;

        // Act
        var result = reader.SafeGetValue<int>(columnName);

        // Assert
        Assert.AreEqual(expectedValue, result);
    }

    [Test]
    public void SafeGetValue_WithDbNullColumnAndProvideDefaultValue_ShouldReturnProvidedDefaultValue()
    {
        // Arrange
        string columnName = "Age";
        int defaultValue = 99;
        var reader = SetupMock(columnName, DBNull.Value).Object;

        // Act
        var result = reader.SafeGetValue(columnName, defaultValue);

        // Assert
        Assert.AreEqual(defaultValue, result);
    }

    [Test]
    public void SafeGetValue_WithDateTimeColumn_ShouldReturnValue()
    {
        // Arrange
        string columnName = "CreatedDate";
        DateTime expectedValue = DateTime.Now;
        var reader = SetupMock(columnName, expectedValue).Object;

        // Act
        var result = reader.SafeGetValue(columnName, expectedValue);

        // Assert
        Assert.AreEqual(expectedValue, result);
    }

    [Test]
    public void SafeGetValue_WithMinDateTimeColumnAndProvideDefaultValue_ShouldReturnProvidedDefaultValue()
    {
        // Arrange
        string columnName = "CreatedDate";
        DateTime outOfRangeDate = DateTime.MinValue;
        DateTime defaultValue = DateTime.Now;
        var reader = SetupMock(columnName, outOfRangeDate).Object;

        // Act
        var result = reader.SafeGetValue(columnName, defaultValue);

        // Assert
        Assert.AreEqual(defaultValue, result);
    }

    [Test]
    public void SafeGetValue_WithMaxDateTimeColumnAndProvideDefaultValue_ShouldReturnProvidedDefaultValue()
    {
        // Arrange
        string columnName = "CreatedDate";
        DateTime outOfRangeDate = DateTime.MaxValue;
        DateTime defaultValue = DateTime.Now;
        var reader = SetupMock(columnName, outOfRangeDate).Object;

        // Act
        var result = reader.SafeGetValue(columnName, defaultValue);

        // Assert
        Assert.AreEqual(defaultValue, result);
    }

    [Test]
    public void SafeGetValue_WithNullableDateTimeColumn_ShouldReturnNull()
    {
        // Arrange
        string columnName = "DeletedDate";
        DateTime? expectedValue = null;
        var reader = SetupMock(columnName, expectedValue).Object;

        // Act
        var result = reader.SafeGetValue<DateTime?>(columnName);

        // Assert
        Assert.AreEqual(expectedValue, result);
        Assert.IsNull(result);
    }

    [Test]
    public void SafeGetValue_WithTypesNotMatch_ShouldReturnCastedValue()
    {
        // Arrange
        string columnName = "Status";
        int expectedValue = 1;
        var reader = SetupMock(columnName, (byte)expectedValue).Object;

        // Act
        var result = reader.SafeGetValue<int>(columnName);

        // Assert
        Assert.AreEqual(expectedValue, result);
    }
}
