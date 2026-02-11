using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
                List<List<object>> testCases = new List<List<object>>()
                {
                    #region Valid JSON object case
                    new List<object>
                    {
                        new ValidationModel { Prop = JsonConvert.SerializeObject(new { PropOne = "Value 1", PropTwo = 2 }) },
                        true,
                        null
                    },
                    #endregion
                    #region Valid with empty string / null case
                    new List<object>
                    {
                        new ValidationWithAllowNullOrEmptyModel { Prop = null },
                        true,
                        null
                    },
                    new List<object>
                    {
                        new ValidationWithAllowNullOrEmptyModel { Prop = "" },
                        true,
                        null
                    },
                    new List<object>
                    {
                        new ValidationWithAllowNullOrEmptyModel { Prop = JsonConvert.SerializeObject(new { PropOne = "Value 1", PropTwo = 2 }) },
                        true,
                        null
                    },
                    #endregion
                    #region Invalid JSON / Invalid JSON object case
                    new List<object>
                    {
                        new ValidationModel { Prop = "VALUE" },
                        false,
                        $"{nameof(ValidationModel.Prop)} is an invalid JSON object."
                    },
                    new List<object>
                    {
                        new ValidationModel { Prop = JsonConvert.SerializeObject(1) },
                        false,
                        $"{nameof(ValidationModel.Prop)} is an invalid JSON object."
                    },
                    new List<object>
                    {
                        new ValidationModel { Prop = "{ \"PropOne\": \"Missing comma\" \"PropTwo\": null }" },
                        false,
                        $"{nameof(ValidationModel.Prop)} is an invalid JSON object."
                    },
                    new List<object>
                    {
                        new ValidationWithMessageModel { Prop = "" },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    },
                    new List<object>
                    {
                        new ValidationWithMessageModel { Prop = "{ \"PropOne\": \"Missing comma\" \"PropTwo\": null }" },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    },
                    new List<object>
                    {
                        new ValidationWithMessageModel
                        {
                            Prop = JsonConvert.SerializeObject(
                                new List<ValidationWithMessageModel>
                                {
                                    new ValidationWithMessageModel { Prop = JsonConvert.SerializeObject(new { PropOne = "Value 1", PropTwo = 2 }) }
                                })
                        },
                        false,
                        $"Prop is invalid JSON object. Please provide a valid JSON object."
                    },
                    #endregion
                };

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
            ValidationContext validationContext = new ValidationContext(testInput);
            List<ValidationResult> results = new List<ValidationResult>();

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
            [JsonObject(IsAllowNullOrEmpty = false, ErrorMessage = "Prop is invalid JSON object. Please provide a valid JSON object.")]
            public string Prop { get; set; }
        }
    }
}
