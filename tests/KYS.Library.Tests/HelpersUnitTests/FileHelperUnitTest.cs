using System;
using System.IO;
using KYS.Library.Helpers;
using NUnit.Framework;

namespace KYS.Library.Tests.HelpersUnitTests
{
    internal class FileHelperUnitTest
    {
        [Test]
        public void WriteFile_WithValidMemoryStream_ShouldCreateFileWithCorrectContent()
        {
            // Arrange
            byte[] data = [1, 2, 3, 4, 5];
            using MemoryStream ms = new MemoryStream(data);

            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);

            // Act
            FileHelper.WriteFile(ms, tempFilePath);

            // Assert
            Assert.That(File.Exists(tempFilePath), Is.True, "File should be created.");

            byte[] fileBytes = File.ReadAllBytes(tempFilePath);
            Assert.That(fileBytes, Is.EqualTo(data), "File content should match stream data.");

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteFile_WithNullStream_ShouldThrowArgumentNullException()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileHelper.WriteFile((MemoryStream)null, tempFilePath));

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteFile_WithMemoryStreamInvalidPath_ShouldThrowException()
        {
            // Arrange
            byte[] data = [1, 2, 3];
            using MemoryStream ms = new MemoryStream(data);
            string invalidPath = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => "?:\\invalid\\path.txt",  // Invalid on Windows
                _ => "/invalid_dir/\\invalid_path.txt"          // Invalid on Linux/macOS
            };

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => FileHelper.WriteFile(ms, invalidPath));
        }

        [Test]
        public void WriteFile_WithValidBytesArray_ShouldCreateFileWithCorrectContent()
        {
            // Arrange
            byte[] data = [1, 2, 3, 4, 5];

            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);

            // Act
            FileHelper.WriteFile(data, tempFilePath);

            // Assert
            Assert.That(File.Exists(tempFilePath), Is.True, "File should be created.");

            byte[] fileBytes = File.ReadAllBytes(tempFilePath);
            Assert.That(fileBytes, Is.EqualTo(data), "File content should match stream data.");

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteFile_WithNullBytesArray_ShouldThrowArgumentNullException()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileHelper.WriteFile((byte[])null, tempFilePath));

            // Cleanup
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteFile_WithBytesArrayAndInvalidPath_ShouldThrowException()
        {
            // Arrange
            byte[] data = [1, 2, 3];
            string invalidPath = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => "?:\\invalid\\path.txt",  // Invalid on Windows
                _ => "/invalid_dir/\\invalid_path.txt"          // Invalid on Linux/macOS
            };

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => FileHelper.WriteFile(data, invalidPath));
        }

        [Test]
        public void LoadFileToMemoryStream_WithValidFilePath_ShouldReturnMemoryStream()
        {
            // Arrange
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "sample.txt");

            // Act
            using MemoryStream result = FileHelper.LoadFileToMemoryStream(filePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanRead);
            Assert.AreEqual(0, result.Position, "Stream position should be reset to 0");
        }

        [Test]
        public void LoadFileToMemoryStream_WithEmptyFilePath_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => FileHelper.LoadFileToMemoryStream(null));
        }

        [Test]
        public void LoadFileToMemoryStream_WithInvalidFilePath_ShouldThrowException()
        {
            // Arrange
            string invalidPath = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => "?:\\invalid\\path.txt",  // Invalid on Windows
                _ => "/invalid_dir/\\invalid_path.txt"          // Invalid on Linux/macOS
            };

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => FileHelper.LoadFileToMemoryStream(invalidPath));
        }
    }
}
