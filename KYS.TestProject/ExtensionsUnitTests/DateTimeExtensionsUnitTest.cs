using KYS.Library.Extensions;
using NUnit.Framework;
using System;

namespace KYS.TestProject.ExtensionsUnitTests
{
    internal class DateTimeExtensionsUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GenerateDateInIso8601Format()
        {
            // Arrange
            DateTime date = new DateTime(2025, 09, 10, 10, 50, 00, DateTimeKind.Unspecified);
            string expectedValue = "2025-09-10T10:50:00.0000000";

            // Act
            string actualValue = date.ToIso8601String();

            // Assert
            Assert.That(expectedValue, Is.EqualTo(actualValue));
        }

        [Test]
        public void GetFirstDateOfMonth()
        {
            // Arrange
            DateTime date = new DateTime(2025, 09, 10, 10, 50, 00, DateTimeKind.Unspecified);
            DateTime expectedFirstDateOfMonth = new DateTime(date.Year, date.Month, 1);

            // Act
            DateTime actualFirstDateOfMonth = date.GetFirstDateOfMonth();

            // Assert
            Assert.That(expectedFirstDateOfMonth, Is.EqualTo(actualFirstDateOfMonth));
        }

        [Test]
        public void GetLastDateOfMonth()
        {
            // Arrange
            DateTime dateForJan = new DateTime(2025, 01, 01, 00, 00, 00, DateTimeKind.Unspecified);
            DateTime dateForFeb = new DateTime(2025, 02, 05, 00, 00, 00, DateTimeKind.Unspecified);
            DateTime dateForSep = new DateTime(2025, 09, 21, 00, 00, 00, DateTimeKind.Unspecified);

            DateTime expectedLastDateOfMonthForJan = new DateTime(dateForJan.Year, dateForJan.Month, 31);
            DateTime expectedLastDateOfMonthForFeb = new DateTime(dateForFeb.Year, dateForFeb.Month, 28);
            DateTime expectedLastDateOfMonthForSep = new DateTime(dateForSep.Year, dateForSep.Month, 30);

            // Act
            DateTime actualLastDateOfMonthForJan = dateForJan.GetLastDateOfMonth();
            DateTime actualLastDateOfMonthForFeb = dateForFeb.GetLastDateOfMonth();
            DateTime actualLastDateOfMonthForSep = dateForSep.GetLastDateOfMonth();

            // Assert
            Assert.That(expectedLastDateOfMonthForJan, Is.EqualTo(actualLastDateOfMonthForJan));
            Assert.That(expectedLastDateOfMonthForFeb, Is.EqualTo(actualLastDateOfMonthForFeb));
            Assert.That(expectedLastDateOfMonthForSep, Is.EqualTo(actualLastDateOfMonthForSep));
        }

        [Test]
        public void GetFirstDateOfYear()
        {
            // Arrange
            DateTime date = new DateTime(2025, 09, 10, 10, 50, 00, DateTimeKind.Unspecified);
            DateTime expectedFirstDateOfYear = new DateTime(date.Year, 1, 1);

            // Act
            DateTime actualFirstDateOfYear = date.GetFirstDateOfYear();

            // Assert
            Assert.That(expectedFirstDateOfYear, Is.EqualTo(actualFirstDateOfYear));
        }

        [Test]
        public void GetLastDateOfYear()
        {
            // Arrange
            DateTime date = new DateTime(2025, 09, 10, 10, 50, 00, DateTimeKind.Unspecified);
            DateTime expectedLastDateOfYear = new DateTime(date.Year, 12, 31);

            // Act
            DateTime actualLastDateOfYear = date.GetLastDateOfYear();

            // Assert
            Assert.That(expectedLastDateOfYear, Is.EqualTo(actualLastDateOfYear));
        }

        [Test]
        public void GetThaiBuddhistDate()
        {
            // Arrange
            DateTime date = new DateTime(2025, 09, 21, 00, 00, 00, DateTimeKind.Unspecified);
            DateTime expectedThaiBuddhistDate = new DateTime(2568, 9, 21);

            // Act
            DateTime actualThaiBuddhistDate = date.ConvertToThaiBuddhistDateTime();

            // Assert
            Assert.That(expectedThaiBuddhistDate, Is.EqualTo(actualThaiBuddhistDate));
        }

        [Test]
        public void GetCurrentAgeFromToday()
        {
            // Arrange
            DateTime dob = new DateTime(1990, 09, 21, 00, 00, 00, DateTimeKind.Unspecified);
            int expectedAge = (int)Math.Truncate((DateTime.Now - dob).TotalDays / 365);

            // Act
            int actualAge = dob.GetAge();

            // Assert
            Assert.That(expectedAge, Is.EqualTo(actualAge));
        }

        [Test]
        public void GetCurrentAgeFromCalculatedDate()
        {
            // Arrange
            DateTime calculatedDate = new DateTime(2025, 01, 01, 00, 00, 00, DateTimeKind.Unspecified);
            DateTime dob = new DateTime(1990, 09, 21, 00, 00, 00, DateTimeKind.Unspecified);
            int expectedAge = (int)Math.Truncate((calculatedDate - dob).TotalDays / 365);

            // Act
            int actualAge = dob.GetAge(calculatedDate);

            // Assert
            Assert.That(expectedAge, Is.EqualTo(actualAge));
        }
    }
}
