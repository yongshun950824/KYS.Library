using KYS.Library.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KYS.TestProject.ExtensionsUnitTests
{
    internal class DataTableExtensionsUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void IsNullOrEmpty_WithNull_ShouldReturnTrue()
        {
            // Arrange
            DataTable dt = null;

            // Act
            bool isNullOrEmpty = dt.IsNullOrEmpty();

            // Assert
            Assert.IsTrue(isNullOrEmpty);
            Assert.IsNull(dt);
        }

        [Test]
        public void IsNullOrEmpty_WithEmptyDataTable_ShouldReturnTrue()
        {
            // Arrange
            DataTable dt = new DataTable();

            // Act
            bool isNullOrEmpty = dt.IsNullOrEmpty();

            // Assert
            Assert.IsTrue(isNullOrEmpty);
            Assert.AreEqual(0, dt.Rows.Count);
        }

        [Test]
        public void IsNullOrEmpty_WithNonEmptyDataTable_ShouldReturnFalse()
        {
            // Arrange
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            var row = dt.NewRow();
            row["Name"] = "Test";
            dt.Rows.Add(row);

            // Act
            bool isNullOrEmpty = dt.IsNullOrEmpty();

            // Assert
            Assert.IsFalse(isNullOrEmpty);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("Test", dt.Rows[0]["Name"]);
        }

        [Test]
        public void ToList_WithNull_ShouldReturnEmptyList()
        {
            // Arrange
            DataTable dt = null;

            // Act
            IList<Model> list = dt.ToList<Model>();

            // Assert
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void ToList_WithEmptyDataTable_ShouldReturnEmptyList()
        {
            // Arrange
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Description", typeof(string));

            // Act
            IList<Model> list = dt.ToList<Model>();

            // Assert
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void ToList_WithNonEmptyDataTable_ShouldReturnListWithMappedValues()
        {
            // Arrange
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Description", typeof(string));

            var row = dt.NewRow();
            row["Name"] = "Test";
            row["Description"] = "This is a description.";

            dt.Rows.Add(row);

            // Act
            IList<Model> list = dt.ToList<Model>();

            // Assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(dt.Rows[0][nameof(Model.Name)], list[0].Name);
            Assert.AreEqual(dt.Rows[0][nameof(Model.Description)], list[0].Description);
        }

        [Test]
        public void ToList_WithUnknownProperty_ShouldReturnListWithMappedValues()
        {
            // Arrange
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("UnknownKey", typeof(string));

            var row = dt.NewRow();
            row["Name"] = "Test";
            row["Description"] = "This is a description.";
            row["UnknownKey"] = "Unknown value";

            dt.Rows.Add(row);

            // Act
            IList<Model> list = dt.ToList<Model>();

            // Assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(dt.Rows[0][nameof(Model.Name)], list[0].Name);
            Assert.AreEqual(dt.Rows[0][nameof(Model.Description)], list[0].Description);
        }

        [Test]
        public void ToList_WithDBNullValue_ShouldReturnListWithMappedValues()
        {
            // Arrange
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Description", typeof(string));

            var row = dt.NewRow();
            row["Name"] = "Test";
            row["Description"] = DBNull.Value;

            dt.Rows.Add(row);

            // Act
            IList<Model> list = dt.ToList<Model>();

            // Assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(dt.Rows[0][nameof(Model.Name)], list[0].Name);
            Assert.AreEqual(null, list[0].Description);
        }

        private record Model(string Name, string Description)
        {
            public Model(): this(null, null) { }
        };
    }
}
