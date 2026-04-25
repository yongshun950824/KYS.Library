using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using JsonObjectAttribute = KYS.Library.Validations.JsonObjectAttribute;

namespace KYS.Library.Tests.ValidationsUnitTests
{
    public class JsonObjectAttributeUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                List<List<object>> testCases =
                [
                    #region Valid JSON object case
                    [
                        new ValidationModel { Prop = JsonConvert.SerializeObject(new { PropOne = "Value 1", PropTwo = 2 }) },
                        true,
                        null
                    ],
                    #endregion
                    #region Valid with empty string / null case
                    [
                        new ValidationWithAllowNullOrEmptyModel { Prop = null },
                        true,
                        null
                    ],
                    [
                        new ValidationWithAllowNullOrEmptyModel { Prop = "" },
                        true,
                        null
                    ],
                    [
                        new ValidationWithAllowNullOrEmptyModel { Prop = JsonConvert.SerializeObject(new { PropOne = "Value 1", PropTwo = 2 }) },
                        true,
                        null
                    ],
                    #endregion
                    #region Invalid JSON / Invalid JSON object case
                    [
                        new ValidationModel { Prop = "VALUE" },
                        false,
                        $"{nameof(ValidationModel.Prop)} is an invalid JSON object."
                    ],
                    [
                        new ValidationModel { Prop = JsonConvert.SerializeObject(1) },
                        false,
                        $"{nameof(ValidationModel.Prop)} is an invalid JSON object."
                    ],
                    [
                        new ValidationModel { Prop = "{ \"PropOne\": \"Missing comma\" \"PropTwo\": null }" },
                        false,
                        $"{nameof(ValidationModel.Prop)} is an invalid JSON object."
                    ],
                    [
                        new ValidationWithMessageModel { Prop = "VALUE" },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    ],
                    [
                        new ValidationWithMessageModel { Prop = JsonConvert.SerializeObject(1) },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    ],
                    [
                        new ValidationWithMessageModel { Prop = "{ \"PropOne\": \"Missing comma\" \"PropTwo\": null }" },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    ],
                    [
                        new ValidationWithNotAllowNullOrEmptyAndMessageModel { Prop = "" },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    ],
                    [
                        new ValidationWithNotAllowNullOrEmptyAndMessageModel { Prop = "{ \"PropOne\": \"Missing comma\" \"PropTwo\": null }" },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    ],
                    [
                        new ValidationWithNotAllowNullOrEmptyAndMessageModel
                        {
                            Prop = JsonConvert.SerializeObject(
                                new List<ValidationWithNotAllowNullOrEmptyAndMessageModel>
                                {
                                    new() { Prop = JsonConvert.SerializeObject(new { PropOne = "Value 1", PropTwo = 2 }) }
                                })
                        },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    ],
                    #endregion
                ];

                // Iterate to create `TestCaseData` with different arguments
                // Without index, will treat as same test case even the reference (type) is different
                for (int i = 0; i < testCases.Count; i++)
                {
                    yield return new TestCaseData(testCases[i][0],
                        testCases[i][1],
                        testCases[i][2],
                        i);
                }
            }
        }

        [TestCaseSource("TestCases")]
        public void JsonObjectAttribute_WithTestCase_ShouldProcessValidationCorrectly(
            dynamic testInput,
            bool expectedResult,
            string expectedErrorMessage,
            int index)
        {
            // Arrange
            ValidationContext validationContext = new(testInput);
            List<ValidationResult> results = [];

            // Act
            bool isValid = Validator.TryValidateObject(testInput, validationContext, results, true);

            // Assert
            Assert.AreEqual(expectedResult, isValid);
            Assert.AreEqual(expectedErrorMessage, results.FirstOrDefault()?.ErrorMessage);
        }

        public class ValidationModel
        {
            [JsonObject]
            public string Prop { get; set; }
        }

        public class ValidationWithAllowNullOrEmptyModel
        {
            [JsonObject(IsAllowNullOrEmpty = true)]
            public string Prop { get; set; }
        }

        public class ValidationWithMessageModel
        {
            // [JsonObject(ErrorMessage = "Prop is invalid JSON object. Please provide a valid JSON object.")]
            [JsonObject("Prop is invalid JSON object. Please provide a valid JSON object.")]
            public string Prop { get; set; }
        }

        public class ValidationWithNotAllowNullOrEmptyAndMessageModel
        {
            // [JsonObject(IsAllowNullOrEmpty = false, ErrorMessage = "Prop is invalid JSON object. Please provide a valid JSON object.")]
            [JsonObject(false, "Prop is invalid JSON object. Please provide a valid JSON object.")]
            public string Prop { get; set; }
        }
    }
}
