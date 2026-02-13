using KYS.Library.Extensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace KYS.Library.Tests.ExtensionsUnitTests
{
    internal class IDictionaryExtensionsUnitTest
    {
        private Dictionary<string, string> _dictionary;

        [SetUp]
        public void SetUp()
        {
            _dictionary = new Dictionary<string, string>
            {
                { "Key", "Value" }
            };
        }

        [Test]
        public void Upsert_WithNewKey_ShouldInsertCorrectly()
        {
            // Arrange
            string newKey = "NewKey";
            string newValue = "NewValue";

            // Act
            _dictionary.Upsert(newKey, newValue);

            // Assert
            Assert.AreEqual(2, _dictionary.Count);
            Assert.IsTrue(_dictionary.TryGetValue(newKey, out string actualNewValue));
            Assert.AreEqual(newValue, actualNewValue);
        }

        [Test]
        public void Upsert_WithExisingKey_ShouldUpdateCorrectly()
        {
            // Arrange
            string key = "Key";
            string updatedValue = "NewValue";

            // Act
            _dictionary.Upsert(key, updatedValue);

            // Assert
            Assert.AreEqual(1, _dictionary.Count);
            Assert.IsTrue(_dictionary.TryGetValue(key, out string actualUpdatedValue));
            Assert.AreEqual(updatedValue, actualUpdatedValue);
        }

        [Test]
        public void Upsert_WithNewKeyAsFunc_ShouldInsertCorrectly()
        {
            // Arrange
            string newKey = "NewKey";
            string newValue = "NewValue";

            // Act
            _dictionary.Upsert(newKey, () => newValue);

            // Assert
            Assert.AreEqual(2, _dictionary.Count);
            Assert.IsTrue(_dictionary.TryGetValue(newKey, out string actualNewValue));
            Assert.AreEqual(newValue, actualNewValue);
        }

        [Test]
        public void Upsert_WithExistingKeyAsFunc_ShouldUpdateCorrectly()
        {
            // Arrange
            string key = "Key";
            string updatedValue = "NewValue";

            // Act
            _dictionary.Upsert(key, () => updatedValue);

            // Assert
            Assert.AreEqual(1, _dictionary.Count);
            Assert.IsTrue(_dictionary.TryGetValue(key, out string actualUpdatedValue));
            Assert.AreEqual(updatedValue, actualUpdatedValue);
        }
    }
}
