using KYS.Library.Helpers;
using KYS.Library.Validations;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KYS.TestProject
{
    internal class RequiredIf2AttributeUnitTest
    {
        [SetUp]
        public void SetUp()
        {

        }

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                List<List<object>> testCases = new List<List<object>>()
                {
                    #region Equal cases
                    new List<object>
                    {
                        new EqualTestModel { Prop1 = 1, Prop2 = "Provided" },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new EqualTestModel  { Prop1 = 1 },
                        false,
                        1,
                        $"{nameof(EqualTestModel.Prop2)} must be provided when {nameof(EqualTestModel.Prop1)} is 1."
                    },
                    new List<object>
                    {
                        new EqualTestModel { Prop1 = 0 },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new EqualTestModel2 { Prop1 = 1 },
                        false,
                        1,
                        $"{nameof(EqualTestModel2.Prop2)} must be provided when {nameof(EqualTestModel2.Prop1)} is equal to 1."
                    },
                    #endregion Equal cases

                    #region Less Than cases
                    new List<object>
                    {
                        new LessThanTestModel { Prop1 = -1, Prop2 = "Provided" },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new LessThanTestModel { Prop1 = -1 },
                        false,
                        1,
                        $"{nameof(LessThanTestModel.Prop2)} must be provided when {nameof(LessThanTestModel.Prop1)} is less than 0."
                    },
                    new List<object>
                    {
                        new LessThanTestModel { Prop1 = 0 },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new LessThanTestModel { Prop1 = -1 },
                        false,
                        1,
                        $"{nameof(LessThanTestModel2.Prop2)} must be provided when {nameof(LessThanTestModel2.Prop1)} is less than 0."
                    },
                    #endregion Less Than cases

                    #region Less Than Or Equal cases
                    new List<object>
                    {
                        new LessThanOrEqualTestModel { Prop1 = 0, Prop2 = "Provided" },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new LessThanOrEqualTestModel { Prop1 = 0 },
                        false,
                        1,
                        $"{nameof(LessThanOrEqualTestModel.Prop2)} must be provided when {nameof(LessThanTestModel.Prop1)} is less than or equal to 0."
                    },
                    new List<object>
                    {
                        new LessThanOrEqualTestModel { Prop1 = 1 },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new LessThanOrEqualTestModel2 { Prop1 = 0 },
                        false,
                        1,
                        $"{nameof(LessThanOrEqualTestModel2.Prop2)} must be provided when {nameof(LessThanTestModel2.Prop1)} is less than or equal to 0."
                    },
                    #endregion Less Than Or Equal cases

                    #region Greater Than cases
                    new List<object>
                    {
                        new GreaterThanTestModel { Prop1 = 101, Prop2 = "Provided" },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new GreaterThanTestModel { Prop1 = 101 },
                        false,
                        1,
                        $"{nameof(GreaterThanTestModel.Prop2)} must be provided when {nameof(GreaterThanTestModel.Prop1)} is greater than 100."
                    },
                    new List<object>
                    {
                        new GreaterThanTestModel { Prop1 = 100 },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new GreaterThanTestModel2 { Prop1 = 101 },
                        false,
                        1,
                        $"{nameof(GreaterThanTestModel2.Prop2)} must be provided when {nameof(GreaterThanTestModel2.Prop1)} is greater than 100."
                    },
                    #endregion Greater Than cases

                    #region Greater Than Or Equal cases
                    new List<object>
                    {
                        new GreaterThanOrEqualTestModel { Prop1 = 100, Prop2 = "Provided" },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new GreaterThanOrEqualTestModel { Prop1 = 100 },
                        false,
                        1,
                        $"{nameof(GreaterThanOrEqualTestModel.Prop2)} must be provided when {nameof(GreaterThanOrEqualTestModel.Prop1)} is greater than or equal to 100."
                    },
                    new List<object>
                    {
                        new GreaterThanOrEqualTestModel { Prop1 = 99 },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new GreaterThanOrEqualTestModel2 { Prop1 = 100 },
                        false,
                        1,
                        $"{nameof(GreaterThanOrEqualTestModel2.Prop2)} must be provided when {nameof(GreaterThanOrEqualTestModel2.Prop1)} is greater than or equal to 100."
                    },
                    #endregion Greater Than Or Equal cases

                    #region Not Equal cases
                    new List<object>
                    {
                        new NotEqualTestModel { Prop1 = true, Prop2 = "Provided" },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new NotEqualTestModel { Prop1 = true },
                        false,
                        1,
                        $"{nameof(NotEqualTestModel.Prop2)} must be provided when {nameof(NotEqualTestModel.Prop1)} is not false."
                    },
                    new List<object>
                    {
                        new NotEqualTestModel { Prop1 = false },
                        true,
                        0,
                        null
                    },
                    new List<object>
                    {
                        new NotEqualTestModel2 { Prop1 = true },
                        false,
                        1,
                        $"{nameof(NotEqualTestModel2.Prop2)} must be provided when {nameof(NotEqualTestModel2.Prop1)} is not equal to False."
                    },
                    new List<object>
                    {
                        new NotEqualTestModel3 { Prop1 = true },
                        false,
                        1,
                        $"{nameof(NotEqualTestModel3.Prop2)} must be provided when {nameof(NotEqualTestModel3.Prop1)} is not equal to False."
                    },
                    new List<object>
                    {
                        new NotEqualTestModel4 { Prop1 = "No" },
                        false,
                        1,
                        $"{nameof(NotEqualTestModel4.Prop2)} must be provided when {nameof(NotEqualTestModel4.Prop1)} is not equal to Yes."
                    },


                    #endregion Greater Than Or Equal cases
                };

                // Iterate to create `TestCaseData` with different arguments
                // Without index, will treat as same test case even the reference (type) is different
                for (int i = 0; i < testCases.Count; i++)
                {
                    yield return new TestCaseData(testCases[i][0],
                        testCases[i][1],
                        testCases[i][2],
                        testCases[i][3],
                        i);
                }
            }
        }

        [TestCaseSource("TestCases")]
        public void Validate(
            dynamic testInput,
            bool expectedResult,
            int expectedErrorCount,
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
            Assert.AreEqual(expectedErrorCount, results.Count);
            Assert.AreEqual(expectedErrorMessage, results.FirstOrDefault()?.ErrorMessage);
        }

        #region Equal Models
        private class EqualTestModel
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 1,
                @operator: CompareOperator.CompareOperatorConstants.Equal,
                errorMessage: "Prop2 must be provided when Prop1 is 1.")]
            public string Prop2 { get; set; }
        }

        private class EqualTestModel2
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 1,
                @operator: CompareOperator.CompareOperatorConstants.Equal)]
            public string Prop2 { get; set; }
        }
        #endregion Equal Models

        #region Less Than Models
        private class LessThanTestModel
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 0,
                @operator: CompareOperator.CompareOperatorConstants.LessThan,
                errorMessage: "Prop2 must be provided when Prop1 is less than 0.")]
            public string Prop2 { get; set; }
        }

        private class LessThanTestModel2
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 0,
                @operator: CompareOperator.CompareOperatorConstants.LessThan)]
            public string Prop2 { get; set; }
        }
        #endregion Less Than Models

        #region Less Than Or Equal Models
        private class LessThanOrEqualTestModel
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 0,
                @operator: CompareOperator.CompareOperatorConstants.LessThanOrEqual,
                errorMessage: "Prop2 must be provided when Prop1 is less than or equal to 0.")]
            public string Prop2 { get; set; }
        }

        private class LessThanOrEqualTestModel2
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 0,
                @operator: CompareOperator.CompareOperatorConstants.LessThanOrEqual)]
            public string Prop2 { get; set; }
        }
        #endregion Less Than Or Equal Models

        #region Greater Than Models
        private class GreaterThanTestModel
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 100,
                @operator: CompareOperator.CompareOperatorConstants.GreaterThan,
                errorMessage: "Prop2 must be provided when Prop1 is greater than 100.")]
            public string Prop2 { get; set; }
        }

        private class GreaterThanTestModel2
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 100,
                @operator: CompareOperator.CompareOperatorConstants.GreaterThan)]
            public string Prop2 { get; set; }
        }
        #endregion Greater Than Models

        #region Greater Than Or Equal Models
        private class GreaterThanOrEqualTestModel
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 100,
                @operator: CompareOperator.CompareOperatorConstants.GreaterThanOrEqual,
                errorMessage: "Prop2 must be provided when Prop1 is greater than or equal to 100.")]
            public string Prop2 { get; set; }
        }

        private class GreaterThanOrEqualTestModel2
        {
            public int Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: 100,
                @operator: CompareOperator.CompareOperatorConstants.GreaterThanOrEqual)]
            public string Prop2 { get; set; }
        }
        #endregion Greater Than Or Equal Models

        #region Not Equal Models
        private class NotEqualTestModel
        {
            public bool Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: false,
                @operator: CompareOperator.CompareOperatorConstants.NotEqual,
                errorMessage: "Prop2 must be provided when Prop1 is not false.")]
            public string Prop2 { get; set; }
        }

        private class NotEqualTestModel2
        {
            public bool Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: false,
                @operator: CompareOperator.CompareOperatorConstants.NotEqual)]
            public string Prop2 { get; set; }
        }

        private class NotEqualTestModel3
        {
            public bool Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: false,
                @operator: CompareOperator.CompareOperatorConstants.NotEqual)]
            public string Prop2 { get; set; }
        }

        private class NotEqualTestModel4
        {
            public string Prop1 { get; set; }

            [RequiredIf2(otherPropertyName: nameof(Prop1),
                matchedValue: "Yes",
                @operator: CompareOperator.CompareOperatorConstants.NotEqual)]
            public int Prop2 { get; set; }
        }
        #endregion Not Equal Models
    }
}
