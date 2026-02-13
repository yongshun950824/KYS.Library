using ICSharpCode.SharpZipLib.Zip;
using KYS.Library.Helpers;
using KYS.Library.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace KYS.Library.Tests.ServicesUnitTests
{
    internal class ZipServiceUnitTest
    {
        private string _resourcePath;
        private string _outDirectoryPath;
        private List<ZipService.ZipFileItem> _fileItems;

        [SetUp]
        public void SetUp()
        {
            _resourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "logo.png");
            _outDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "OutDir");

            _fileItems = new List<ZipService.ZipFileItem>();
            string[] resourceFileNames = new string[] { "logo.png", "sample.txt" };
            foreach (string fileName in resourceFileNames)
            {
                using MemoryStream ms = FileHelper.LoadFileToMemoryStream(_resourcePath);

                _fileItems.Add(new ZipService.ZipFileItem
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
                ZipService zipService = new ZipService
                {
                    FileName = "",
                    FileItems = _fileItems,
                };
                zipService.Zip();
            });

            // Assert
            Assert.IsInstanceOf<ArgumentException>(ex);
            Assert.That(nameof(ZipService.FileName), Is.EqualTo(ex.ParamName));
        }

        [Test]
        public void ZipService_WithFileNameWithoutExtension_ShouldAppendFileExtension()
        {
            // Arrange & Act
            string fileName = "test";
            ZipService zipService = new ZipService
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
            ArgumentException expectedEx = new ArgumentException("FileItems must be provided with at least 1 file.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipService zipService = new ZipService
                {
                    FileName = "Sample.zip",
                    FileItems = new List<ZipService.ZipFileItem>(),
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
            ArgumentException expectedEx = new ArgumentException("FileItems must be provided with at least 1 file.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipService zipService = new ZipService
                {
                    FileName = "Sample.zip",
                    FileItems = null,
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
                ZipService zipService = new ZipService
                {
                    FileName = "Sample.zip",
                    FileItems = _fileItems,
                    Password = string.Empty
                };
                zipService.Zip();
            });

            // Assert
            Assert.IsInstanceOf<ArgumentException>(ex);
            Assert.That(nameof(ZipService.Password), Is.EqualTo(ex.ParamName));
        }

        [Test]
        public void Constructor_WithNoArgument_ShouldCreateInstance()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            string password = "abc1234";
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };

            // Act
            ZipService zipService = new ZipService
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
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };

            // Act
            ZipService zipService = new ZipService(zipFileName, fileItems);

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
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };

            // Act
            ZipService zipService = new ZipService(zipFileName, fileItems, password);

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
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };

            // Act
            ZipService zipService = new ZipService
            {
                FileName = zipFileName,
                FileItems = fileItems,
            };
            zipService.ZipAndToFile(_outDirectoryPath);

            // Assert
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
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };

            // Act
            ZipService zipService = new ZipService
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            zipService.ZipAndToFile(_outDirectoryPath);

            // Assert
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
            ZipService zipService = new ZipService
            {
                FileName = zipFileName,
                FileItems = fileItems,
            };
            zipService.ZipAndToFile(_outDirectoryPath);

            // Assert
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
            ZipService zipService = new ZipService
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            zipService.ZipAndToFile(_outDirectoryPath);

            // Assert
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(password, zipService.Password);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
            Assert.IsTrue(File.Exists(outputFilePath));
        }

        [Test]
        public void Unzip_WithNonExistentFile_ShouldThrowException()
        {
            // Arrange
            string zipFileName = $"invalid.zip";
            string zipFileDirPath = Path.Combine(_outDirectoryPath, zipFileName);
            string unzipDirPath = Path.Combine(_outDirectoryPath, zipFileName.Replace(".zip", ""));

            // Act
            var ex = Assert.Throws<FileNotFoundException>(() =>
            {
                ZipService unzipService = new ZipService();
                unzipService.Unzip(zipFileDirPath, unzipDirPath);
            });

            // Assert
            Assert.IsFalse(File.Exists(zipFileDirPath));
            Assert.IsFalse(Directory.Exists(unzipDirPath));
            Assert.IsNotNull(ex);
            Assert.IsInstanceOf<FileNotFoundException>(ex);
        }

        [Test]
        public void Unzip_WithUnencryptedZipFile_ShouldUnzipToDir()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };

            string unzipDirPath = Path.Combine(_outDirectoryPath, zipFileName.Replace(".zip", ""));

            #region Generate zip file for unencrypted later
            ZipService zipService = new ZipService
            {
                FileName = zipFileName,
                FileItems = fileItems,
            };
            zipService.ZipAndToFile(_outDirectoryPath);
            #endregion

            // Act
            ZipService unzipService = new ZipService();
            unzipService.Unzip(Path.Combine(_outDirectoryPath, zipFileName), unzipDirPath);

            // Assert
            Assert.IsTrue(Directory.Exists(unzipDirPath));
            Assert.AreEqual(zipFileName, unzipService.FileName);
            Assert.AreEqual(fileItems.Count, unzipService.FileItems.Count);
        }

        [Test]
        public void Unzip_WithEncryptedZipFile_ShouldUnzipToDir()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };
            string password = "abc1234";

            string unzipDirPath = Path.Combine(_outDirectoryPath, zipFileName.Replace(".zip", ""));

            #region Generate zip file for unencrypted later
            ZipService zipService = new ZipService
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            zipService.ZipAndToFile(_outDirectoryPath);
            #endregion

            // Act
            ZipService unzipService = new ZipService
            {
                Password = password
            };
            unzipService.Unzip(Path.Combine(_outDirectoryPath, zipFileName), unzipDirPath);

            // Assert
            Assert.IsTrue(Directory.Exists(unzipDirPath));
            Assert.AreEqual(zipFileName, unzipService.FileName);
            Assert.AreEqual(fileItems.Count, unzipService.FileItems.Count);
        }

        [Test]
        public void Unzip_WithEncryptedZipFileAndNoPassword_ShouldThrowException()
        {
            // Arrange
            var expectedEx = new ZipException("No password available for encrypted stream");

            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };
            string password = "abc1234";

            string unzipDirPath = Path.Combine(_outDirectoryPath, zipFileName.Replace(".zip", ""));

            #region Generate zip file for unencrypted later
            ZipService zipService = new ZipService
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            zipService.ZipAndToFile(_outDirectoryPath);
            #endregion

            // Act
            var ex = Assert.Throws<ZipException>(() =>
            {
                ZipService zipService = new ZipService();

                zipService.Unzip(Path.Combine(_outDirectoryPath, zipFileName), unzipDirPath);
            });

            // Assert
            Assert.IsInstanceOf<ZipException>(ex);
            Assert.That(expectedEx.Message, Is.EqualTo(ex.Message));
        }
    }
}
