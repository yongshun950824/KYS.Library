using ICSharpCode.SharpZipLib.Zip;
using KYS.Library.Helpers;
using KYS.Library.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace KYS.Library.Tests.ServicesUnitTests
{
    internal class ZipCreatorUnitTest
    {
        private string _resourcePath;
        private string _outDirectoryPath;
        private List<ZipFileItem> _fileItems;

        [SetUp]
        public void SetUp()
        {
            _resourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "logo.png");
            _outDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "OutDir");

            _fileItems = [];
            string[] resourceFileNames = ["logo.png", "sample.txt"];
            foreach (string fileName in resourceFileNames)
            {
                using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_resourcePath);

                _fileItems.Add(new ZipFileItem
                {
                    Name = fileName,
                    Contents = ms.ToArray()
                });
            }
        }

        [Test]
        public void Zip_WithInvalidFileName_ShouldThrowException()
        {
            // Arrange
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipCreator zipService = new()
                {
                    FileName = "",
                    FileItems = _fileItems,
                };
                zipService.Zip();
            });

            // Assert
            Assert.IsInstanceOf<ArgumentException>(ex);
            Assert.That(nameof(ZipCreator.FileName), Is.EqualTo(ex.ParamName));
        }

        [Test]
        public void ZipService_WithFileNameWithoutExtension_ShouldAppendFileExtension()
        {
            // Arrange & Act
            string fileName = "test";
            ZipCreator zipService = new()
            {
                FileName = fileName,
                FileItems = _fileItems,
            };

            // Assert
            Assert.That(fileName + ".zip", Is.EqualTo(zipService.FileName));
        }

        [Test]
        public void Zip_WithEmptyFileItems_ShouldThrowException()
        {
            // Arrange
            ArgumentException expectedEx = new("FileItems must be provided with at least 1 file.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipCreator zipService = new()
                {
                    FileName = "Sample.zip",
                    FileItems = []
                };
                zipService.Zip();
            });

            // Assert
            Assert.IsInstanceOf<ArgumentException>(ex);
            Assert.That(expectedEx.Message, Is.EqualTo(ex.Message));
        }

        [Test]
        public void Zip_WithNullFileItems_ShouldThrowException()
        {
            // Arrange
            ArgumentException expectedEx = new("FileItems must be provided with at least 1 file.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipCreator zipService = new()
                {
                    FileName = "Sample.zip",
                    FileItems = null
                };
                zipService.Zip();
            });

            // Assert
            Assert.IsInstanceOf<ArgumentException>(ex);
            Assert.That(expectedEx.Message, Is.EqualTo(ex.Message));
        }

        [Test]
        public void Zip_WithInvalidPassword_ShouldThrowException()
        {
            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipCreator zipService = new()
                {
                    FileName = "Sample.zip",
                    FileItems = _fileItems,
                    Password = string.Empty
                };
                zipService.Zip();
            });

            // Assert
            Assert.IsInstanceOf<ArgumentException>(ex);
            Assert.That(nameof(ZipCreator.Password), Is.EqualTo(ex.ParamName));
        }

        [Test]
        public void Constructor_WithNoArgument_ShouldCreateInstance()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            string password = "abc1234";
            var fileItems = new List<ZipFileItem> { _fileItems[0] };

            // Act
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };

            // Assert
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
            Assert.AreEqual(password, zipService.Password);
        }

        [Test]
        public void Constructor_WithSecondConstructor_ShouldCreateInstance()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var fileItems = new List<ZipFileItem> { _fileItems[0] };

            // Act
            ZipCreator zipService = new(zipFileName, fileItems);

            // Assert
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
        }

        [Test]
        public void Constructor_WithThirdConstructor_ShouldCreateInstance()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            string password = "abc1234";
            var fileItems = new List<ZipFileItem> { _fileItems[0] };

            // Act
            ZipCreator zipService = new(zipFileName, fileItems, password);

            // Assert
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
            Assert.AreEqual(password, zipService.Password);
        }

        [Test]
        public void ZipAndToFile_WithSingleFile_ShouldZipToFile()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            string outputFilePath = Path.Combine(_outDirectoryPath, zipFileName);
            var fileItems = new List<ZipFileItem> { _fileItems[0] };

            // Act
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
            };
            var result = zipService.ZipAndToFile(_outDirectoryPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
            Assert.IsTrue(File.Exists(outputFilePath));
        }

        [Test]
        public void ZipAndToFile_WithSingleFileAndPassword_ShouldZipToFile()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            string outputFilePath = Path.Combine(_outDirectoryPath, zipFileName);
            string password = "abc1234";
            var fileItems = new List<ZipFileItem> { _fileItems[0] };

            // Act
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            var result = zipService.ZipAndToFile(_outDirectoryPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(password, zipService.Password);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
            Assert.IsTrue(File.Exists(outputFilePath));
        }

        [Test]
        public void ZipAndToFile_WithMultipleFiles_ShouldZipToFile()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            string outputFilePath = Path.Combine(_outDirectoryPath, zipFileName);
            var fileItems = _fileItems;

            // Act
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
            };
            var result = zipService.ZipAndToFile(_outDirectoryPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
            Assert.IsTrue(File.Exists(outputFilePath));
        }

        [Test]
        public void ZipAndToFile_WithMultipleFilesAndPassword_ShouldZipToFile()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            string outputFilePath = Path.Combine(_outDirectoryPath, zipFileName);
            string password = "abc1234";
            var fileItems = _fileItems;

            // Act
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            var result = zipService.ZipAndToFile(_outDirectoryPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(password, zipService.Password);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
            Assert.IsTrue(File.Exists(outputFilePath));
        }

        [Test]
        public void ZipAndToFile_WithInvalidDestinationDirPath_ShouldReturnResultFailure()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            string password = "abc1234";
            var fileItems = _fileItems;

            // Act
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            var result = zipService.ZipAndToFile(String.Empty);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("destDirPath must be provided.", result.Error);
        }
    }
}
