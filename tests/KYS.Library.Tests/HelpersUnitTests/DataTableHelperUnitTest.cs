using iText.Layout;
using iText.Layout.Properties;
using KYS.Library.Extensions;
using KYS.Library.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace KYS.Library.Tests.HelpersUnitTests;

public class DataTableHelperUnitTest
{
    private readonly DataTable _dt = new DataTable();
    private readonly List<Student> _studentList = new List<Student>
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
    public void WriteToTextFile_WithDefault_ShouldReturnByteArray()
    {
        // Act
        var bytes = DataTableHelper.WriteToTextFile(_dt);
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        Assert.That(content, Does.Contain($"{nameof(Student.Id)}{SEPARATOR}{nameof(Student.Name)}"));
        Assert.That(content, Does.Contain($"{_studentList[0].Id}{SEPARATOR}{_studentList[0].Name}"));
        Assert.That(content, Does.Contain($"{_studentList[1].Id}{SEPARATOR}{_studentList[1].Name}"));
    }

    [Test]
    public void WriteToTextFile_WithEmptyDataTable_ShouldReturnByteArray()
    {
        // Act
        var bytes = DataTableHelper.WriteToTextFile(new DataTable());
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        Assert.AreEqual(String.Empty, content);
    }

    [Test]
    public void WriteToTextFile_WithoutHeaders_ShouldReturnByteArray()
    {
        // Act
        var bytes = DataTableHelper.WriteToTextFile(_dt, printHeaders: false);
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        Assert.That(content, !Does.Contain($"{nameof(Student.Id)}{SEPARATOR}{nameof(Student.Name)}"));
        Assert.That(content, Does.Contain($"{_studentList[0].Id}{SEPARATOR}{_studentList[0].Name}"));
        Assert.That(content, Does.Contain($"{_studentList[1].Id}{SEPARATOR}{_studentList[1].Name}"));
    }

    [Test]
    public void WriteToTextFile_WithProvidedDelimiters_ShouldReturnByteArray()
    {
        // Arrange
        string separator = ",";

        // Act
        var bytes = DataTableHelper.WriteToTextFile(_dt, delimiter: separator);
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        Assert.That(content, Does.Contain($"{nameof(Student.Id)}{separator}{nameof(Student.Name)}"));
        Assert.That(content, Does.Contain($"{_studentList[0].Id}{separator}{_studentList[0].Name}"));
        Assert.That(content, Does.Contain($"{_studentList[1].Id}{separator}{_studentList[1].Name}"));
    }

    [Test]
    public void WriteToTextFile_WithProvidedCulture_ShouldReturnByteArray()
    {
        // Arrange
        CultureInfo culture = new CultureInfo("th-TH");

        // Act
        var bytes = DataTableHelper.WriteToTextFile(_dt, cultureInfo: culture);
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        Assert.That(content, Does.Contain($"{nameof(Student.Id)}{SEPARATOR}{nameof(Student.Name)}"));
        Assert.That(content, Does.Contain($"{_studentList[0].Id}{SEPARATOR}{_studentList[0].Name}"));
        Assert.That(content, Does.Contain($"{_studentList[1].Id}{SEPARATOR}{_studentList[1].Name}"));
    }

    [Test]
    public void WriteToJsonFile_WithDefault_ShouldReturnByteArray()
    {
        // Arrange
        var expectedResult = JsonConvert.SerializeObject(_studentList, Newtonsoft.Json.Formatting.Indented);

        // Act
        var bytes = DataTableHelper.WriteToJsonFile(_dt);
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        Assert.AreEqual(expectedResult, content);
    }

    [Test]
    public void WriteToJsonFile_WithNonIndent_ShouldReturnByteArray()
    {
        // Arrange
        var expectedResult = JsonConvert.SerializeObject(_studentList);

        // Act
        var bytes = DataTableHelper.WriteToJsonFile(_dt, false);
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        Assert.AreEqual(expectedResult, content);
    }

    [Test]
    public void WriteToJsonFile_WithEmptyDataTable_ShouldReturnByteArray()
    {
        // Arrange
        var expectedResult = "[]";

        // Act
        var bytes = DataTableHelper.WriteToJsonFile(new DataTable());
        var content = Encoding.UTF8.GetString(bytes);

        // Assert
        Assert.AreEqual(expectedResult, content);
    }

    [Test]
    public void ReadCSV_WithValidFilePath_ShouldReturnDataTable()
    {
        // Act
        DataTable dt = DataTableHelper.ReadCSV(_csvFilePath);

        // Assert
        Assert.IsNotNull(dt);
        Assert.IsTrue(dt.Columns.Count > 0);
        Assert.IsTrue(dt.Rows.Count > 0);
    }

    [Test]
    public void ReadCSV_WithInvalidFilePath_ShouldReturnDataTable()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => DataTableHelper.ReadCSV((string)null));
    }

    [Test]
    public void ReadCSV_WithNotCSVFilePath_ShouldReturnDataTable()
    {
        // Arrange
        string filePath = Directory.GetCurrentDirectory() + "\\Resources\\DataTableHelper\\data.txt";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => DataTableHelper.ReadCSV(filePath));

        Assert.AreEqual("Provided filePath must be a CSV file.", ex.Message);
    }

    [Test]
    public void ReadCSV_WithNullStream_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => DataTableHelper.ReadCSV((Stream)null));
    }

    [Test]
    public void ReadCSV_WithStream_ShouldReturnDataTable()
    {
        // Arrange
        var stream = FileHelper.LoadFileToMemoryStream(_csvFilePath);

        // Act & Assert
        DataTable dt = DataTableHelper.ReadCSV(stream);

        Assert.IsTrue(dt.Rows.Count > 0);
        Assert.IsTrue(dt.Columns.Count > 0);
    }

    [Test]
    public void WriteToExcelFile_WithNullDataTable_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => DataTableHelper.WriteToExcelFile(null));
    }

    [Test]
    public void WriteToExcelFile_ShouldReturnByteArray()
    {
        // Arrange
        string sheetName = "sample";
        _dt.TableName = sheetName;

        // Act
        byte[] result = DataTableHelper.WriteToExcelFile(_dt);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, sheetName);
    }

    [Test]
    public void WriteToExcelFile_WithExcelColumnFormats_ShouldReturnByteArray()
    {
        // Arrange
        string sheetName = "sample";
        _dt.TableName = sheetName;
        var excelColumnFormats = new List<ExcelHelper.ExcelColumnFormat>
        {
            new ExcelHelper.ExcelColumnFormat
            {
                ColumnName = nameof(Student.Id),
                Format = "0",
                HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right
            },
            new ExcelHelper.ExcelColumnFormat
            {
                ColumnName = nameof(Student.Name)
            }
        };

        // Act
        byte[] result = DataTableHelper.WriteToExcelFile(_dt, excelColumnFormats: excelColumnFormats);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, sheetName, excelColumnFormats: excelColumnFormats);
    }

    [Test]
    public void WriteToExcelFile_WithExcelHeaderStyle_ShouldReturnByteArray()
    {
        // Arrange
        string sheetName = "sample";
        _dt.TableName = sheetName;
        var headerRowStyle = new ExcelHelper.ExcelRowStyle
        {
            Bold = true,
            PatternType = ExcelFillStyle.LightUp,
            BackgroundColor = Color.DarkBlue
        };

        // Act
        byte[] result = DataTableHelper.WriteToExcelFile(_dt, headerRowStyle: headerRowStyle);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, sheetName, headerRowStyle: headerRowStyle);
    }

    [Test]
    public void WriteToPdfFile_WithNullDataTable_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => DataTableHelper.WriteToPdfFile(null));
    }

    [Test]
    public void WriteToPdfFile_WithEmptyDataTable_ShouldReturnByteArray()
    {
        // Act
        byte[] result = DataTableHelper.WriteToPdfFile(new DataTable());

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));
    }

    [Test]
    public void WriteToPdfFile_ShouldReturnByteArray()
    {
        // Act
        byte[] result = DataTableHelper.WriteToPdfFile(_dt);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));
    }

    [Test]
    public void WriteToPdfFile_WithExcludeHeaderColumn_ShouldReturnByteArray()
    {
        // Act
        byte[] result = DataTableHelper.WriteToPdfFile(_dt, printHeaders: false);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));
    }

    [Test]
    public void WriteToPdfFile_WithTableBodyStyle_ShouldReturnByteArray()
    {
        // Arrange
        Style tableBodyStyle = new Style()
            .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.WHITE)
            .SetFontSize(11)
            .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK)
            .SetTextAlignment(TextAlignment.LEFT);

        // Act
        byte[] result = DataTableHelper.WriteToPdfFile(_dt, tableBodyStyle: tableBodyStyle);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));
    }

    private void AssertExcel(byte[] bytes,
        string sheetName,
        List<ExcelHelper.ExcelColumnFormat> excelColumnFormats = null,
        ExcelHelper.ExcelRowStyle headerRowStyle = null)
    {
        using MemoryStream ms = StreamHelper.ToMemoryStream(bytes);
        using ExcelPackage package = new ExcelPackage(ms);
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
