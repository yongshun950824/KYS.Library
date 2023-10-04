﻿using KYS.Library.Extensions;
using KYS.Library.Services;
using NUnit.Framework;
using System;
using System.IO;

namespace KYS.TestProject
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
        public void GenerateQRWithoutValueAndFailed()
        {
            // Arrange
            ArgumentNullException expectedEx = new ArgumentNullException("value");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                QrGeneratorService qrGeneratorService = new QrGeneratorService(null);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void GenerateQRWithInvalidWidthAndFailed()
        {
            // Arrange
            ArgumentException expectedEx = new ArgumentException("Invalid width.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, -10, _defaultHeight, _defaultMargin);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void GenerateQRWithInvalidHeightAndFailed()
        {
            // Arrange
            ArgumentException expectedEx = new ArgumentException("Invalid height.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, _defaultWidth, -10, _defaultMargin);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void GenerateQRWithInvalidMarginAndFailed()
        {
            // Arrange
            ArgumentException expectedEx = new ArgumentException("Invalid margin.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, _defaultWidth, _defaultHeight, -10);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void GenerateQRWithDefaultSize()
        {
            // Act
            QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue);
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
        public void GenerateQRWithSpecifiedSize()
        {
            // Arrange
            int width = 100;
            int height = 100;
            int margin = 2;

            // Act
            QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, width, height, margin);
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
        public void GenerateQRWithDefaultSizeLogo()
        {
            // Act
            QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, _logoPath);
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
        public void GenerateQRWithSpecifiedSizeLogo()
        {
            // Arrange
            int logoWidth = 25;
            int logoHeight = 25;
            int margin = 1;

            // Act
            QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, _logoPath,
                _defaultWidth, _defaultHeight, margin, logoWidth, logoHeight);
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
        public void GenerateQRWithSpecifiedSizeQRandLogo()
        {
            // Arrange
            int width = 100;
            int height = 100;
            int logoWidth = 25;
            int logoHeight = 25;
            int margin = 1;

            // Act
            QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, _logoPath,
                width, height, margin, logoWidth, logoHeight);
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
        public void GenerateQRWithLogoButNotProvideLogoPathAndFailed()
        {
            // Arrange
            ArgumentNullException expectedEx = new ArgumentNullException("logoPath");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, logoPath: null);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void GenerateQRWithLogoButProvideInvalidLogoPath()
        {
            // Arrange
            FileNotFoundException expectedEx = new FileNotFoundException("logoPath");

            // Act
            var ex = Assert.Throws<FileNotFoundException>(() =>
            {
                QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue,
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources", "dummy-image.png"));
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void GenerateQRWithLogoWidthIsLargerAndFailed()
        {
            // Arrange
            int width = 100;
            int height = 100;
            int logoWidth = 200;
            int logoHeight = 50;

            ArgumentException expectedEx = new ArgumentException("Provided logo width must be smaller than QR code width.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, _logoPath, 
                    width, height, _defaultMargin, logoWidth, logoHeight);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void GenerateQRWithLogoHeightIsLargerAndFailed()
        {
            // Arrange
            int width = 100;
            int height = 100;
            int logoWidth = 50;
            int logoHeight = 200;

            ArgumentException expectedEx = new ArgumentException("Provided logo height must be smaller than QR code height.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                QrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, _logoPath, 
                    width, height, _defaultMargin, logoWidth, logoHeight);
            });

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void GenerateQRAndToMemoryStream()
        {
            // Arrange
            IQrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue);

            // Act
            MemoryStream ms = qrGeneratorService.DrawAndToMemoryStream();

            // Assert
            Assert.NotNull(ms);
            Assert.IsTrue(ms.Length > 0);   // Not empty MemoryStream
        }

        [Test]
        public void GenerateQRAndWriteToFile()
        {
            // Arrange
            string filePath = Path.Combine(_outputDirectoryPath, $"qr_{DateTime.Now:yyyyMMddHHmm}.png");
            IQrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue);

            // Act
            qrGeneratorService.DrawAndToFile(filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public void GenerateQRWithLogoAndWriteToFile()
        {
            // Arrange
            string filePath = Path.Combine(_outputDirectoryPath, $"qr_{DateTime.Now:yyyyMMddHHmm}.png");
            IQrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue, _logoPath);

            // Act
            qrGeneratorService.DrawAndToFile(filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public void GenerateQRAndToBase64()
        {
            // Arrange
            IQrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue);
            using MemoryStream ms = qrGeneratorService.DrawAndToMemoryStream();
            string expectedBase64String = Convert.ToBase64String(ms.ToArray());

            // Act
            string base64String = qrGeneratorService.DrawAndToBase64();

            // Assert
            Assert.IsNotNull(base64String);
            Assert.IsNotEmpty(base64String);
            Assert.IsTrue(base64String.IsValidBase64());
            Assert.AreEqual(expectedBase64String, base64String);
        }

        [Test]
        public void GenerateQRAndToDataUri()
        {
            // Arrange
            string dataUriPrefix = "data:image/png;base64, ";
            IQrGeneratorService qrGeneratorService = new QrGeneratorService(_qrValue);
            string base64String = qrGeneratorService.DrawAndToBase64();
            string expectedDataUri = dataUriPrefix + base64String;

            // Act
            string dataUri = qrGeneratorService.DrawAndToDataUri();

            // Assert
            Assert.IsNotNull(dataUri);
            Assert.IsNotEmpty(dataUri);
            Assert.IsTrue(dataUri.StartsWith(dataUriPrefix));
            Assert.AreEqual(expectedDataUri, dataUri);
        }
    }
}
