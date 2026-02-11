using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Kernel.Exceptions;
using KYS.Library.Helpers;
using NUnit.Framework;

namespace KYS.Library.Tests.HelpersUnitTests;

public class PdfHelperUnitTest
{
    private string _pdfFilePath;
    private string _protectedPdfFilePath;
    private string _htmlContent;
    private List<string> _montserratFontFiles;

    [SetUp]
    public void Setup()
    {
        _pdfFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "PdfHelper", "sample-PDF.pdf");
        _protectedPdfFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "PdfHelper", "sample-PDF-protected.pdf");

        _htmlContent = @"
            <html>
                <h2>Hello world!</h2>
                <p>This is a sample.</p>
            </html>
        ";

        _montserratFontFiles =
        [
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "PdfHelper", "Montserrat", "Montserrat-Bold.ttf"),
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "PdfHelper", "Montserrat", "Montserrat-Italic.ttf"),
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "PdfHelper", "Montserrat", "Montserrat-Medium.ttf"),
            Path.Combine(Directory.GetCurrentDirectory(), "Resources", "PdfHelper", "Montserrat", "Montserrat-Regular.ttf")
        ];
    }

    [Test]
    public void IsPasswordProtected_WithProtectedPdf_ShouldReturnTrue()
    {
        // Arrange
        using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_protectedPdfFilePath);
        byte[] bytes = StreamHelper.ToByteArray(ms);

        // Act
        bool isProtected = PdfHelper.IsPasswordProtected(bytes);

        // Assert
        Assert.IsTrue(isProtected);
    }

    [Test]
    public void IsPasswordProtected_WithUnprotectedPdf_ShouldReturnTrue()
    {
        // Arrange
        using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_pdfFilePath);
        byte[] bytes = StreamHelper.ToByteArray(ms);

        // Act
        bool isProtected = PdfHelper.IsPasswordProtected(bytes);

        // Assert
        Assert.IsFalse(isProtected);
    }

    [Test]
    public void LockPdfDocument_WithoutPassword_ShouldThrowException()
    {
        // Arrange
        using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_pdfFilePath);
        byte[] bytes = StreamHelper.ToByteArray(ms);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => PdfHelper.LockPdfDocument(bytes, null));
    }

    [Test]
    public void LockPdfDocument_ShouldLockPdf()
    {
        // Arrange
        using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_pdfFilePath);
        byte[] bytes = StreamHelper.ToByteArray(ms);

        // Act & Assert
        byte[] lockedPdfFileBytes = PdfHelper.LockPdfDocument(bytes, "Qwer1234");

        // Assert
        Assert.IsNotNull(lockedPdfFileBytes);
        Assert.That(lockedPdfFileBytes.Length, Is.GreaterThan(0));
        Assert.IsTrue(PdfHelper.IsPasswordProtected(lockedPdfFileBytes));
    }

    [Test]
    public void UnlockPdfDocument_WithoutPassword_ShouldThrowException()
    {
        // Arrange
        using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_pdfFilePath);
        byte[] bytes = StreamHelper.ToByteArray(ms);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => PdfHelper.UnlockPdfDocument(bytes, null));
    }

    [Test]
    public void UnlockPdfDocument_WithWrongPassword_ShouldThrowException()
    {
        // Arrange
        using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_protectedPdfFilePath);
        byte[] bytes = StreamHelper.ToByteArray(ms);

        // Act & Assert
        Assert.Throws<BadPasswordException>(() => PdfHelper.UnlockPdfDocument(bytes, "Abcd1234"));
    }

    [Test]
    public void UnlockPdfDocument_WithNotProtectedPdf_ShouldReturnByteArray()
    {
        // Arrange
        using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_pdfFilePath);
        byte[] bytes = StreamHelper.ToByteArray(ms);

        // Act & Assert
        var unlockedPdfFileBytes = PdfHelper.UnlockPdfDocument(bytes, "Qwer1234");

        // Assert
        Assert.IsNotNull(unlockedPdfFileBytes);
        Assert.That(unlockedPdfFileBytes.Length, Is.GreaterThan(0));
        Assert.IsFalse(PdfHelper.IsPasswordProtected(unlockedPdfFileBytes));
    }

    [Test]
    public void UnlockPdfDocument_WithProtectedPdf_ShouldReturnByteArray()
    {
        // Arrange
        using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_protectedPdfFilePath);
        byte[] bytes = StreamHelper.ToByteArray(ms);

        // Act & Assert
        var unlockedPdfFileBytes = PdfHelper.UnlockPdfDocument(bytes, "Qwer1234");

        // Assert
        Assert.IsNotNull(unlockedPdfFileBytes);
        Assert.That(unlockedPdfFileBytes.Length, Is.GreaterThan(0));
        Assert.IsFalse(PdfHelper.IsPasswordProtected(unlockedPdfFileBytes));
    }

    [Test]
    public void ConvertHtmlToPdf_WithDefault_ShouldReturnMemoryStream()
    {
        // Act
        using var ms = PdfHelper.ConvertHtmlToPdf(_htmlContent);

        // Assert
        Assert.That(ms, Is.Not.Null);
        Assert.That(ms.Length, Is.GreaterThan(0));

        string header = Encoding.ASCII.GetString(ms.ToArray(), 0, 5);
        Assert.That(header, Is.EqualTo("%PDF-"));
    }

    [Test]
    public void ConvertHtmlToPdf_WithCustomFonts_ShouldReturnMemoryStream()
    {
        // Act
        using var ms = PdfHelper.ConvertHtmlToPdf(_htmlContent, importCustomFontFamilyFilePaths: _montserratFontFiles);

        // Assert
        Assert.That(ms, Is.Not.Null);
        Assert.That(ms.Length, Is.GreaterThan(0));

        string header = Encoding.ASCII.GetString(ms.ToArray(), 0, 5);
        Assert.That(header, Is.EqualTo("%PDF-"));
    }

    [Test]
    public void ConvertHtmlToPdf_WithDisableRegisterSystemFonts_ShouldReturnMemoryStream()
    {
        // Act
        using var ms = PdfHelper.ConvertHtmlToPdf(_htmlContent, registerSystemFonts: false);

        // Assert
        Assert.That(ms, Is.Not.Null);
        Assert.That(ms.Length, Is.GreaterThan(0));

        string header = Encoding.ASCII.GetString(ms.ToArray(), 0, 5);
        Assert.That(header, Is.EqualTo("%PDF-"));
    }

    [Test]
    public void ConvertHtmlToPdf_WithDisableRegisterStandardPdfFonts_ShouldReturnMemoryStream()
    {
        // Act
        using var ms = PdfHelper.ConvertHtmlToPdf(_htmlContent, registerStandardPdfFonts: false);

        // Assert
        Assert.That(ms, Is.Not.Null);
        Assert.That(ms.Length, Is.GreaterThan(0));

        string header = Encoding.ASCII.GetString(ms.ToArray(), 0, 5);
        Assert.That(header, Is.EqualTo("%PDF-"));
    }

    [Test]
    public void ConvertHtmlToPdf_WithDisableRegisterShippedFonts_ShouldReturnMemoryStream()
    {
        // Act
        using var ms = PdfHelper.ConvertHtmlToPdf(_htmlContent, registerShippedFonts: false);

        // Assert
        Assert.That(ms, Is.Not.Null);
        Assert.That(ms.Length, Is.GreaterThan(0));

        string header = Encoding.ASCII.GetString(ms.ToArray(), 0, 5);
        Assert.That(header, Is.EqualTo("%PDF-"));
    }
}
