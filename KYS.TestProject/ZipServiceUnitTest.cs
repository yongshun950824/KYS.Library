using ICSharpCode.SharpZipLib.Zip;
using KYS.Library.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace KYS.TestProject
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
                using FileStream fs = new FileStream(_resourcePath, FileMode.Open, FileAccess.Read);
                fs.Position = 0;

                using MemoryStream ms = new MemoryStream();
                fs.CopyTo(ms);

                _fileItems.Add(new ZipService.ZipFileItem
                {
                    Name = fileName,
                    Contents = ms.ToArray()
                });
            }
        }

        [Test]
        public void ZipWithInvalidFileName()
        {
            ArgumentException expectedEx = new ArgumentException("FileName must be provided.");

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipService zipService = new ZipService
                {
                    FileName = "",
                    FileItems = _fileItems,
                };
                zipService.Zip();
            });

            Assert.That(expectedEx.Message, Is.EqualTo(ex.Message));
        }

        [Test]
        public void ZipWithEmptyFileItemsAndFail()
        {
            ArgumentException expectedEx = new ArgumentException("FileItems must be provided with at least 1 file.");

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipService zipService = new ZipService
                {
                    FileName = "Sample.zip",
                    FileItems = new List<ZipService.ZipFileItem>(),
                };
                zipService.Zip();
            });

            Assert.That(expectedEx.Message, Is.EqualTo(ex.Message));
        }

        [Test]
        public void ZipWithNullFileItemsAndFail()
        {
            ArgumentException expectedEx = new ArgumentException("FileItems must be provided with at least 1 file.");

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipService zipService = new ZipService
                {
                    FileName = "Sample.zip",
                    FileItems = null,
                };
                zipService.Zip();
            });

            Assert.That(expectedEx.Message, Is.EqualTo(ex.Message));
        }

        [Test]
        public void ZipWithInvalidPasswordAndFail()
        {
            ArgumentException expectedEx = new ArgumentException("Password must be provided.");

            var ex = Assert.Throws<ArgumentException>(() =>
            {
                ZipService zipService = new ZipService
                {
                    FileName = "Sample.zip",
                    FileItems = _fileItems,
                    Password = String.Empty
                };
                zipService.Zip();
            });

            Assert.That(expectedEx.Message, Is.EqualTo(ex.Message));
        }

        [Test]
        public void CreateZipWriterServiceInstanceWithFirstConstructor()
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
        public void CreateZipWriterServiceInstanceWithSecondConstructor()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var fileItems = new List<ZipService.ZipFileItem> { _fileItems[0] };

            // Act
            ZipService zipService = new ZipService
            {
                FileName = zipFileName,
                FileItems = fileItems
            };

            // Assert
            Assert.AreEqual(zipFileName, zipService.FileName);
            Assert.AreEqual(fileItems.Count, zipService.FileItems.Count);
        }

        [Test]
        public void CreateZipWriterServiceInstanceWithThirdConstructor()
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
        public void ZipSingleFileAndWriteFile()
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
        public void ZipSingleFileWithPasswordAndWriteFile()
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
        public void ZipMultipleFilesAndWriteFile()
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
        public void ZipMultipleFilesWithPasswordAndWriteFile()
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
        public void UnzipUnencryptedZipFile()
        {
            // Arrange
            string fileName = "Sample zip_20231009_1622.zip";
            string unzipDirPath = Path.Combine(_outDirectoryPath, fileName.Replace(".zip", ""));
            string password = "abc1234";

            // Act
            ZipService zipService = new ZipService
            {
                Password = password
            };
            zipService.Unzip(Path.Combine(_outDirectoryPath, fileName), unzipDirPath);

            // Assert
            Assert.IsTrue(Directory.Exists(unzipDirPath));
        }

        [Test]
        public void UnzipEncryptedFile()
        {
            // Arrange
            string fileName = "Sample zip_20231009_1622.zip";
            string unzipDirPath = Path.Combine(_outDirectoryPath, fileName.Replace(".zip", ""));
            string password = "abc1234";

            // Act
            ZipService zipService = new ZipService
            {
                Password = password
            };
            zipService.Unzip(Path.Combine(_outDirectoryPath, fileName), unzipDirPath);

            // Assert
            Assert.IsTrue(Directory.Exists(unzipDirPath));
        }

        [Test]
        public void UnzipEncryptedFileWithoutPasswordAndFail()
        {
            // Arrange
            string fileName = "Sample zip_20231009_1622.zip";
            var expectedEx = new ZipException("No password available for encrypted stream");

            // Act
            var ex = Assert.Throws<ZipException>(() =>
            {
                ZipService zipService = new ZipService();

                zipService.Unzip(Path.Combine(_outDirectoryPath, fileName),
                    Path.Combine(_outDirectoryPath, fileName.Replace(".zip", "")));
            });

            // Assert
            Assert.That(expectedEx.Message, Is.EqualTo(ex.Message));
        }
    }
}
