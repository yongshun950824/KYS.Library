using KYS.Library.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KYS.TestProject.ExtensionsUnitTests
{
    internal class IEnumerableExtensionsUnitTest
    {
        private List<int> _numbers;

        [SetUp]
        public void Setup()
        {
            _numbers = Enumerable.Range(1, 10).ToList();
        }

        [Test]
        public void AsString_WithDefaultSeparator_ShouldReturnString()
        {
            // Arrange
            string[] inputList = ["A", "B", "C"];

            // Act
            string result = inputList.AsString();

            // Assert
            Assert.AreEqual("A,B,C", result);
        }

        [Test]
        public void AsString_WithProvidingSeparator_ShouldReturnString()
        {
            // Arrange
            string[] inputList = ["A", "B", "C"];

            // Act
            string result = inputList.AsString(";");

            // Assert
            Assert.AreEqual("A;B;C", result);
        }

        [Test]
        public void Trim_WithNullOrWhitespaceEntries_ShouldReturnTrimmedElements()
        {
            // Arrange
            string[] inputList = ["A", "B", "C", null, String.Empty];

            // Act
            IEnumerable<string> result = inputList.Trim();

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public void HasDuplicates_WithDuplicateElements_ShouldReturnTrue()
        {
            // Arrange
            string[] inputList = ["A", "A", "B"];

            // Act
            bool hasDuplicate = inputList.HasDuplicates();

            // Assert
            Assert.IsTrue(hasDuplicate);
        }

        [Test]
        public void HasDuplicates_WithUniqueElements_ShouldReturnFalse()
        {
            // Arrange
            string[] inputList = ["A", "B", "C"];

            // Act
            bool hasDuplicate = inputList.HasDuplicates();

            // Assert
            Assert.IsFalse(hasDuplicate);
        }

        [Test]
        public void ToString_WithEmptyArrayOfStrings_ShouldReturnEmptyString()
        {
            // Arrange
            string[] inputList = [];

            // Act
            string result = IEnumerableExtensions.ToString(inputList);

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        [Test]
        public void ToString_WithArrayOfStringsAndDefaultSettings_ShouldReturnString()
        {
            // Arrange
            string[] inputList = ["A", "B"];

            // Act
            string result = IEnumerableExtensions.ToString(inputList);

            // Assert
            Assert.AreEqual("A, B", result);
        }

        [Test]
        public void ToString_WithArrayOfStringsAndSeparator_ShouldReturnString()
        {
            // Arrange
            string[] inputList = ["A", "B"];

            // Act
            string result = IEnumerableExtensions.ToString(inputList, ';');

            // Assert
            Assert.AreEqual("A; B", result);
        }

        [Test]
        public void ToString_WithArrayOfStringsAndNoWhiteSpaceAfterSeparator_ShouldReturnString()
        {
            // Arrange
            string[] inputList = ["A", "B"];

            // Act
            string result = IEnumerableExtensions.ToString(inputList, hasWhiteSpaceAfterSeparator: false);

            // Assert
            Assert.AreEqual("A,B", result);
        }

        [Test]
        public void ToString_WithArrayOfStringsAndRemoveEmptyItem_ShouldReturnString()
        {
            // Arrange
            string[] inputList = ["A", "B", ""];

            // Act
            string result = IEnumerableExtensions.ToString(inputList, removeEmptyItem: true);

            // Assert
            Assert.AreEqual("A, B", result);
        }

        [Test]
        public void ToString_WithArrayOfObjects_ShouldReturnString()
        {
            // Arrange
            Item[] inputList = [new Item("A"), new Item("B")];

            // Act
            string result = IEnumerableExtensions.ToString(inputList);

            // Assert
            Assert.AreEqual("A, B", result);
        }

        [Test]
        public void ToString_WithArrayOfObjectsAndRemoveEmptyItem_ShouldReturnString()
        {
            // Arrange
            Item[] inputList = [null, new Item("A"), new Item("B")];

            // Act
            string result = IEnumerableExtensions.ToString(inputList, removeEmptyItem: true);

            // Assert
            Assert.AreEqual("A, B", result);
        }

        [Test]
        public void ToString_WithEmptyArrayOfStructs_ShouldReturnEmptyString()
        {
            // Arrange
            int?[] inputList = [];

            // Act
            string result = IEnumerableExtensions.ToString(inputList);

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        [Test]
        public void ToString_WithArrayOfStructsAndDefaultSettings_ShouldReturnString()
        {
            // Arrange
            int?[] inputList = [1, 2];

            // Act
            string result = IEnumerableExtensions.ToString(inputList);

            // Assert
            Assert.AreEqual("1, 2", result);
        }

        [Test]
        public void ToString_WithArrayOfStructsAndSeparator_ShouldReturnString()
        {
            // Arrange
            int?[] inputList = [1, 2];

            // Act
            string result = IEnumerableExtensions.ToString(inputList, ';');

            // Assert
            Assert.AreEqual("1; 2", result);
        }

        [Test]
        public void ToString_WithArrayOfStructsAndNoWhiteSpaceAfterSeparator_ShouldReturnString()
        {
            // Arrange
            int?[] inputList = [1, 2];

            // Act
            string result = IEnumerableExtensions.ToString(inputList, hasWhiteSpaceAfterSeparator: false);

            // Assert
            Assert.AreEqual("1,2", result);
        }

        [Test]
        public void ToString_WithArrayOfStructsAndRemoveEmptyItem_ShouldReturnString()
        {
            // Arrange
            int?[] inputList = [1, 2, null];

            // Act
            string result = IEnumerableExtensions.ToString(inputList, removeEmptyItem: true);

            // Assert
            Assert.AreEqual("1, 2", result);
        }

        [Test]
        public void Paging_WithNullEnumerable_ShouldReturnNull()
        {
            // Arrange
            IEnumerable<int> input = null;

            // Act
            var result = input.Paging(1, 5);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Paging_WithEmptyEnumerable_ShouldReturnEmpty()
        {
            // Arrange
            var input = new List<int>();

            // Act
            var result = input.Paging(1, 5, out int totalCount);

            // Assert
            Assert.IsEmpty(result);
            Assert.AreEqual(0, totalCount);
        }

        [Test]
        public void Paging_FirstPage_ShouldReturnCorrectItemsAndTotalCount()
        {
            // Act
            var result = _numbers.Paging(1, 3, out int totalCount)
                .ToList();

            // Assert
            Assert.AreEqual(_numbers.Count, totalCount);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
        }

        [Test]
        public void Paging_SecondPage_ShouldReturnCorrectItems()
        {
            // Act
            var result = _numbers.Paging(2, 3, out int totalCount)
                .ToList();

            // Assert
            Assert.AreEqual(_numbers.Count, totalCount);
            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, result);
        }

        [Test]
        public void Paging_ZeroBasedPageNumber_ShouldStartAtIndexZero()
        {
            // Act
            var result = _numbers.Paging(0, 4, true, out int totalCount)
                .ToList();

            // Assert
            Assert.AreEqual(_numbers.Count, totalCount);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, result);
        }

        [Test]
        public void Paging_PageNumberOutOfRange_ShouldReturnEmptyAndZeroTotalCount()
        {
            // Act
            var result = _numbers.Paging(5, 5, out int totalCount).ToList();

            // Assert
            Assert.AreEqual(0, totalCount);
            Assert.IsEmpty(result);
        }

        [Test]
        public void OrderBySequence_WithAllSourceElementsExistedInOrder_ShouldReturnSortedEnumerable()
        {
            // Arrange
            string[] inputList = ["Level 1", "Level 10", "Level 9"];
            string[] orderList = ["Level 1", "Level 9", "Level 10"];

            // Act
            IEnumerable<string> result = inputList.OrderBySequence(x => x, orderList);

            // Assert
            Assert.IsTrue(result.SequenceEqual(orderList));
        }

        [Test]
        public void OrderBySequence_WithSourceElementsMissingInOrder_ShouldReturnSortedEnumerable()
        {
            // Arrange
            string[] inputList = ["Level 1", "Level 10", "Level 9"];
            string[] orderList = ["Level 1", "Level 9"];

            // Act
            IEnumerable<string> result = inputList.OrderBySequence(x => x, orderList);

            // Assert
            Assert.IsTrue(result.SequenceEqual(orderList));
        }

        [Test]
        public void OrderBy_WithAllSourceElementsExistedInOrder_ShouldReturnSortedEnumerable()
        {
            // Arrange
            string[] inputList = ["Level 1", "Level 10", "Level 9"];
            string[] orderList = ["Level 1", "Level 9", "Level 10"];

            // Act
            IEnumerable<string> result = inputList.OrderBy(x => x, orderList);

            // Assert
            Assert.IsTrue(result.SequenceEqual(orderList));
        }

        [Test]
        public void OrderBy_WithSourceElementsMissingInOrder_ShouldReturnSortedEnumerable()
        {
            // Arrange
            string[] inputList = ["Level 21", "Level 1", "Level 10", "Level 9"];
            string[] orderList = ["Level 1", "Level 9"];
            string[] expectedResult = ["Level 1", "Level 9", "Level 10", "Level 21" ];

            // Act
            IEnumerable<string> actualResult = inputList.OrderBy(x => x, orderList);

            // Assert
            Assert.IsTrue(expectedResult.SequenceEqual(actualResult));
        }

        private record Item(string Name)
        {
            public Item() : this(String.Empty) { }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
