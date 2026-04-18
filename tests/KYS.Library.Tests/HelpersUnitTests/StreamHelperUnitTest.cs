using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KYS.Library.Helpers;
using NUnit.Framework;

namespace KYS.Library.Tests.HelpersUnitTests
{
    internal class StreamHelperUnitTest
    {
        [Test]
        public void WriteStringIntoStream_WithValidString_ShouldReturnEquivalentStream()
        {
            // Arrange
            string input = "test";

            // Act
            using Stream stream = StreamHelper.WriteStringIntoStream(input);
            using StreamReader reader = new(stream);
            string result = reader.ReadToEnd();

            // Assert
            Assert.IsNotNull(stream);
            Assert.IsTrue(stream.CanRead);
            Assert.AreEqual(input, result);
        }

        [Test]
        public void WriteStringIntoStream_WithEmptyString_ShouldReturnEquivalentStream()
        {
            // Arrange
            string input = String.Empty;

            // Act
            using Stream stream = StreamHelper.WriteStringIntoStream(input);
            using StreamReader reader = new(stream);
            string result = reader.ReadToEnd();

            // Assert
            Assert.AreEqual(0, stream.Position, "Stream position should be reset to 0");
            Assert.AreEqual(input, result);
        }

        [Test]
        public void WriteStringIntoStream_WithNullString_ShouldReturnNull()
        {
            // Arrange
            string input = null;

            // Act
            using Stream stream = StreamHelper.WriteStringIntoStream(input);
            using StreamReader reader = new(stream);
            string result = reader.ReadToEnd();

            // Assert
            Assert.AreEqual(0, stream.Position, "Stream position should be reset to 0");
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void ToMemoryStream_WithValidByteArray_ShouldReturnEquivalentStream()
        {
            // Arrange
            byte[] input = { 1, 2, 3, 4, 5 };

            // Act
            var result = StreamHelper.ToMemoryStream(input);

            // Assert
            Assert.IsTrue(result.IsSuccess);

            using var ms = result.Value;
            Assert.IsNotNull(ms);
            Assert.AreEqual(0, ms.Position, "Stream position should be reset to 0");
            Assert.AreEqual(input.Length, ms.Length, "Stream length should match byte array length");

            // Verify that content is identical
            byte[] resultBytes = ms.ToArray();
            CollectionAssert.AreEqual(input, resultBytes);
        }

        [Test]
        public void ToMemoryStream_WithEmptyArray_ShouldReturnEmptyStream()
        {
            // Arrange
            byte[] input = Array.Empty<byte>();

            // Act
            var result = StreamHelper.ToMemoryStream(input);

            // Assert
            Assert.IsTrue(result.IsSuccess);

            using var ms = result.Value;
            Assert.IsNotNull(ms);
            Assert.AreEqual(0, ms.Length);
            Assert.AreEqual(0, ms.Position);
        }

        [Test]
        public void ToMemoryStream_WithNullInput_ShouldThrowNullReferenceException()
        {
            // Arrange
            byte[] input = null;

            // Act
            var result = StreamHelper.ToMemoryStream(input);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(DomainErrors.CannotBeNull("byteArray"), result.Error);
        }

        [Test]
        public void ToByteArray_WithValidMemoryStream_ShouldReturnEquivalentBytes()
        {
            // Arrange
            byte[] expectedBytes = { 10, 20, 30, 40 };
            using MemoryStream ms = new(expectedBytes);

            // Act
            byte[] result = StreamHelper.ToByteArray(ms);

            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expectedBytes, result);
        }

        [Test]
        public void ToByteArray_WithEmptyMemoryStream_ShouldReturnEmptyArray()
        {
            // Arrange
            using MemoryStream ms = new();

            // Act
            byte[] result = StreamHelper.ToByteArray(ms);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void ToByteArray_WithNullInput_ShouldThrowNullReferenceException()
        {
            // Arrange
            MemoryStream ms = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => StreamHelper.ToByteArray(ms));
        }

        [Test]
        public void ReadBase64StringToByteArray_WithValidBase64_ReturnsExpectedBytes()
        {
            // Arrange
            byte[] originalBytes = { 1, 2, 3, 4, 5 };
            string base64 = Convert.ToBase64String(originalBytes);

            // Act
            var result = StreamHelper.ReadBase64StringToByteArray(base64);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(originalBytes, result.Value);
        }

        [Test]
        public void ReadBase64StringToByteArray_WithEmptyString_ShouldReturnResultFailure()
        {
            // Arrange
            string invalidBase64 = "";

            // Act & Assert
            var result = StreamHelper.ReadBase64StringToByteArray(invalidBase64);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(DomainErrors.Required("base64String"), result.Error);
        }

        [Test]
        public void ReadBase64StringToByteArray_WithCorruptedBase64_ShouldReturnResultFailure()
        {
            // Arrange
            string invalidBase64 = "Invalid!!Base64==";

            // Act
            var result = StreamHelper.ReadBase64StringToByteArray(invalidBase64);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public async Task ToBase64Async_WithNullValue_ShouldReturnResultFailure()
        {
            // Arrange
            Stream stream = null;

            // Act
            var result = await StreamHelper.ToBase64Async(stream);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(DomainErrors.CannotBeNull("stream"), result.Error);
        }

        [Test]
        public async Task ToBase64Async_WithStream_ShouldReturnBase64()
        {
            // Arrange
            var value = "Hello World!";
            var bytes = Encoding.UTF8.GetBytes(value);
            var expectedBase64 = Convert.ToBase64String(bytes);
            using var stream = new MemoryStream(bytes);

            // Act
            var result = await StreamHelper.ToBase64Async(stream);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedBase64, result.Value);
        }
    }
}
