using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static KYS.Library.Helpers.JsonHelper;

namespace KYS.TestProject.HelpersUnitTests
{
    internal class JsonHelperUnitTest
    {
        private readonly DateTime testDate = new DateTime(2023, 10, 27, 10, 30, 0, DateTimeKind.Utc);

        [Test]
        public void ConstructFlattenKey_WithFlattenFormat_ShouldThrowException()
        {
            // Arrange
            FlattenFormat unknown = (FlattenFormat)999;
            var expectedEx = new InvalidEnumArgumentException("flattenFormat", (int)unknown, typeof(FlattenFormat));

            // Act
            var ex = Assert.Catch<InvalidEnumArgumentException>(
                () => ConstructFlattenKey("test", unknown)
            );

            // Assert
            Assert.IsInstanceOf<InvalidEnumArgumentException>(ex);
            Assert.AreEqual(expectedEx.Message, ex.Message);
        }

        [Test]
        public void FlattenObject_SimpleObject_ReturnsExpectedValuesAndJsonPathKeys()
        {
            // Arrange
            var source = new SimpleObject
            {
                Name = "TestItem",
                Value = 42,
                Date = testDate
            };

            // Act
            var result = FlattenObject(source, FlattenFormat.JsonPath);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("TestItem", result["Name"]);
            Assert.AreEqual(42L, result["Value"]); // JValue.Value<object>() often returns long for int
            Assert.AreEqual(testDate, Helpers.ParseDateFromIso8601(result["Date"].ToString())); // Expect DateTime due to serializer settings
        }

        [Test]
        public void FlattenObject_NestedObject_ReturnsFlattenedDataWithCorrectKeys()
        {
            // Arrange
            var source = new NestedObject
            {
                Id = "A101",
                Detail = new SimpleObject { Name = "Inner", Value = 99, Date = testDate },
                Tags = new List<string> { "tag1", "tag2" },
                Counts = new int[] { 10, 20 },
                Metadata = new Dictionary<string, object> { { "key", "metaValue" } },
                IsActive = true,
                NullValue = null
            };

            // Act
            var result = FlattenObject(source, FlattenFormat.JsonPath);

            // Assert
            Assert.AreEqual(11, result.Count);

            // Simple property
            Assert.AreEqual("A101", result["Id"]);

            // Nested properties
            Assert.AreEqual("Inner", result["Detail.Name"]);
            Assert.AreEqual(99L, result["Detail.Value"]);

            // Array/List properties (JsonPath notation)
            Assert.AreEqual("tag1", result["Tags[0]"]);
            Assert.AreEqual("tag2", result["Tags[1]"]);
            Assert.AreEqual(10L, result["Counts[0]"]);
            Assert.AreEqual(20L, result["Counts[1]"]);

            // Dictionary/Object properties
            Assert.AreEqual("metaValue", result["Metadata.key"]);

            // Boolean property
            Assert.IsTrue((bool)result["IsActive"]);

            // Null property (value should be null)
            Assert.IsNull(result["NullValue"]);
        }

        [Test]
        public void FlattenArray_ListOfSimpleObjects_ReturnsFlattenedDataWithCorrectKeys()
        {
            // Arrange
            var sourceList = new List<SimpleObject>
            {
                new SimpleObject { Name = "Item0", Value = 1, Date = testDate },
                new SimpleObject { Name = "Item1", Value = 2, Date = testDate.AddDays(1) }
            };
            var token = JToken.FromObject(sourceList);

            // Act
            var result = FlattenArray(token, FlattenFormat.JsonPath);

            // Assert
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("Item0", result["[0].Name"]);
            Assert.AreEqual(1L, result["[0].Value"]);
            Assert.AreEqual("Item1", result["[1].Name"]);
            Assert.AreEqual(2L, result["[1].Value"]);
            // Note: Date comparison relies on the JsonSerializer settings for RoundtripKind
            Assert.AreEqual(testDate, Helpers.ParseDateFromIso8601(result["[0].Date"].ToString()));
        }

        [Test]
        public void FlattenArray_JArrayOfPrimitives_ReturnsCorrectKeysAndValues()
        {
            // Arrange
            var sourceArray = new JArray(10, "ten", true);

            // Act
            var result = FlattenArray(sourceArray, FlattenFormat.JsonPath);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(10L, result["[0]"]);
            Assert.AreEqual("ten", result["[1]"]);
            Assert.IsTrue((bool)result["[2]"]);
        }

        [Test]
        public void Flatten_JObjectToken_CallsFlattenObject()
        {
            // Arrange
            var source = new { Name = "Check", Value = 1 };
            var token = JToken.FromObject(source);

            // Act
            var result = Flatten(token);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Check", result["Name"]);
        }

        [Test]
        public void Flatten_JArrayToken_CallsFlattenArray()
        {
            // Arrange
            var source = JArray.FromObject(new string[] { "a", "b" });
            var token = source;

            // Act
            var result = Flatten(token);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("a", result["[0]"]);
            Assert.AreEqual("b", result["[1]"]);
        }

        [Test]
        public void Flatten_InvalidToken_ShouldThrowArgumentException()
        {
            // Arrange
            var token = new JValue(123); // JTokenType.Integer, which is neither Object nor Array

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Flatten(token));
            Assert.StringContains(ex.Message, "is neither a valid JSON object nor array");
        }

        [Test]
        public void FlattenArray_WithNestedArray_ReturnsCorrectlyFlattenedKeys()
        {
            // Arrange
            // JSON structure: [ [10, 20], ["a", "b"] ]
            var nestedArray = new JArray(
                new JArray(10, 20),
                new JArray("a", "b")
            );

            var token = JToken.FromObject(nestedArray);

            // Act
            var result = FlattenArray(token, FlattenFormat.JsonPath);

            // Assert
            Assert.AreEqual(4, result.Count);

            // The outer array index [0] contains an inner array.
            // The path should be: OuterArray[i] + InnerArrayPath

            // Path for '10'
            Assert.True(result.ContainsKey("[0][0]"));
            Assert.AreEqual(10L, result["[0][0]"]); // 10 (as long)

            // Path for '20'
            Assert.True(result.ContainsKey("[0][1]"));
            Assert.AreEqual(20L, result["[0][1]"]);

            // Path for 'a'
            Assert.True(result.ContainsKey("[1][0]"));
            Assert.AreEqual("a", result["[1][0]"]);

            // Path for 'b'
            Assert.True(result.ContainsKey("[1][1]"));
            Assert.AreEqual("b", result["[1][1]"]);
        }

        [Theory]
        [TestCase("Root.Item", "Root.Item", FlattenFormat.JsonPath)]
        [TestCase("[0].Property", "[0].Property", FlattenFormat.JsonPath)]
        public void ConstructFlattenKeyByFormat_JsonPath_ReturnsOriginalKey(string inputPath, string expected, FlattenFormat format)
        {
            Assert.AreEqual(expected, ConstructFlattenKey(inputPath, format));
        }

        [Theory]
        [TestCase("Root.Item", "Root:Item", FlattenFormat.DotNet)]
        [TestCase("Root.Nested[0].Value", "Root:Nested:0:Value", FlattenFormat.DotNet)]
        [TestCase("[0].Value", "0:Value", FlattenFormat.DotNet)]
        public void ConstructFlattenKeyByFormat_DotNetFormat_ConvertsToColonSeparatedKey(string inputPath, string expected, FlattenFormat format)
        {
            Assert.AreEqual(expected, ConstructFlattenKey(inputPath, format));
        }

        [Theory]
        [TestCase("Root.Item", "Root.Item", FlattenFormat.JS)]
        [TestCase("Root.Nested[0].Value", "Root.Nested.0.Value", FlattenFormat.JS)]
        [TestCase("[0].Value", "0.Value", FlattenFormat.JS)]
        public void ConstructFlattenKeyByFormat_JSFormat_ConvertsToArrayBracketsToDotIndex(string inputPath, string expected, FlattenFormat format)
        {
            Assert.AreEqual(expected, ConstructFlattenKey(inputPath, format));
        }

        private class SimpleObject
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public DateTime Date { get; set; }
        }

        private class NestedObject
        {
            public string Id { get; set; }
            public SimpleObject Detail { get; set; }
            public List<string> Tags { get; set; } = new List<string>();
            public Dictionary<string, object> Metadata { get; set; }
            public int[] Counts { get; set; } = new int[0];
            public bool? IsActive { get; set; }
            public object NullValue { get; set; }
        }
    }
}
