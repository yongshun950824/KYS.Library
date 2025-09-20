using KYS.Library.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace KYS.TestProject.ExtensionsUnitTests
{
    internal class IListExtensionsUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ConvertListToDataTable()
        {
            // Arrange
            #region Initialize People List
            List<People> inputList = new List<People>
            {
                new People
                (
                    "Lim",
                    2,
                    new DateTime(1990, 1, 1),
                    'M',
                    35,
                    true
                ),
                new People
                (
                    "Mary",
                    null,
                    new DateTime(2005, 2, 2),
                    'F',
                    20,
                    false
                )
            };
            #endregion

            #region Initialize DataTable
            DataTable dt = new DataTable();
            dt.Columns.Add(nameof(People.Name), typeof(string));
            dt.Columns.Add(nameof(People.MortgageNumber), typeof(int));
            dt.Columns.Add(nameof(People.BirthDate), typeof(DateTime));
            dt.Columns.Add(nameof(People.Gender), typeof(char));
            dt.Columns.Add(nameof(People.Age), typeof(int));
            dt.Columns.Add(nameof(People.IsResident), typeof(bool));

            foreach (var people in inputList)
            {
                var row = dt.NewRow();
                row[nameof(People.Name)] = people.Name;
                row[nameof(People.MortgageNumber)] = people.MortgageNumber ?? (object)DBNull.Value;
                row[nameof(People.BirthDate)] = people.BirthDate;
                row[nameof(People.Gender)] = people.Gender;
                row[nameof(People.Age)] = people.Age;
                row[nameof(People.IsResident)] = people.IsResident;

                dt.Rows.Add(row);
            }
            #endregion

            // Act
            DataTable actualDataTable = inputList.ToDataTable();

            // Assert
            Assert.AreEqual(dt.Rows.Count, actualDataTable.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Assert.AreEqual(dt.Rows[i][nameof(People.Name)], 
                    actualDataTable.Rows[i][nameof(People.Name)]);
                Assert.AreEqual(dt.Rows[i][nameof(People.MortgageNumber)] ?? DBNull.Value, 
                    actualDataTable.Rows[i][nameof(People.MortgageNumber)] ?? DBNull.Value);
                Assert.AreEqual(dt.Rows[i][nameof(People.BirthDate)], 
                    actualDataTable.Rows[i][nameof(People.BirthDate)]);
                Assert.AreEqual(dt.Rows[i][nameof(People.Gender)], 
                    actualDataTable.Rows[i][nameof(People.Gender)]);
                Assert.AreEqual(dt.Rows[i][nameof(People.Age)], 
                    actualDataTable.Rows[i][nameof(People.Age)]);
                Assert.AreEqual(dt.Rows[i][nameof(People.IsResident)], 
                    actualDataTable.Rows[i][nameof(People.IsResident)]);
            }
        }

        private record People(string Name, int? MortgageNumber, DateTime BirthDate, char Gender, int Age, bool IsResident);
    }
}
