using KYS.Library.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System;

namespace KYS.TestProject.ExtensionsUnitTests
{
    internal class CacheExtensionsUnitTest
    {
        [Test]
        public void Add_WithKeyAndExpirySeconds_ShouldStoreValueInCache()
        {
            // Arrange
            string key = "myKey";
            string value = "myValue";

            var cacheMock = new Mock<IMemoryCache>();
            var entryMock = new Mock<ICacheEntry>();

            object storedValue = null;
            entryMock.SetupProperty(e => e.Value);
            entryMock
                .SetupSet(e => e.Value = It.IsAny<object>())
                .Callback<object>(v => storedValue = v);

            cacheMock.Setup(c => c.CreateEntry(key))
                     .Returns(entryMock.Object);

            // Act
            cacheMock.Object.Add(value, key, 60);

            // Assert
            cacheMock.Verify(c => c.CreateEntry(key), Times.Once);  // Prove IMemoryCache.Set() was called
            entryMock.VerifySet(e => e.Value = value, Times.Once);  // Prove value was stored
            Assert.AreEqual(value, storedValue);
        }

        [Test]
        public void Add_WithKeyAndExpiryTimeSpan_ShouldStoreValueInCache()
        {
            // Arrange
            string key = "myKey";
            string value = "myValue";

            var cacheMock = new Mock<IMemoryCache>();
            var entryMock = new Mock<ICacheEntry>();

            object storedValue = null;
            entryMock.SetupProperty(e => e.Value);
            entryMock
                .SetupSet(e => e.Value = It.IsAny<object>())
                .Callback<object>(v => storedValue = v);

            cacheMock.Setup(c => c.CreateEntry(key))
                     .Returns(entryMock.Object);

            // Act
            cacheMock.Object.Add(value, key, TimeSpan.FromHours(1));

            // Assert
            cacheMock.Verify(c => c.CreateEntry(key), Times.Once);  // Prove IMemoryCache.Set() was called
            entryMock.VerifySet(e => e.Value = value, Times.Once);  // Prove value was stored
            Assert.AreEqual(value, storedValue);
        }

        [Test]
        public void Get_WithNonExistentEntry_ShouldReturnsFalse()
        {
            // Arrange
            string key = "myKey";

            var cacheMock = new Mock<IMemoryCache>();
            object dummy;
            cacheMock
                .Setup(c => c.TryGetValue(key, out dummy))
                .Returns(false);

            // Act
            var result = cacheMock.Object.Get(key, out string value);

            // Assert
            cacheMock.Verify(c => c.TryGetValue(key, out dummy), Times.Once);
            Assert.IsFalse(result);
        }

        [Test]
        public void Get_WithExistingEntry_ShouldReturnsTrue()
        {
            // Arrange
            string key = "myKey";
            object cacheValue = "myValue";

            var cacheMock = new Mock<IMemoryCache>();
            cacheMock
                .Setup(c => c.TryGetValue(key, out cacheValue))
                .Returns(true);

            // Act
            var result = cacheMock.Object.Get(key, out string value);

            // Assert
            cacheMock.Verify(c => c.TryGetValue(key, out cacheValue), Times.Once);
            Assert.IsTrue(result);
            Assert.AreEqual(cacheValue, value);
        }
    }
}
