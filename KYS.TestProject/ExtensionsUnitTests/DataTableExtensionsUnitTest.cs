using KYS.Library.Extensions;
using NUnit.Framework;
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
        public void TestIsNullOrEmptyWithNull()
        {
            // Arrange
            DataTable dt = null;

            // Act
            bool isNullEmptyOrEmpty = dt.IsNullOrEmpty();

            // Assert
            Assert.IsTrue(isNullEmptyOrEmpty);
            Assert.IsNull(dt);
        }

        [Test]
        public void TestIsNullOrEmptyWithEmptyDataTable()
        {
            // Arrange
            DataTable dt = new DataTable();

            // Act
            bool isNullEmptyOrEmpty = dt.IsNullOrEmpty();

            // Assert
            Assert.IsTrue(isNullEmptyOrEmpty);
            Assert.AreEqual(0, dt.Rows.Count);
        }

        [Test]
        public void TestIsNullOrEmptyWithDataTable()
        {
            // Arrange
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            var row = dt.NewRow();
            row["Name"] = "Test";
            dt.Rows.Add(row);

            // Act
            bool isNullEmptyOrEmpty = dt.IsNullOrEmpty();

            // Assert
            Assert.IsFalse(isNullEmptyOrEmpty);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("Test", dt.Rows[0]["Name"]);
        }

        [Test]
        public void ConvertDataTableToListWithNull()
        {
            // Arrange
            DataTable dt = null;

            // Act
            IList<Model> list = dt.ToList<Model>();

            // Assert
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void ConvertDataTableToListWithEmptyDataTable()
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
        public void ConvertDataTableToListWithDataTable()
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

        private record Model(string Name, string Description);
    }
}
