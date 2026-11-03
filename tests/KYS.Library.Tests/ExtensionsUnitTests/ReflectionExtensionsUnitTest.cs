using KYS.Library.Extensions;
using Moq;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KYS.Library.Tests.ExtensionsUnitTests
{
    internal class ReflectionExtensionsUnitTest
    {
        [Test]
        public void ToName_WithDisplayAttribute_ShouldReturnDisplayName()
        {
            // Arrange
            var prop = typeof(DisplayModel).GetProperty(nameof(DisplayModel.PropertyWithDisplay));

            // Act
            var result = prop.ToName();

            // Assert
            Assert.AreEqual("Custom Display Name", result);
        }

        [Test]
        public void ToName_WithDisplayNameAttribute_ShouldReturnDisplayName()
        {
            // Arrange
            var prop = typeof(DisplayNameModel).GetProperty(nameof(DisplayNameModel.PropertyWithDisplayName));

            // Act
            var result = prop.ToName();

            // Assert
            Assert.AreEqual("Custom DisplayName", result);
        }

        [Test]
        public void ToName_WithoutAttributes_ShouldReturnPropertyName()
        {
            // Arrange
            var prop = typeof(PlainModel).GetProperty(nameof(PlainModel.PlainProperty));

            // Act
            var result = prop.ToName();

            // Assert
            Assert.AreEqual("PlainProperty", result);
        }

        [Test]
        public void ToName_WithDisplayAttributeOnProperty_ShouldReturnDisplayName()
        {
            // Arrange
            MemberInfo member = typeof(DisplayModel).GetProperty(nameof(DisplayModel.PropertyWithDisplay));

            // Act
            var result = member.ToName();

            // Assert
            Assert.AreEqual("Custom Display Name", result);
        }

        [Test]
        public void ToName_WithDisplayNameAttributeOnProperty_ShouldReturnDisplayName()
        {
            // Arrange
            MemberInfo member = typeof(DisplayNameModel).GetProperty(nameof(DisplayNameModel.PropertyWithDisplayName));

            // Act
            var result = member.ToName();

            // Assert
            Assert.AreEqual("Custom DisplayName", result);
        }

        [Test]
        public void ToName_WithoutAttributesOnProperty_ShouldReturnPropertyName()
        {
            // Arrange
            MemberInfo member = typeof(PlainModel).GetProperty(nameof(PlainModel.PlainProperty));

            // Act
            var result = member.ToName();

            // Assert
            Assert.AreEqual("PlainProperty", result);
        }

        [Test]
        public void ToName_WithoutAttributesOnMethod_ShouldReturnMethodName()
        {
            // Arrange
            MemberInfo member = typeof(PlainModel).GetMethod(nameof(PlainModel.PlainMethod));

            // Act
            var result = member.ToName();

            // Assert
            Assert.AreEqual("PlainMethod", result);
        }

        [Test]
        public void GetPropertyDisplayName_WithPropertyName_ShouldReturnPropertyName()
        {
            // Act
            var result = ReflectionExtensions.GetPropertyDisplayName((DisplayModel x) => x.PropertyWithDisplay);

            // Assert
            Assert.AreEqual("Custom Display Name", result);
        }

        [Test]
        public void GetPropertyDisplayName_WithMethod_ShouldReturnPropertyName()
        {
            // Act
            var result = ReflectionExtensions.GetPropertyDisplayName((DisplayModel x) => x.ToString());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetPropertyDisplayName_WithField_ShouldReturnPropertyName()
        {
            // Act
            var result = ReflectionExtensions.GetPropertyDisplayName((DisplayModel x) => x.Field);

            // Assert
            Assert.IsNull(result);
        }

        private class DisplayModel
        {
            public string Field = String.Empty;

            [Display(Name = "Custom Display Name")]
            public string PropertyWithDisplay { get; set; }
        }

        private class DisplayNameModel
        {
            [DisplayName("Custom DisplayName")]
            public string PropertyWithDisplayName { get; set; }
        }

        private class PlainModel
        {
            public string PlainProperty { get; set; }
            public static void PlainMethod() { }
        }
    }
}
