using KYS.Library.Helpers;
using KYS.Library.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace KYS.Library.Tests.ServicesUnitTests
{
    internal class ZipExtractorUnitTest
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
        public void Unzip_WithNonExistentFile_ShouldThrowException()
        {
            // Arrange
            string zipFileName = $"invalid.zip";
            string zipFileDirPath = Path.Combine(_outDirectoryPath, zipFileName);
            string unzipDirPath = Path.Combine(_outDirectoryPath, zipFileName.Replace(".zip", ""));

            // Act
            ZipExtractor unzipService = new();
            var result = unzipService.Unzip(zipFileDirPath, unzipDirPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsFalse(File.Exists(zipFileDirPath));
            Assert.IsFalse(Directory.Exists(unzipDirPath));
            Assert.That(result.Error, Is.EqualTo($"File not found: {zipFileDirPath}"));
        }

        [Test]
        public void Unzip_WithUnencryptedZipFile_ShouldUnzipToDir()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var fileItems = new List<ZipFileItem> { _fileItems[0] };

            string unzipDirPath = Path.Combine(_outDirectoryPath, zipFileName.Replace(".zip", ""));

            #region Generate zip file for unencrypted later
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
            };
            zipService.ZipAndToFile(_outDirectoryPath);
            #endregion

            // Act
            ZipExtractor unzipService = new();
            var result = unzipService.Unzip(Path.Combine(_outDirectoryPath, zipFileName), unzipDirPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(Directory.Exists(unzipDirPath));
            Assert.AreEqual(zipFileName, unzipService.FileName);
            Assert.AreEqual(fileItems.Count, unzipService.FileItems.Count);
        }

        [Test]
        public void Unzip_WithEncryptedZipFile_ShouldUnzipToDir()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var fileItems = new List<ZipFileItem> { _fileItems[0] };
            string password = "abc1234";

            string unzipDirPath = Path.Combine(_outDirectoryPath, zipFileName.Replace(".zip", ""));

            #region Generate zip file for unencrypted later
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            zipService.ZipAndToFile(_outDirectoryPath);
            #endregion

            // Act
            ZipExtractor unzipService = new()
            {
                Password = password
            };
            var result = unzipService.Unzip(Path.Combine(_outDirectoryPath, zipFileName), unzipDirPath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(Directory.Exists(unzipDirPath));
            Assert.AreEqual(zipFileName, unzipService.FileName);
            Assert.AreEqual(fileItems.Count, unzipService.FileItems.Count);
        }

        [Test]
        public void Unzip_WithEncryptedZipFileAndNoPassword_ShouldThrowException()
        {
            // Arrange
            string zipFileName = $"Sample zip_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            var fileItems = new List<ZipFileItem> { _fileItems[0] };
            string password = "abc1234";

            string unzipDirPath = Path.Combine(_outDirectoryPath, zipFileName.Replace(".zip", ""));

            #region Generate zip file for unencrypted later
            ZipCreator zipService = new()
            {
                FileName = zipFileName,
                FileItems = fileItems,
                Password = password
            };
            zipService.ZipAndToFile(_outDirectoryPath);
            #endregion

            // Act
            ZipExtractor unzipService = new();

            var result = unzipService.Unzip(Path.Combine(_outDirectoryPath, zipFileName), unzipDirPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo("Failed to unzip file. Error: No password available for encrypted stream"));
        }
    }
}
