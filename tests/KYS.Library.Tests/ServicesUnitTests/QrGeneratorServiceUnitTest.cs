using CSharpFunctionalExtensions;
using KYS.Library.Extensions;
using KYS.Library.Services;
using NUnit.Framework;
using System;
using System.IO;

namespace KYS.Library.Tests.ServicesUnitTests
{
    public class QrGeneratorServiceUnitTest
    {
        private string _logoPath;
        private string _outputDirectoryPath;

        private string _qrValue;
        private int _defaultWidth;
        private int _defaultHeight;
        private int _defaultLogoWidth;
        private int _defaultLogoHeight;
        private int _defaultMargin;

        [SetUp]
        public void SetUp()
        {
            _logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "logo.png");
            _outputDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "OutDir");

            Directory.CreateDirectory(_outputDirectoryPath);

            _qrValue = "Sample value";
            _defaultWidth = 200;
            _defaultHeight = 200;
            _defaultLogoWidth = _defaultWidth / 5;
            _defaultLogoHeight = _defaultHeight / 5;
            _defaultMargin = 0;
        }

        [Test]
        public void Create_WithNull_ShouldReturnFailure()
        {
            // Act
            var result = QrGenerator.Create(null);

            // Assert
            Assert.That(result.Error, Is.EqualTo("Invalid value."));
        }

        [Test]
        public void Create_WithInvalidWidth_ShouldReturnFailure()
        {
            // Act
            var result = QrGenerator.Create(_qrValue, -10, _defaultHeight, _defaultMargin);

            // Assert
            Assert.That(result.Error, Is.EqualTo("Invalid width."));
        }

        [Test]
        public void Create_WithInvalidHeight_ShouldReturnFailure()
        {
            // Act
            var result = QrGenerator.Create(_qrValue, _defaultWidth, -10, _defaultMargin);

            // Assert
            Assert.That(result.Error, Is.EqualTo("Invalid height."));
        }

        [Test]
        public void Create_WithInvalidMargin_ShouldReturnFailure()
        {
            // Act
            var result = QrGenerator.Create(_qrValue, _defaultWidth, _defaultHeight, -10);

            // Assert
            Assert.That(result.Error, Is.EqualTo("Invalid margin."));
        }

        [Test]
        public void Draw_WithDefaultSize_ShouldGenerateQR()
        {
            // Act
            var qrGeneratorService = QrGenerator.Create(_qrValue).Value;
            qrGeneratorService.Draw();

            // Assert
            Assert.IsNotNull(qrGeneratorService.QRCode);
            Assert.AreEqual(_qrValue, qrGeneratorService.Value);
            Assert.AreEqual(_defaultWidth, qrGeneratorService.Width);
            Assert.AreEqual(_defaultHeight, qrGeneratorService.Height);
            Assert.AreEqual(null, qrGeneratorService.LogoPath);
            Assert.AreEqual(null, qrGeneratorService.LogoWidth);
            Assert.AreEqual(null, qrGeneratorService.LogoHeight);
            Assert.AreEqual(_defaultMargin, qrGeneratorService.Margin);
        }

        [Test]
        public void Draw_WithSpecifiedSize_ShouldGenerateQR()
        {
            // Arrange
            int width = 100;
            int height = 100;
            int margin = 2;

            // Act
            var qrGeneratorService = QrGenerator.Create(_qrValue, width, height, margin).Value;
            qrGeneratorService.Draw();

            // Assert
            Assert.IsNotNull(qrGeneratorService.QRCode);
            Assert.AreEqual(_qrValue, qrGeneratorService.Value);
            Assert.AreEqual(width, qrGeneratorService.Width);
            Assert.AreEqual(height, qrGeneratorService.Height);
            Assert.AreEqual(null, qrGeneratorService.LogoPath);
            Assert.AreEqual(null, qrGeneratorService.LogoWidth);
            Assert.AreEqual(null, qrGeneratorService.LogoHeight);
            Assert.AreEqual(margin, qrGeneratorService.Margin);
        }

        [Test]
        public void Draw_WithDefaultSizeLogo_ShouldGenerateQR()
        {
            // Act
            var qrGeneratorService = QrGenerator.CreateWithLogo(_qrValue, _logoPath).Value;
            qrGeneratorService.Draw();

            // Assert
            Assert.IsNotNull(qrGeneratorService.QRCode);
            Assert.AreEqual(_qrValue, qrGeneratorService.Value);
            Assert.AreEqual(_defaultWidth, qrGeneratorService.Width);
            Assert.AreEqual(_defaultHeight, qrGeneratorService.Height);
            Assert.AreEqual(_logoPath, qrGeneratorService.LogoPath);
            Assert.AreEqual(_defaultLogoWidth, qrGeneratorService.LogoWidth);
            Assert.AreEqual(_defaultLogoHeight, qrGeneratorService.LogoHeight);
            Assert.AreEqual(_defaultMargin, qrGeneratorService.Margin);
        }

        [Test]
        public void Draw_WithSpecifiedSizeLogo_ShouldGenerateQR()
        {
            // Arrange
            int logoWidth = 25;
            int logoHeight = 25;
            int margin = 1;

            // Act
            var qrGeneratorService = QrGenerator.CreateWithLogo(_qrValue, _logoPath,
                _defaultWidth, _defaultHeight, margin, logoWidth, logoHeight).Value;
            qrGeneratorService.Draw();

            // Assert
            Assert.IsNotNull(qrGeneratorService.QRCode);
            Assert.AreEqual(_qrValue, qrGeneratorService.Value);
            Assert.AreEqual(_defaultWidth, qrGeneratorService.Width);
            Assert.AreEqual(_defaultHeight, qrGeneratorService.Height);
            Assert.AreEqual(_logoPath, qrGeneratorService.LogoPath);
            Assert.AreEqual(logoWidth, qrGeneratorService.LogoWidth);
            Assert.AreEqual(logoHeight, qrGeneratorService.LogoHeight);
            Assert.AreEqual(margin, qrGeneratorService.Margin);
        }

        [Test]
        public void Draw_WithSpecifiedSizeQRandLogo_ShouldGenerateQR()
        {
            // Arrange
            int width = 100;
            int height = 100;
            int logoWidth = 25;
            int logoHeight = 25;
            int margin = 1;

            // Act
            var qrGeneratorService = QrGenerator.CreateWithLogo(_qrValue, _logoPath, width, height, margin, logoWidth, logoHeight).Value;
            qrGeneratorService.Draw();

            // Assert
            Assert.IsNotNull(qrGeneratorService.QRCode);
            Assert.AreEqual(_qrValue, qrGeneratorService.Value);
            Assert.AreEqual(width, qrGeneratorService.Width);
            Assert.AreEqual(height, qrGeneratorService.Height);
            Assert.AreEqual(_logoPath, qrGeneratorService.LogoPath);
            Assert.AreEqual(logoWidth, qrGeneratorService.LogoWidth);
            Assert.AreEqual(logoHeight, qrGeneratorService.LogoHeight);
            Assert.AreEqual(margin, qrGeneratorService.Margin);
        }

        [Test]
        public void CreateWithLogo_WithNullLogoPath_ShouldReturnFailure()
        {
            // Act
            var result = QrGenerator.CreateWithLogo(_qrValue, logoPath: null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo("Invalid logoPath."));
        }

        [Test]
        public void CreateWithLogo_WithInvalidLogoPath_ShouldReturnFailure()
        {
            // Act
            var result = QrGenerator.CreateWithLogo(_qrValue,
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "dummy-image.png"));

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo("File not found: logoPath."));
        }

        [Test]
        public void CreateWithLogo_WithLogoWidthIsLarger_ShouldReturnFailure()
        {
            // Arrange
            int width = 100;
            int height = 100;
            int logoWidth = 200;
            int logoHeight = 50;

            // Act
            var result = QrGenerator.CreateWithLogo(_qrValue, _logoPath,
                width, height, _defaultMargin, logoWidth, logoHeight);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo("Provided logo width must be smaller than QR code width."));
        }

        [Test]
        public void CreateWithLogo_WithLogoHeightIsLarger_ShouldReturnFailure()
        {
            // Arrange
            int width = 100;
            int height = 100;
            int logoWidth = 50;
            int logoHeight = 200;

            // Act
            var result = QrGenerator.CreateWithLogo(_qrValue, _logoPath,
                width, height, _defaultMargin, logoWidth, logoHeight);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo("Provided logo height must be smaller than QR code height."));
        }

        [Test]
        public void DrawAndToMemoryStream_ShouldWriteIntoMS()
        {
            // Arrange
            QrGenerator qrGeneratorService = QrGenerator.Create(_qrValue).Value;

            // Act
            Result<MemoryStream> msResult = qrGeneratorService.DrawAndToMemoryStream();

            // Assert
            Assert.IsTrue(msResult.IsSuccess);
            Assert.NotNull(msResult.Value);
            Assert.IsTrue(msResult.Value.Length > 0);   // Not empty MemoryStream
        }

        [Test]
        public void DrawAndToFile_WithoutFilePath_ShouldThrowException()
        {
            // Arrange
            string filePath = null;
            QrGenerator qrGeneratorService = QrGenerator.Create(_qrValue).Value;

            // Act
            var result = qrGeneratorService.DrawAndToFile(filePath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo("Invalid filePath."));
        }

        [Test]
        public void DrawAndToFile_ShouldGenerateQRToFile()
        {
            // Arrange
            string filePath = Path.Combine(_outputDirectoryPath, $"qr_{DateTime.Now:yyyyMMddHHmm}.png");
            QrGenerator qrGeneratorService = QrGenerator.Create(_qrValue).Value;

            // Act
            var result = qrGeneratorService.DrawAndToFile(filePath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public void DrawAndToFile_WithFilePathNoExtension_ShouldGenerateQRToFile()
        {
            // Arrange
            string filePath = Path.Combine(_outputDirectoryPath, $"qr_{DateTime.Now:yyyyMMddHHmm}");
            QrGenerator qrGeneratorService = QrGenerator.Create(_qrValue).Value;

            // Act
            var result = qrGeneratorService.DrawAndToFile(filePath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(File.Exists(filePath + ".png"));
        }

        [Test]
        public void DrawAndToFile_WithLogo_ShouldGenerateQRToFile()
        {
            // Arrange
            string filePath = Path.Combine(_outputDirectoryPath, $"qr_{DateTime.Now:yyyyMMddHHmm}.png");
            QrGenerator qrGeneratorService = QrGenerator.CreateWithLogo(_qrValue, _logoPath).Value;

            // Act
            var result = qrGeneratorService.DrawAndToFile(filePath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public void DrawAndToBase64_ShouldGenerateQRToBase64()
        {
            // Arrange
            QrGenerator qrGeneratorService = QrGenerator.Create(_qrValue).Value;

            Result<MemoryStream> msResult = qrGeneratorService.DrawAndToMemoryStream();
            string expectedBase64String = Convert.ToBase64String(msResult.Value.ToArray());

            // Act
            Result<string> base64StringResult = qrGeneratorService.DrawAndToBase64();

            // Assert
            Assert.IsTrue(base64StringResult.IsSuccess);

            string base64String = base64StringResult.Value;
            Assert.IsNotNull(base64String);
            Assert.IsNotEmpty(base64String);
            Assert.IsTrue(base64String.IsValidBase64());
            Assert.AreEqual(expectedBase64String, base64String);
        }

        [Test]
        public void DrawAndToDataUri_ShouldGenerateQRToDataUri()
        {
            // Arrange
            string dataUriPrefix = "data:image/png;base64, ";
            QrGenerator qrGeneratorService = QrGenerator.Create(_qrValue).Value;

            Result<string> base64StringResult = qrGeneratorService.DrawAndToBase64();
            string expectedDataUri = dataUriPrefix + base64StringResult.Value;

            // Act
            Result<string> dataUriResult = qrGeneratorService.DrawAndToDataUri();

            // Assert
            Assert.IsTrue(dataUriResult.IsSuccess);

            string dataUri = dataUriResult.Value;
            Assert.IsNotNull(dataUri);
            Assert.IsNotEmpty(dataUri);
            Assert.IsTrue(dataUri.StartsWith(dataUriPrefix));
            Assert.AreEqual(expectedDataUri, dataUri);
        }
    }
}
