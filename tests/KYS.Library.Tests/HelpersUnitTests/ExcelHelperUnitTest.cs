using System;
using System.Data;
using NUnit.Framework;
using KYS.Library.Helpers;
using KYS.Library.Extensions;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using System.Linq;
using System.Drawing;

namespace KYS.Library.Tests.HelpersUnitTests;

public class ExcelHelperUnitTest
{
    private readonly DataTable _dt = new DataTable();
    private readonly List<Student> _studentList = new List<Student>
    {
        new Student { Id = 1, Name = "Ali", Age = 10 },
        new Student {  Id = 2, Name = "Bob", Age = 11 }
    };
    private readonly DataTable _dtTwo = new DataTable();
    private readonly List<Student> _studentTwoList = new List<Student>
    {
        new Student { Id = 3, Name = "Charles", Age = 12 },
        new Student {  Id = 4, Name = "Danny", Age = 13 }
    };

    [SetUp]
    public void Setup()
    {
        if (_dt.IsNullOrEmpty())
        {
            _dt.Columns.Add(nameof(Student.Id), typeof(int));
            _dt.Columns.Add(nameof(Student.Name), typeof(string));
            _dt.Columns.Add(nameof(Student.Age), typeof(int));

            var rowOne = _dt.NewRow();
            rowOne[nameof(Student.Id)] = _studentList[0].Id;
            rowOne[nameof(Student.Name)] = _studentList[0].Name;
            rowOne[nameof(Student.Age)] = _studentList[0].Age;
            _dt.Rows.Add(rowOne);

            var rowTwo = _dt.NewRow();
            rowTwo[nameof(Student.Id)] = _studentList[1].Id;
            rowTwo[nameof(Student.Name)] = _studentList[1].Name;
            rowTwo[nameof(Student.Age)] = _studentList[1].Age;
            _dt.Rows.Add(rowTwo);
        }

        if (_dtTwo.IsNullOrEmpty())
        {
            _dtTwo.Columns.Add(nameof(Student.Id), typeof(int));
            _dtTwo.Columns.Add(nameof(Student.Name), typeof(string));
            _dtTwo.Columns.Add(nameof(Student.Age), typeof(int));

            var rowOne = _dtTwo.NewRow();
            rowOne[nameof(Student.Id)] = _studentTwoList[0].Id;
            rowOne[nameof(Student.Name)] = _studentTwoList[0].Name;
            rowOne[nameof(Student.Age)] = _studentTwoList[0].Age;
            _dtTwo.Rows.Add(rowOne);

            var rowTwo = _dtTwo.NewRow();
            rowTwo[nameof(Student.Id)] = _studentTwoList[1].Id;
            rowTwo[nameof(Student.Name)] = _studentTwoList[1].Name;
            rowTwo[nameof(Student.Age)] = _studentTwoList[1].Age;
            _dtTwo.Rows.Add(rowTwo);
        }
    }

    [Test]
    public void CreateExcelBook_WithNullDataSet_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ExcelHelper.CreateExcelBook((DataSet)null));
    }

    [Test]
    public void CreateExcelBook_WithDataSet_ShouldReturnByteArray()
    {
        // Arrange
        string sheetName = "sample";
        _dt.TableName = sheetName;

        DataSet ds = new DataSet();
        ds.Tables.Add(_dt);

        // Act
        byte[] result = ExcelHelper.CreateExcelBook(ds);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, _dt);
    }

    [Test]
    public void CreateExcelBook_WithNullDataTable_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ExcelHelper.CreateExcelBook((DataTable)null));
    }

    [Test]
    public void CreateExcelBook_WithDataTable_ShouldReturnByteArray()
    {
        // Arrange
        _dt.TableName = "sample";

        // Act
        byte[] result = ExcelHelper.CreateExcelBook(_dt);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, _dt);
    }

    [Test]
    public void CreateExcelBook_WithExcelColumnFormats_ShouldReturnByteArray()
    {
        // Arrange
        _dt.TableName = "sample";

        List<ExcelHelper.ExcelColumnFormat> excelColumnFormats = new List<ExcelHelper.ExcelColumnFormat>
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
        byte[] result = ExcelHelper.CreateExcelBook(_dt, excelColumnFormats);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, _dt, excelColumnFormats: excelColumnFormats);
    }

    [Test]
    public void CreateExcelBook_WithExcelColumnFormatsHaveDisplayedColumnName_ShouldReturnByteArray()
    {
        // Arrange
        _dt.TableName = "sample";

        List<ExcelHelper.ExcelColumnFormat> excelColumnsFormat = new List<ExcelHelper.ExcelColumnFormat>
        {
            new ExcelHelper.ExcelColumnFormat
            {
                ColumnName = nameof(Student.Id),
                DisplayedColumnName = "Student ID",
                Format = "0",
                HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right
            },
            new ExcelHelper.ExcelColumnFormat
            {
                ColumnName = nameof(Student.Name),
                DisplayedColumnName = "Full Name"
            }
        };

        // Act
        byte[] result = ExcelHelper.CreateExcelBook(_dt, excelColumnsFormat);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, _dt, excelColumnFormats: excelColumnsFormat);
    }

    [Test]
    public void CreateExcelBook_WithExcelColumnFormatsHaveSumColumn_ShouldReturnByteArray()
    {
        // Arrange
        _dt.TableName = "sample";

        List<ExcelHelper.ExcelColumnFormat> excelColumnFormats = new List<ExcelHelper.ExcelColumnFormat>
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
            },
            new ExcelHelper.ExcelColumnFormat
            {
                ColumnName = nameof(Student.Age),
                HasSumColumn = true
            }
        };

        // Act
        byte[] result = ExcelHelper.CreateExcelBook(_dt, excelColumnFormats);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, _dt, excelColumnFormats: excelColumnFormats);
    }

    [Test]
    public void CreateExcelBook_WithAdditionalExcelSheetHasNullDataTable_ShouldReturnByteArray()
    {
        // Arrange
        _dt.TableName = "sample";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            List<ExcelHelper.AdditionalExcelSheet> additionalExcelSheets = new List<ExcelHelper.AdditionalExcelSheet>
            {
                new ExcelHelper.AdditionalExcelSheet
                {
                    DataTable = null
                }
            };

            ExcelHelper.CreateExcelBook(_dt, additionalExcelSheets: additionalExcelSheets);
        });
    }

    [Test]
    public void CreateExcelBook_WithAdditionalExcelSheet_ShouldReturnByteArray()
    {
        // Arrange
        _dt.TableName = "sample";

        _dtTwo.TableName = "sample-2";
        List<ExcelHelper.AdditionalExcelSheet> additionalExcelSheets = new List<ExcelHelper.AdditionalExcelSheet>
        {
            new ExcelHelper.AdditionalExcelSheet
            {
                DataTable = _dtTwo
            }
        };

        // Act
        byte[] result = ExcelHelper.CreateExcelBook(_dt, additionalExcelSheets: additionalExcelSheets);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, _dt);

        int sheetIndex = 1;
        foreach (var sheet in additionalExcelSheets)
        {
            AssertExcel(result, sheet.DataTable, sheetIndex, sheet.ExcelColumnFormats, sheet.HeaderRowStyle);

            sheetIndex++;
        }
    }

    [Test]
    public void CreateExcelBook_WithHeaderRowStyle_ShouldReturnByteArray()
    {
        // Arrange
        _dt.TableName = "sample";

        ExcelHelper.ExcelRowStyle headerRowStyle = new ExcelHelper.ExcelRowStyle
        {
            BackgroundColor = Color.DarkBlue,
            FontColor = Color.White,
            AutoFilter = true
        };

        // Act
        byte[] result = ExcelHelper.CreateExcelBook(_dt, headerRowStyle: headerRowStyle);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, _dt, headerRowStyle: headerRowStyle);
    }

    [Test]
    public void CreateExcelBook_WithSummaryRowStyle_ShouldReturnByteArray()
    {
        // Arrange
        _dt.TableName = "sample";

        ExcelHelper.ExcelRowStyle summaryRowStyle = new ExcelHelper.ExcelRowStyle
        {
            BackgroundColor = Color.ForestGreen,
            FontColor = Color.Black
        };

        // Act
        byte[] result = ExcelHelper.CreateExcelBook(_dt, summaryRowStyle: summaryRowStyle);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Length, Is.GreaterThan(0));

        AssertExcel(result, _dt, summaryRowStyle: summaryRowStyle);
    }

    private static void AssertExcel(byte[] bytes,
        DataTable dt,
        int sheetIndex = 0,
        List<ExcelHelper.ExcelColumnFormat> excelColumnFormats = null,
        ExcelHelper.ExcelRowStyle headerRowStyle = null,
        ExcelHelper.ExcelRowStyle summaryRowStyle = null)
    {
        using MemoryStream ms = StreamHelper.ToMemoryStream(bytes);
        using ExcelPackage package = new ExcelPackage(ms);
        var worksheet = package.Workbook.Worksheets[sheetIndex];

        // Sheet name assertion
        Assert.AreEqual(dt.TableName, worksheet.Name);

        // Data column assertion
        string columnOneName = dt.Columns[0].ColumnName;
        string columnTwoName = dt.Columns[1].ColumnName;
        DataRow rowOne = dt.Rows[0];
        DataRow rowTwo = dt.Rows[1];

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

            int colIndex = 1;
            foreach (var format in excelColumnFormats)
            {
                if (!String.IsNullOrEmpty(format.DisplayedColumnName))
                {
                    Assert.That(worksheet.Cells[1, colIndex].GetValue<string>(), Is.EqualTo(format.DisplayedColumnName));
                }
                else
                {
                    Assert.That(worksheet.Cells[1, colIndex].GetValue<string>(), Is.EqualTo(format.ColumnName));
                }

                if (format.HasSumColumn)
                {
                    Assert.That(worksheet.Cells[4, colIndex].GetValue<int>(), Is.EqualTo(dt.Compute($"SUM({dt.Columns[colIndex - 1]})", null)));
                }

                colIndex++;
            }
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

        if (summaryRowStyle != null)
        {
            Assert.AreEqual(worksheet.Cells[4, 1].Style.Font.Bold, summaryRowStyle.Bold);
            Assert.AreEqual(worksheet.Cells[4, 1].Style.Fill.PatternType, summaryRowStyle.PatternType);
            Assert.AreEqual(worksheet.Cells[4, 1].Style.Fill.BackgroundColor.Rgb, summaryRowStyle.BackgroundColor.ToArgb().ToString("X"));

            Assert.AreEqual(worksheet.Cells[4, 2].Style.Font.Bold, summaryRowStyle.Bold);
            Assert.AreEqual(worksheet.Cells[4, 2].Style.Fill.PatternType, summaryRowStyle.PatternType);
            Assert.AreEqual(worksheet.Cells[4, 2].Style.Fill.BackgroundColor.Rgb, summaryRowStyle.BackgroundColor.ToArgb().ToString("X"));
        }
    }

    private class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
