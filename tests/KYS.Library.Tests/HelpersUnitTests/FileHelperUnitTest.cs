using System;
using System.IO;
using KYS.Library.Helpers;
using NUnit.Framework;

namespace KYS.Library.Tests.HelpersUnitTests
{
    internal class FileHelperUnitTest
    {
        private readonly string COULD_NOT_FIND_PATH_ERROR_MESSAGE = "Could not find a part of the path '{0}'.";

        [Test]
        public void WriteFile_WithValidMemoryStream_ShouldCreateFileWithCorrectContent()
        {
            // Arrange
            byte[] data = [1, 2, 3, 4, 5];
            using MemoryStream ms = new(data);

            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);

            // Act
            var result = FileHelper.WriteFile(ms, tempFilePath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.That(File.Exists(tempFilePath), Is.True, "File should be created.");

            byte[] fileBytes = File.ReadAllBytes(tempFilePath);
            Assert.That(fileBytes, Is.EqualTo(data), "File content should match stream data.");

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteFile_WithNullStream_ShouldReturnResultFailure()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();

            // Act
            var result = FileHelper.WriteFile((MemoryStream)null, tempFilePath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("ms cannot be null.", result.Error);

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteFile_WithMemoryStreamAndInvalidPath_ShouldReturnResultFailure()
        {
            // Arrange
            byte[] data = [1, 2, 3];
            using MemoryStream ms = new(data);
            string invalidPath = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => "?:\\invalid\\path.txt",  // Invalid on Windows
                _ => "/invalid_dir/\\invalid_path.txt"          // Invalid on Linux/macOS
            };

            // Act
            var result = FileHelper.WriteFile(ms, invalidPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(String.Format(COULD_NOT_FIND_PATH_ERROR_MESSAGE, invalidPath), result.Error);
        }

        [Test]
        public void WriteFile_WithMemoryStreamAndEmptyFileName_ShouldReturnResultFailure()
        {
            // Arrange
            byte[] data = [1, 2, 3, 4, 5];
            using MemoryStream ms = new(data);

            // Act
            var result = FileHelper.WriteFile(ms, String.Empty);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(String.Format(DomainErrors.Required("fileName")), result.Error);
        }

        [Test]
        public void WriteFile_WithValidBytesArray_ShouldCreateFileWithCorrectContent()
        {
            // Arrange
            byte[] data = [1, 2, 3, 4, 5];

            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);

            // Act
            var result = FileHelper.WriteFile(data, tempFilePath);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.That(File.Exists(tempFilePath), Is.True, "File should be created.");

            byte[] fileBytes = File.ReadAllBytes(tempFilePath);
            Assert.That(fileBytes, Is.EqualTo(data), "File content should match stream data.");

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteFile_WithNullBytesArray_ShouldReturnResultFailure()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();

            // Act 
            var result = FileHelper.WriteFile((byte[])null, tempFilePath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("bytes cannot be null.", result.Error);

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteFile_WithBytesArrayAndInvalidPath_ShouldReturnResultFailure()
        {
            // Arrange
            byte[] data = [1, 2, 3];
            string invalidPath = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => "?:\\invalid\\path.txt",  // Invalid on Windows
                _ => "/invalid_dir/\\invalid_path.txt"          // Invalid on Linux/macOS
            };

            // Act
            var result = FileHelper.WriteFile(data, invalidPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(String.Format(COULD_NOT_FIND_PATH_ERROR_MESSAGE, invalidPath), result.Error);
        }

        [Test]
        public void WriteFile_WithBytesArrayAndEmptyFileName_ShouldReturnResultFailure()
        {
            // Arrange
            byte[] data = [1, 2, 3, 4, 5];

            // Act
            var result = FileHelper.WriteFile(data, String.Empty);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(String.Format(DomainErrors.Required("fileName")), result.Error);
        }

        [Test]
        public void LoadFileToMemoryStream_WithValidFilePath_ShouldReturnMemoryStream()
        {
            // Arrange
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "sample.txt");

            // Act
            var result = FileHelper.LoadFileToMemoryStream(filePath);

            // Assert
            Assert.IsTrue(result.IsSuccess);

            var ms = result.Value;
            Assert.IsNotNull(ms);
            Assert.IsTrue(ms.CanRead);
            Assert.AreEqual(0, ms.Position, "Stream position should be reset to 0");
        }

        [Test]
        public void LoadFileToMemoryStream_WithEmptyFilePath_ShouldReturnResultFailure()
        {
            // Act 
            var result = FileHelper.LoadFileToMemoryStream(null);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(DomainErrors.Required("filePath"), result.Error);
        }

        [Test]
        public void LoadFileToMemoryStream_WithInvalidFilePath_ShouldReturnResultFailure()
        {
            // Arrange
            string invalidPath = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => "?:\\invalid\\path.txt",  // Invalid on Windows
                _ => "/invalid_dir/\\invalid_path.txt"          // Invalid on Linux/macOS
            };

            // Act 
            var result = FileHelper.LoadFileToMemoryStream(invalidPath);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"File not found: {invalidPath}.", result.Error);
        }
    }
}
