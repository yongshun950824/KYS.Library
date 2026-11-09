using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using KYS.Library.Extensions;
using KYS.Library.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KYS.Library.Tests.HelpersUnitTests;

public class DataTableHelperUnitTest
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
    public void WriteToTextFile_WithDefault_ShouldReturnByteArray()
    {
        // Act
        var result = DataTableHelper.WriteToTextFile(_dt);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = Encoding.UTF8.GetString(result.Value);
        Assert.That(content, Does.Contain($"{nameof(Student.Id)}{SEPARATOR}{nameof(Student.Name)}"));
        Assert.That(content, Does.Contain($"{_studentList[0].Id}{SEPARATOR}{_studentList[0].Name}"));
        Assert.That(content, Does.Contain($"{_studentList[1].Id}{SEPARATOR}{_studentList[1].Name}"));
    }

    [Test]
    public void WriteToTextFile_WithEmptyDataTable_ShouldReturnByteArray()
    {
        // Act
        var result = DataTableHelper.WriteToTextFile(new DataTable());

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = Encoding.UTF8.GetString(result.Value);
        Assert.AreEqual(String.Empty, content);
    }

    [Test]
    public void WriteToTextFile_WithoutHeaders_ShouldReturnByteArray()
    {
        // Act
        var result = DataTableHelper.WriteToTextFile(_dt, printHeaders: false);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = Encoding.UTF8.GetString(result.Value);
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
        var result = DataTableHelper.WriteToTextFile(_dt, delimiter: separator);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = Encoding.UTF8.GetString(result.Value);
        Assert.That(content, Does.Contain($"{nameof(Student.Id)}{separator}{nameof(Student.Name)}"));
        Assert.That(content, Does.Contain($"{_studentList[0].Id}{separator}{_studentList[0].Name}"));
        Assert.That(content, Does.Contain($"{_studentList[1].Id}{separator}{_studentList[1].Name}"));
    }

    [Test]
    public void WriteToTextFile_WithProvidedCulture_ShouldReturnByteArray()
    {
        // Arrange
        CultureInfo culture = new("th-TH");

        // Act
        var result = DataTableHelper.WriteToTextFile(_dt, cultureInfo: culture);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = Encoding.UTF8.GetString(result.Value);
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
        var result = DataTableHelper.WriteToJsonFile(_dt);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = Encoding.UTF8.GetString(result.Value);
        Assert.AreEqual(expectedResult, content);
    }

    [Test]
    public void WriteToJsonFile_WithNonIndent_ShouldReturnByteArray()
    {
        // Arrange
        var expectedResult = JsonConvert.SerializeObject(_studentList);

        // Act
        var result = DataTableHelper.WriteToJsonFile(_dt, false);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = Encoding.UTF8.GetString(result.Value);
        Assert.AreEqual(expectedResult, content);
    }

    [Test]
    public void WriteToJsonFile_WithEmptyDataTable_ShouldReturnByteArray()
    {
        // Arrange
        var expectedResult = "[]";

        // Act
        var result = DataTableHelper.WriteToJsonFile(new DataTable());

        // Assert        
        Assert.IsTrue(result.IsSuccess);

        var content = Encoding.UTF8.GetString(result.Value);
        Assert.AreEqual(expectedResult, content);
    }

    [Test]
    public void ReadCSV_WithValidFilePath_ShouldReturnDataTable()
    {
        // Act
        var result = DataTableHelper.ReadCSV(_csvFilePath);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        DataTable dt = result.Value;
        Assert.IsNotNull(dt);
        Assert.IsTrue(dt.Columns.Count > 0);
        Assert.IsTrue(dt.Rows.Count > 0);
    }

    [Test]
    public void ReadCSV_WithInvalidFilePath_ShouldReturnResultFailure()
    {
        // Act
        var result = DataTableHelper.ReadCSV((string)null);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(DomainErrors.Required("filePath"), result.Error);
    }

    [Test]
    public void ReadCSV_WithNotCSVFilePath_ShouldReturnResultFailure()
    {
        // Arrange
        string filePath = Directory.GetCurrentDirectory() + "\\Resources\\DataTableHelper\\data.txt";

        // Act
        var result = DataTableHelper.ReadCSV(filePath);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Provided filePath must be a CSV file.", result.Error);
    }

    [Test]
    public void ReadCSV_WithNullStream_ShouldReturnResultFailure()
    {
        // Act
        var result = DataTableHelper.ReadCSV((Stream)null);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(DomainErrors.CannotBeNull("stream"), result.Error);
    }

    [Test]
    public void ReadCSV_WithStream_ShouldReturnDataTable()
    {
        // Arrange
        var stream = new MemoryStream(File.ReadAllBytes(_csvFilePath));

        // Act
        var result = DataTableHelper.ReadCSV(stream);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        DataTable dt = result.Value;
        Assert.IsTrue(dt.Rows.Count > 0);
        Assert.IsTrue(dt.Columns.Count > 0);
    }

    private class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
