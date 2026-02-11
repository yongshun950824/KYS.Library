using KYS.Library.Helpers;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
            using StreamReader reader = new StreamReader(stream);
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
            using StreamReader reader = new StreamReader(stream);
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
            using StreamReader reader = new StreamReader(stream);
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
            using MemoryStream result = StreamHelper.ToMemoryStream(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Position, "Stream position should be reset to 0");
            Assert.AreEqual(input.Length, result.Length, "Stream length should match byte array length");

            // Verify that content is identical
            byte[] resultBytes = result.ToArray();
            CollectionAssert.AreEqual(input, resultBytes);
        }

        [Test]
        public void ToMemoryStream_WithEmptyArray_ShouldReturnEmptyStream()
        {
            // Arrange
            byte[] input = Array.Empty<byte>();

            // Act
            using MemoryStream result = StreamHelper.ToMemoryStream(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
            Assert.AreEqual(0, result.Position);
        }

        [Test]
        public void ToMemoryStream_WithNullInput_ShouldThrowNullReferenceException()
        {
            // Arrange
            byte[] input = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => StreamHelper.ToMemoryStream(input));
        }

        [Test]
        public void ToByteArray_WithValidMemoryStream_ShouldReturnEquivalentBytes()
        {
            // Arrange
            byte[] expectedBytes = { 10, 20, 30, 40 };
            using MemoryStream ms = new MemoryStream(expectedBytes);

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
            using MemoryStream ms = new MemoryStream();

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
            byte[] result = StreamHelper.ReadBase64StringToByteArray(base64);

            // Assert
            Assert.AreEqual(originalBytes, result);
        }

        [Test]
        public void ReadBase64StringToByteArray_WithEmptyString_ShouldThrowException()
        {
            // Arrange
            string invalidBase64 = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                StreamHelper.ReadBase64StringToByteArray(invalidBase64));
        }

        [Test]
        public void ReadBase64StringToByteArray_WithCorruptedBase64_ShouldThrowFormatException()
        {
            // Arrange
            string invalidBase64 = "Invalid!!Base64==";

            // Act & Assert
            Assert.Throws<FormatException>(() =>
                StreamHelper.ReadBase64StringToByteArray(invalidBase64));
        }

        [Test]
        public async Task ToBase64Async_WithNullValue_ShouldThrowException()
        {
            // Arrange
            Stream stream = null;
            ArgumentNullException expectedEx = new ArgumentNullException(nameof(stream));

            // Act
            var ex = Assert.CatchAsync<ArgumentException>(() => StreamHelper.ToBase64Async(stream));

            // Assert
            Assert.IsInstanceOf<ArgumentException>(ex);
            Assert.AreEqual(expectedEx.Message, ex.Message);
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
            Assert.AreEqual(expectedBase64, result);
        }
    }
}
