using KYS.Library.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Web;

namespace KYS.TestProject.HelpersUnitTests
{
    internal class UriHelperUnitTest
    {
        [Test]
        public void ToQueryParams_WithMultipleParameters_ShouldReturnCorrectlyFormattedString()
        {
            // Arrange
            var queryParams = new Dictionary<string, string>
            {
                { "param1", "value1" },
                { "param2", "value with spaces" },
                { "param3", "value&with&symbols" }
            };

            // Act
            string result = queryParams.ToQueryParams();

            // Assert
            string expectedEncodedValue2 = HttpUtility.UrlEncode("value with spaces"); // Should be "value%20with%20spaces"
            string expectedEncodedValue3 = HttpUtility.UrlEncode("value&with&symbols"); // Should be "value%26with%26symbols"

            string expected = $"param1=value1&param2={expectedEncodedValue2}&param3={expectedEncodedValue3}";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToQueryParams_WithNoParameters_ShouldReturnEmptyString()
        {
            // Arrange
            var queryParams = new Dictionary<string, string>();

            // Act
            string result = queryParams.ToQueryParams();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ToQueryParams_WithOneParameter_ShouldReturnCorrectlyFormattedString()
        {
            // Arrange
            var queryParams = new Dictionary<string, string>
            {
                { "key", "single_value" }
            };

            // Act
            string result = queryParams.ToQueryParams();

            // Assert
            Assert.AreEqual("key=single_value", result);
        }

        [Test]
        public void ToQueryParams_WithValueContainingReservedChars_IsUrlEncoded()
        {
            // Arrange
            var rawValue = "A value with = and & and /";
            var queryParams = new Dictionary<string, string>
            {
                { "q", rawValue }
            };

            // Act
            string result = queryParams.ToQueryParams();

            // Assert
            string expectedEncodedValue = HttpUtility.UrlEncode(rawValue);
            string expected = $"q={expectedEncodedValue}";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void BuildUriWithQueryParams_WithParameters_ShouldAppendCorrectly()
        {
            // Arrange
            string baseUri = "https://api.example.com/data";
            var queryParams = new Dictionary<string, string>
            {
                { "id", "123" },
                { "sort", "desc" }
            };

            // Act
            string result = queryParams.BuildUriWithQueryParams(baseUri);

            // Assert
            Assert.AreEqual("https://api.example.com/data?id=123&sort=desc", result);
        }

        [Test]
        public void BuildUriWithQueryParams_WithEncodedParameters_AppendsCorrectly()
        {
            // Arrange
            string baseUri = "http://test.com/search";
            var queryParams = new Dictionary<string, string>
            {
                { "q", "search term" }
            };

            // Act
            string result = queryParams.BuildUriWithQueryParams(baseUri);

            // Assert
            string encodedSearchTerm = HttpUtility.UrlEncode("search term");
            string expected = $"http://test.com/search?q={encodedSearchTerm}";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void BuildUriWithQueryParams_WithEmptyDictionary_ShouldAppendOnlyQuestionMark()
        {
            // Arrange
            string baseUri = "http://test.com/resource";
            var queryParams = new Dictionary<string, string>();

            // Act
            string result = queryParams.BuildUriWithQueryParams(baseUri);

            // Assert
            Assert.AreEqual("http://test.com/resource?", result);
        }

        [Test]
        public void ToQueryParamsGeneric_WithTestObject_ShouldReturnCorrectlyFormattedString()
        {
            // Arrange
            var sourceObject = new TestObject
            {
                Name = "Product A",
                Count = 5,
                IsActive = true,
                NullProperty = null
            };

            // Act
            string result = UriHelper.ToQueryParams(sourceObject);

            // Assert
            string expectedPart1 = "Name=Product+A"; // 'Product A' is encoded
            string expectedPart2 = "Count=5";
            string expectedPart3 = "IsActive=True";
            string expectedPart4 = "NullProperty="; // Null value results in an empty string after encoding

            // Checking if the result contains all necessary key=value pairs, separated by '&'
            Assert.StringContains(result, expectedPart1);
            Assert.StringContains(result, expectedPart2);
            Assert.StringContains(result, expectedPart3);
            Assert.StringContains(result, expectedPart4);

            // The number of '&' should be one less than the number of properties (4 - 1 = 3)
            int ampersandCount = result.Split('&').Length - 1;
            Assert.AreEqual(3, ampersandCount);
        }

        private class TestObject
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public bool IsActive { get; set; }
            public string NullProperty { get; set; }
        }
    }
}
