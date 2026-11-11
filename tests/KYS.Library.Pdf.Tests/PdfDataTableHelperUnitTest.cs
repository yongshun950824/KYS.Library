using System.Data;
using iText.Layout;
using iText.Layout.Properties;
using KYS.Library.Extensions;
using KYS.Library.Helpers;
using PdfDataTableHelper = KYS.Library.Pdf.Helpers.PdfDataTableHelper;

namespace KYS.Library.Pdf.Tests.HelpersUnitTests;

public class PdfDataTableHelperUnitTest
{
    private readonly DataTable _dt = new();
    private readonly List<Student> _studentList = new()
    {
        new Student { Id = 1, Name = "Ali" },
        new Student { Id = 2, Name = "Bob" }
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
    public void WriteToPdfFile_WithNullDataTable_ShouldReturnResultFailure()
    {
        // Act
        var result = PdfDataTableHelper.WriteToPdfFile(null);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(DomainErrors.CannotBeNull("dt"), result.Error);
    }

    [Test]
    public void WriteToPdfFile_WithEmptyDataTable_ShouldReturnByteArray()
    {
        // Act
        var result = PdfDataTableHelper.WriteToPdfFile(new DataTable());

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = result.Value;
        Assert.IsNotNull(content);
        Assert.That(content.Length, Is.GreaterThan(0));
    }

    [Test]
    public void WriteToPdfFile_ShouldReturnByteArray()
    {
        // Act
        var result = PdfDataTableHelper.WriteToPdfFile(_dt);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = result.Value;
        Assert.IsNotNull(content);
        Assert.That(content.Length, Is.GreaterThan(0));
    }

    [Test]
    public void WriteToPdfFile_WithExcludeHeaderColumn_ShouldReturnByteArray()
    {
        // Act
        var result = PdfDataTableHelper.WriteToPdfFile(_dt, printHeaders: false);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = result.Value;
        Assert.IsNotNull(content);
        Assert.That(content.Length, Is.GreaterThan(0));
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
        var result = PdfDataTableHelper.WriteToPdfFile(_dt, tableBodyStyle: tableBodyStyle);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        var content = result.Value;
        Assert.IsNotNull(content);
        Assert.That(content.Length, Is.GreaterThan(0));
    }

    private class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
