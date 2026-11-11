using System.Data;
using System.Drawing;
using KYS.Library.Extensions;
using KYS.Library.Helpers;
using NUnit.Framework;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ExcelDataTableHelper = KYS.Library.Excel.Helpers.ExcelDataTableHelper;
using DomainErrors = KYS.Library.Helpers.DomainErrors;
using KYS.Library.Excel.Options;

namespace KYS.Library.Excel.Tests.HelpersUnitTests;

public class ExcelDataTableHelperUnitTest
{
    private readonly DataTable _dt = new();
    private readonly List<Student> _studentList = new()
    {
        new Student { Id = 1, Name = "Ali" },
        new Student {  Id = 2, Name = "Bob" }
    };
    private const char SEPARATOR = ';';
    private string _csvFilePath;

    [SetUp]
    public void Setup()
    {
        _csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "DataTableHelper", "data.csv");

        if (_dt.IsNullOrEmpty())
        {
            _dt.Columns.Add(nameof(Student.Id), typeof(int));
            _dt.Columns.Add(nameof(Student.Name), typeof(string));

            var rowOne = _dt.NewRow();
            rowOne[nameof(Student.Id)] = _studentList[0].Id;
            rowOne[nameof(Student.Name)] = _studentList[0].Name;
            _dt.Rows.Add(rowOne);

            var rowTwo = _dt.NewRow();
            rowTwo[nameof(Student.Id)] = _studentList[1].Id;
            rowTwo[nameof(Student.Name)] = _studentList[1].Name;
            _dt.Rows.Add(rowTwo);
        }
    }

    [Test]
    public void WriteToExcelFile_WithNullDataTable_ShouldReturnResultFailure()
    {
        // Act & Assert
        var result = ExcelDataTableHelper.WriteToExcelFile(null);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(DomainErrors.CannotBeNull("dt"), result.Error);
    }

    [Test]
    public void WriteToExcelFile_ShouldReturnByteArray()
    {
        // Arrange
        string sheetName = "sample";
        _dt.TableName = sheetName;

        // Act
        var result = ExcelDataTableHelper.WriteToExcelFile(_dt);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = result.Value;
        Assert.IsNotNull(content);
        Assert.That(content.Length, Is.GreaterThan(0));

        AssertExcel(content, sheetName);
    }

    [Test]
    public void WriteToExcelFile_WithExcelColumnFormats_ShouldReturnByteArray()
    {
        // Arrange
        string sheetName = "sample";
        _dt.TableName = sheetName;
        var excelColumnFormats = new List<ExcelColumnFormat>
        {
            new()
            {
                ColumnName = nameof(Student.Id),
                Format = "0",
                HorizontalAlignment = ExcelHorizontalAlignment.Right
            },
            new()
            {
                ColumnName = nameof(Student.Name)
            }
        };

        // Act
        var result = ExcelDataTableHelper.WriteToExcelFile(_dt, excelColumnFormats: excelColumnFormats);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = result.Value;
        Assert.IsNotNull(content);
        Assert.That(content.Length, Is.GreaterThan(0));

        AssertExcel(content, sheetName, excelColumnFormats: excelColumnFormats);
    }

    [Test]
    public void WriteToExcelFile_WithExcelHeaderStyle_ShouldReturnByteArray()
    {
        // Arrange
        string sheetName = "sample";
        _dt.TableName = sheetName;
        var headerRowStyle = new ExcelRowStyle
        {
            Bold = true,
            PatternType = ExcelFillStyle.LightUp,
            BackgroundColor = Color.DarkBlue
        };

        // Act
        var result = ExcelDataTableHelper.WriteToExcelFile(_dt, headerRowStyle: headerRowStyle);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = result.Value;
        Assert.IsNotNull(result);
        Assert.That(content.Length, Is.GreaterThan(0));

        AssertExcel(content, sheetName, headerRowStyle: headerRowStyle);
    }

    private void AssertExcel(byte[] bytes,
        string sheetName,
        List<ExcelColumnFormat> excelColumnFormats = null,
        ExcelRowStyle headerRowStyle = null)
    {
        using MemoryStream ms = StreamHelper.ToMemoryStream(bytes).Value;
        using ExcelPackage package = new(ms);
        var worksheet = package.Workbook.Worksheets[0];

        // Sheet name assertion
        Assert.AreEqual(sheetName, worksheet.Name);

        // Data column assertion
        string columnOneName = _dt.Columns[0].ColumnName;
        string columnTwoName = _dt.Columns[1].ColumnName;
        DataRow rowOne = _dt.Rows[0];
        DataRow rowTwo = _dt.Rows[1];

        Assert.That(worksheet.Cells[2, 1].GetValue<int>(), Is.EqualTo(rowOne[columnOneName]));
        Assert.That(worksheet.Cells[2, 2].GetValue<string>(), Is.EqualTo(rowOne[columnTwoName]));

        Assert.That(worksheet.Cells[3, 1].GetValue<int>(), Is.EqualTo(rowTwo[columnOneName]));
        Assert.That(worksheet.Cells[3, 2].GetValue<string>(), Is.EqualTo(rowTwo[columnTwoName]));

        if (!excelColumnFormats.IsNullOrEmpty())
        {
            Assert.That(worksheet.Cells[2, 1].Style.HorizontalAlignment, Is.EqualTo(excelColumnFormats[0].HorizontalAlignment));
            Assert.That(worksheet.Cells[2, 1].Style.Numberformat.Format, Is.EqualTo(excelColumnFormats[0].Format));

            Assert.That(worksheet.Cells[3, 1].Style.HorizontalAlignment, Is.EqualTo(excelColumnFormats[0].HorizontalAlignment));
            Assert.That(worksheet.Cells[3, 1].Style.Numberformat.Format, Is.EqualTo(excelColumnFormats[0].Format));
        }

        if (headerRowStyle != null)
        {
            Assert.AreEqual(worksheet.Cells[1, 1].Style.Font.Bold, headerRowStyle.Bold);
            Assert.AreEqual(worksheet.Cells[1, 1].Style.Fill.PatternType, headerRowStyle.PatternType);
            Assert.AreEqual(worksheet.Cells[1, 1].Style.Fill.BackgroundColor.Rgb, headerRowStyle.BackgroundColor.ToArgb().ToString("X"));

            Assert.AreEqual(worksheet.Cells[1, 2].Style.Font.Bold, headerRowStyle.Bold);
            Assert.AreEqual(worksheet.Cells[1, 2].Style.Fill.PatternType, headerRowStyle.PatternType);
            Assert.AreEqual(worksheet.Cells[1, 2].Style.Fill.BackgroundColor.Rgb, headerRowStyle.BackgroundColor.ToArgb().ToString("X"));
        }
    }

    private class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
