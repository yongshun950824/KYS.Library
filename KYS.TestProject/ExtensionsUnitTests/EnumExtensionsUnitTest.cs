using KYS.Library.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace KYS.TestProject.ExtensionsUnitTests
{
    internal class EnumExtensionsUnitTest
    {
        [SetUp]
        public void SetUp() { }

        [Test]
        public void GetEnumValues_ShouldReturnDistinctValues()
        {
            // Arrange
            var values = EnumExtensions.GetEnumValues<SampleEnum>();

            // Act & Assert
            Assert.AreEqual(values.Count(), values.Distinct().Count());
        }

        [Test]
        public void ToDescription_WithDescriptionAttribute_ShouldReturnDescription()
        {
            // Arrange
            var value = SampleEnum.First;
            var expectedDescription = "1st";

            // Act
            var actualDescription = value.ToDescription();

            // Assert
            Assert.AreEqual(expectedDescription, actualDescription);
        }

        [Test]
        public void ToDescription_WithoutDescriptionAttribute_ShouldReturnDescription()
        {
            // Arrange
            var value = SampleEnum.Third;
            var expectedDescription = "Third";

            // Act
            var actualDescription = value.ToDescription();

            // Assert
            Assert.AreEqual(expectedDescription, actualDescription);
        }

        [Test]
        public void GetEnumNames_ShouldGetAllEnumDescription()
        {
            // Arrange
            string[] expectedValues = ["1st", "2nd", "Third"];

            // Act
            string[] actualValues = EnumExtensions.GetEnumNames<SampleEnum>()
                .ToArray();

            // Assert
            Assert.AreEqual(expectedValues.Length, actualValues.Length);
            Assert.IsTrue(expectedValues.SequenceEqual(actualValues));
        }

        [Test]
        public void ToDictionary_ShouldReturnDictionary()
        {
            // Arrange
            var expectedValue = new Dictionary<SampleEnum, string>
            {
                { SampleEnum.First, "1st" },
                { SampleEnum.Second, "2nd" },
                { SampleEnum.Third, "Third" },
            };

            // Act
            var actualValue = EnumExtensions.ToDictionary<SampleEnum>();

            // Assert
            Assert.AreEqual(expectedValue.Count, actualValue.Count);
            Assert.IsTrue(expectedValue.SequenceEqual(actualValue));
        }

        [Test]
        public void GetValueByDescription_WithDescriptionAttribute_ShouldReturnEnumValue()
        {
            // Arrange
            SampleEnum expectedValue = SampleEnum.First;

            // Arrange
            SampleEnum actualValue = EnumExtensions.GetValueByDescription<SampleEnum>("1st");

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void GetValueByDescription_WithoutDescriptionAttribute_ShouldThrowInvalidOperationException()
        {
            // Act
            var actualEx = Assert.Throws<InvalidOperationException>(
                () => EnumExtensions.GetValueByDescription<SampleEnum>("3rd")
            );

            // Assert
            Assert.AreEqual("Sequence contains no matching element", actualEx.Message);
        }

        [Test]
        public void GetValueByDescription_WithInvalidDescription_ShouldThrowInvalidOperationException()
        {
            // Act
            var actualEx = Assert.Throws<InvalidOperationException>(
                () => EnumExtensions.GetValueByDescription<SampleEnum>("invalid")
            );

            // Assert
            Assert.AreEqual("Sequence contains no matching element", actualEx.Message);
        }

        [Test]
        public void GetEnumMemberValue_WithoutEnumMemberAttribute_ShouldReturnEnumMemberValue()
        {
            // Arrange
            var status = StatusEnum.Canceled;
            var expectedValue = "Canceled";

            // Act
            var actualValue = status.GetEnumMemberValue();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void GetEnumMemberValue_WithEnumMemberAttribute_ShouldReturnEnumMemberValue()
        {
            // Arrange
            var status = StatusEnum.New;
            var expectedValue = "N";

            // Act
            var actualValue = status.GetEnumMemberValue();

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void EnumMemberValueToEnum_WithInvalidValue_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var input = "X";
            var expectedException = new KeyNotFoundException($"{input} is not found.");

            // Act
            var actualException = Assert.Throws<KeyNotFoundException>(
                () => EnumExtensions.EnumMemberValueToEnum<StatusEnum>(input)
            );

            // Assert
            Assert.AreEqual(expectedException.GetType(), actualException.GetType());
            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void EnumMemberValueToEnum_WithEnumDoesNotHaveEnumMemberAttribute_ShouldGetEnum()
        {
            // Arrange
            var input = "Canceled";
            var expectedValue = StatusEnum.Canceled;

            // Act
            var actualValue = EnumExtensions.EnumMemberValueToEnum<StatusEnum>(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void EnumMemberValueToEnum_WithEnumDHasEnumMemberAttribute_ShouldGetEnum()
        {
            // Arrange
            var input = "I";
            var expectedValue = StatusEnum.InProgress;

            // Act
            var actualValue = EnumExtensions.EnumMemberValueToEnum<StatusEnum>(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        enum SampleEnum
        {
            [System.ComponentModel.Description("1st")]
            First = 1,
            [System.ComponentModel.Description("2nd")]
            Second,
            Third
        }

        enum StatusEnum
        {
            [EnumMember(Value = "N")]
            New = 0,
            [EnumMember(Value = "I")]
            InProgress,
            [EnumMember(Value = "F")]
            Finished,
            Canceled
        }
    }
}
