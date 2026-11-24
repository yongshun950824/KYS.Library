using CSharpFunctionalExtensions;
using KYS.Library.Extensions;
using KYS.Library.Qr.Models;
using KYS.Library.Qr.Services;

namespace KYS.Library.Qr.Tests.ServicesUnitTests;

public class QrGeneratorUnitTest
{
    private string? _logoPath;
    private string? _outputDirectoryPath;

    private string? _qrValue;

    private readonly double MAX_LOGO_AREA_RATIO = QrGenerator.MAX_LOGO_AREA_RATIO;

    [SetUp]
    public void SetUp()
    {
        _logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "logo.png");
        _outputDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "OutDir");

        Directory.CreateDirectory(_outputDirectoryPath);

        _qrValue = "Sample value";
    }

    [Test]
    public void Generate_WithDefaultSize_ShouldGenerateQR()
    {
        // Arrange
        var qrCodeRequest = new QrCodeRequest(_qrValue!);
        var qrGeneratorService = new QrGenerator();

        // Act
        var qrCodeResult = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsTrue(qrCodeResult.IsSuccess);
        Assert.IsNotNull(qrCodeResult.Value);
    }

    [Test]
    public void Generate_WithSpecifiedSize_ShouldGenerateQR()
    {
        // Arrange
        int width = 100;
        int height = 100;
        int margin = 2;

        var qrCodeRequest = new QrCodeRequest(_qrValue!, width, height, margin);
        var qrGeneratorService = new QrGenerator();

        // Act
        var qrCodeResult = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsTrue(qrCodeResult.IsSuccess);
        Assert.IsNotNull(qrCodeResult.Value);
    }

    [Test]
    public void Generate_WithDefaultSizeLogo_ShouldGenerateQR()
    {
        // Arrange
        var qrCodeRequest = new QrCodeRequest(_qrValue!, LogoPath: _logoPath);
        var qrGeneratorService = new QrGenerator();

        // Act
        var qrCodeResult = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsTrue(qrCodeResult.IsSuccess);
        Assert.IsNotNull(qrCodeResult.Value);
    }

    [Test]
    public void Generate_WithLogoExceedsAspectRatio_ShouldReturnResultFailure()
    {
        // Arrange
        int logoWidth = 80;
        int logoHeight = 20;

        var qrCodeRequest = new QrCodeRequest(_qrValue!,
            LogoPath: _logoPath,
            LogoWidth: logoWidth,
            LogoHeight: logoHeight);
        var qrGeneratorService = new QrGenerator();

        // Act
        var qrCodeResult = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsFalse(qrCodeResult.IsSuccess);
        Assert.AreEqual("Logo aspect ratio is too wide or tall. Keep it closer to a square.", qrCodeResult.Error);
    }

    [Test]
    public void Generate_WithLogoIsSmallThanMinLogoAreaRatio_ShouldReturnResultFailure()
    {
        // Arrange
        int logoWidth = 1;
        int logoHeight = 1;

        var qrCodeRequest = new QrCodeRequest(_qrValue!,
            LogoPath: _logoPath,
            LogoWidth: logoWidth,
            LogoHeight: logoHeight);
        var qrGeneratorService = new QrGenerator();

        // Act
        var qrCodeResult = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsFalse(qrCodeResult.IsSuccess);
        Assert.AreEqual("Logo is too small and may not be visible.", qrCodeResult.Error);
    }

    [Test]
    public void Generate_WithLogoIsGreaterThanMaxLogoAreaRatio_ShouldReturnResultFailure()
    {
        // Arrange
        int logoWidth = 80;
        int logoHeight = 80;

        var qrCodeRequest = new QrCodeRequest(_qrValue!,
            LogoPath: _logoPath,
            LogoWidth: logoWidth,
            LogoHeight: logoHeight);
        var qrGeneratorService = new QrGenerator();
        var areaRatio = QrGenerator.GetLogoAreaRatio(qrCodeRequest);

        // Act
        var qrCodeResult = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsFalse(qrCodeResult.IsSuccess);
        Assert.AreEqual($"Logo is too large ({areaRatio:P1} of QR area). Keep it under {MAX_LOGO_AREA_RATIO:P0} for reliable scanning.", qrCodeResult.Error);
    }

    [Test]
    public void Generate_WithSpecifiedSizeLogo_ShouldGenerateQR()
    {
        // Arrange
        int logoWidth = 20;
        int logoHeight = 20;
        int margin = 1;

        var qrCodeRequest = new QrCodeRequest(_qrValue!,
            Margin: margin,
            LogoPath: _logoPath,
            LogoWidth: logoWidth,
            LogoHeight: logoHeight);
        var qrGeneratorService = new QrGenerator();

        // Act
        var qrCodeResult = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsTrue(qrCodeResult.IsSuccess);
        Assert.IsNotNull(qrCodeResult.Value);
    }

    [Test]
    public void Generate_WithSpecifiedSizeQrAndLogo_ShouldGenerateQR()
    {
        // Arrange
        int width = 100;
        int height = 100;
        int logoWidth = 20;
        int logoHeight = 20;
        int margin = 1;

        var qrCodeRequest = new QrCodeRequest(_qrValue!,
            Width: width,
            Height: height,
            Margin: margin,
            LogoPath: _logoPath,
            LogoWidth: logoWidth,
            LogoHeight: logoHeight);
        var qrGeneratorService = new QrGenerator();

        // Act
        var qrCodeResult = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsTrue(qrCodeResult.IsSuccess);
        Assert.IsNotNull(qrCodeResult.Value);
    }

    [Test]
    public void Generate_WithLogoWidthIsLarger_ShouldReturnFailure()
    {
        // Arrange
        int width = 100;
        int height = 100;
        int logoWidth = 200;
        int logoHeight = 50;

        var qrCodeRequest = new QrCodeRequest(_qrValue!,
            width,
            height,
            0,
            _logoPath!,
            logoWidth,
            logoHeight);
        var qrGeneratorService = new QrGenerator();

        // Act
        var result = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.That(result.Error, Is.EqualTo("Logo aspect ratio is too wide or tall. Keep it closer to a square."));
    }

    [Test]
    public void Generate_WithLogoHeightIsLarger_ShouldReturnFailure()
    {
        // Arrange
        int width = 100;
        int height = 100;
        int logoWidth = 50;
        int logoHeight = 200;

        var qrCodeRequest = new QrCodeRequest(_qrValue!,
            width,
            height,
            0,
            _logoPath!,
            logoWidth,
            logoHeight);
        var qrGeneratorService = new QrGenerator();

        // Act
        var result = qrGeneratorService.Generate(qrCodeRequest);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.That(result.Error, Is.EqualTo("Logo aspect ratio is too wide or tall. Keep it closer to a square."));
    }

    [Test]
    public void GenerateAsPng_ShouldWriteIntoMS()
    {
        // Arrange
        var qrCodeRequest = new QrCodeRequest(_qrValue!);
        var qrGeneratorService = new QrGenerator();

        // Act
        Result<MemoryStream> msResult = qrGeneratorService.GenerateAsPng(qrCodeRequest);

        // Assert
        Assert.IsTrue(msResult.IsSuccess);
        Assert.NotNull(msResult.Value);
        Assert.IsTrue(msResult.Value.Length > 0);   // Not empty MemoryStream
    }

    [Test]
    public void SaveAsPng_WithoutFilePath_ShouldThrowException()
    {
        // Arrange
        string? filePath = null;
        var qrCodeRequest = new QrCodeRequest(_qrValue!);
        var qrGeneratorService = new QrGenerator();

        // Act
#pragma warning disable CS8604 // Possible null reference argument.
        var result = qrGeneratorService.SaveAsPng(qrCodeRequest, filePath);
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.That(result.Error, Is.EqualTo("Invalid filePath."));
    }

    [Test]
    public void SaveAsPng_ShouldGenerateQRToFile()
    {
        // Arrange
        string filePath = Path.Combine(_outputDirectoryPath!, $"qr_{DateTime.Now:yyyyMMddHHmm}.png");
        var qrCodeRequest = new QrCodeRequest(_qrValue!);
        var qrGeneratorService = new QrGenerator();

        // Act
        var result = qrGeneratorService.SaveAsPng(qrCodeRequest, filePath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(File.Exists(filePath));
    }

    [Test]
    public void SaveAsPng_WithFilePathNoExtension_ShouldGenerateQRToFile()
    {
        // Arrange
        string filePath = Path.Combine(_outputDirectoryPath!, $"qr_{DateTime.Now:yyyyMMddHHmm}");
        var qrCodeRequest = new QrCodeRequest(_qrValue!);
        var qrGeneratorService = new QrGenerator();

        // Act
        var result = qrGeneratorService.SaveAsPng(qrCodeRequest, filePath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(File.Exists(filePath + ".png"));
    }

    [Test]
    public void SaveAsPng_WithLogo_ShouldGenerateQRToFile()
    {
        // Arrange
        string filePath = Path.Combine(_outputDirectoryPath!, $"qr_{DateTime.Now:yyyyMMddHHmm}.png");
        var qrCodeRequest = new QrCodeRequest(_qrValue!, LogoPath: _logoPath!);
        var qrGeneratorService = new QrGenerator();

        // Act
        var result = qrGeneratorService.SaveAsPng(qrCodeRequest, filePath);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(File.Exists(filePath));
    }

    [Test]
    public void GenerateAsBase64_ShouldGenerateQRToBase64()
    {
        // Arrange
        var qrCodeRequest = new QrCodeRequest(_qrValue!);
        var qrGeneratorService = new QrGenerator();

        Result<MemoryStream> msResult = qrGeneratorService.GenerateAsPng(qrCodeRequest);
        string expectedBase64String = Convert.ToBase64String(msResult.Value.ToArray());

        // Act
        Result<string> base64StringResult = qrGeneratorService.GenerateAsBase64(qrCodeRequest);

        // Assert
        Assert.IsTrue(base64StringResult.IsSuccess);

        string base64String = base64StringResult.Value;
        Assert.IsNotNull(base64String);
        Assert.IsNotEmpty(base64String);
        Assert.IsTrue(base64String.IsValidBase64());
        Assert.AreEqual(expectedBase64String, base64String);
    }

    [Test]
    public void GenerateAsDataUri_ShouldGenerateQRToDataUri()
    {
        // Arrange
        string dataUriPrefix = "data:image/png;base64,";
        var qrCodeRequest = new QrCodeRequest(_qrValue!);
        var qrGeneratorService = new QrGenerator();

        Result<string> base64StringResult = qrGeneratorService.GenerateAsBase64(qrCodeRequest);
        string expectedDataUri = dataUriPrefix + base64StringResult.Value;

        // Act
        Result<string> dataUriResult = qrGeneratorService.GenerateAsDataUri(qrCodeRequest);

        // Assert
        Assert.IsTrue(dataUriResult.IsSuccess);

        string dataUri = dataUriResult.Value;
        Assert.IsNotNull(dataUri);
        Assert.IsNotEmpty(dataUri);
        Assert.IsTrue(dataUri.StartsWith(dataUriPrefix));
        Assert.AreEqual(expectedDataUri, dataUri);
    }
}
