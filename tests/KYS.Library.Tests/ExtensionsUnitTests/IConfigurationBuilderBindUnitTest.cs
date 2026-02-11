using KYS.Library.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace KYS.Library.Tests.ExtensionsUnitTests
{
    internal class IConfigurationBuilderBindUnitTest
    {
        private IConfiguration _configuration;

        private const string AppName = "AppName";
        private const string RetryCount = "RetryCount";
        private const string Features = "Features";
        private const string Cultures = "Cultures";
        private const string Users = "Users";

        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }

        [SetUp]
        public void Setup()
        {
            _configuration = LoadConfiguration();
        }

        [Test]
        public void Bind_WithScalarValues_ShouldBindCorrectly()
        {
            // Arrange
            var oldAppName = _configuration[AppName];
            var oldRetryCount = int.Parse(_configuration[RetryCount] ?? "0");

            var configValue = new
            {
                AppName = "NewMyTestApp",
                RetryCount = 3
            };

            // Act
            var config = new ConfigurationBuilder();

            config.Bind(AppName, configValue.AppName);
            config.Bind(RetryCount, configValue.RetryCount);

            var configRoot = config.Build();

            // Assert
            string newAppName = configRoot[AppName];
            int newRetryCount = int.Parse(configRoot[RetryCount] ?? "0");

            Assert.AreNotEqual(oldAppName, newAppName);
            Assert.AreNotEqual(oldRetryCount, newRetryCount);

            Assert.AreEqual(configValue.AppName, newAppName);
            Assert.AreEqual(configValue.RetryCount, newRetryCount);
        }

        [Test]
        public void Bind_WithObject_ShouldBindCorrectly()
        {
            // Arrange
            var oldFeatures = _configuration.GetSection(Features).Get<FeaturesConfig>();

            var configValue = new FeaturesConfig
            {
                EnableLogging = true,
                MaxItems = 10
            };

            // Act
            var config = new ConfigurationBuilder();

            config.Bind(Features, configValue);

            var configRoot = config.Build();

            // Assert
            var newFeatures = configRoot.GetSection(Features).Get<FeaturesConfig>();

            Assert.AreNotEqual(oldFeatures.EnableLogging, newFeatures.EnableLogging);
            Assert.AreNotEqual(oldFeatures.MaxItems, newFeatures.MaxItems);

            Assert.AreEqual(configValue.EnableLogging, newFeatures.EnableLogging);
            Assert.AreEqual(configValue.MaxItems, newFeatures.MaxItems);
        }

        [Test]
        public void Bind_WithArray_ShouldBindCorrectly()
        {
            // Arrange
            var oldCultures = _configuration.GetSection(Cultures).Get<List<string>>();

            var configValue = new string[] { "en-US", "es-ES", "zh-CN" };

            // Act
            var config = new ConfigurationBuilder();

            config.Bind(Cultures, configValue);

            var configRoot = config.Build();

            // Assert
            var newCultures = configRoot.GetSection(Cultures).Get<List<string>>();

            Assert.AreEqual(newCultures[0], configValue[0]);
            Assert.AreEqual(newCultures[1], configValue[1]);
            Assert.AreEqual(newCultures[2], configValue[2]);
        }

        [Test]
        public void Bind_WithArrayOfObjects_ShouldBindCorrectly()
        {
            // Arrange
            var oldUsers = _configuration.GetSection(Users).Get<List<UserConfig>>();

            var configValue = new UserConfig
            {
                Name = "Charles",
                Age = 27
            };

            // Act
            var config = new ConfigurationBuilder();

            oldUsers[1].Name = configValue.Name;
            oldUsers[1].Age = configValue.Age;

            config.Bind(Users, oldUsers);

            var configRoot = config.Build();

            // Assert
            var newUsers = configRoot.GetSection(Users).Get<List<UserConfig>>();

            Assert.AreEqual(oldUsers[0].Name, newUsers[0].Name);
            Assert.AreEqual(oldUsers[0].Age, newUsers[0].Age);

            Assert.AreEqual(configValue.Name, newUsers[1].Name);
            Assert.AreEqual(configValue.Age, newUsers[1].Age);
        }

        [Test]
        public void Bind_WithArrayOfObjectsAndSetEmploymentInfo_ShouldBindCorrectly()
        {
            // Arrange
            var oldUsers = _configuration.GetSection(Users).Get<List<UserConfig>>();

            var configValueForAlice = new EmploymentInfo
            {
                JoinedDate = new DateTime(2025, 3, 1),
                LastDate = new DateTime(2025, 6, 1),
                Department = "HR",
                EmploymentType = EmploymentType.PartTime,
                Position = "Intern",
                Salary = 2000
            };

            var configValueForBob = new EmploymentInfo
            {
                JoinedDate = new DateTime(2025, 1, 1),
                Department = "Tech",
                EmploymentType = EmploymentType.Permanent,
                Position = "Software Dev",
                Salary = 5000
            };

            // Act
            var config = new ConfigurationBuilder();

            oldUsers[0].EmploymentInfo = configValueForAlice;
            oldUsers[1].EmploymentInfo = configValueForBob;

            config.Bind(Users, oldUsers);

            var configRoot = config.Build();

            // Assert
            var newUsers = configRoot.GetSection(Users).Get<List<UserConfig>>();

            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime,
                Culture = CultureInfo.InvariantCulture,
                Converters = { new IsoDateTimeConverter() }
            };
            Assert.AreEqual(
                Helpers.SerializeJson(configValueForAlice),
                Helpers.SerializeJson(newUsers[0].EmploymentInfo));
            Assert.AreEqual(
                Helpers.SerializeJson(configValueForBob),
                Helpers.SerializeJson(newUsers[1].EmploymentInfo));
        }

        [Test]
        public void Bind_WithArrayOfObjectsAndAddContacts_ShouldBindCorrectly()
        {
            // Arrange
            var oldUsers = _configuration.GetSection(Users).Get<List<UserConfig>>();

            var configValue = new Contact
            {
                ContactNumber = "123456",
                ContactType = "Mobile"
            };

            // Act
            var config = new ConfigurationBuilder();

            oldUsers[0].Contacts = new List<Contact>();
            oldUsers[0].Contacts.Add(configValue);

            config.Bind(Users, oldUsers);

            var configRoot = config.Build();

            // Assert
            var newUsers = configRoot.GetSection(Users).Get<List<UserConfig>>();

            Assert.AreEqual(oldUsers[0].Name, newUsers[0].Name);
            Assert.AreEqual(oldUsers[0].Age, newUsers[0].Age);

            Assert.AreEqual(1, newUsers[0].Contacts.Count);
            Assert.AreEqual(configValue.ContactType, newUsers[0].Contacts[0].ContactType);
            Assert.AreEqual(configValue.ContactNumber, newUsers[0].Contacts[0].ContactNumber);
        }

        private class FeaturesConfig
        {
            public bool EnableLogging { get; set; }
            public int MaxItems { get; set; }
        }

        private class UserConfig
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public List<Contact> Contacts { get; set; }
            public EmploymentInfo EmploymentInfo { get; set; }
        }

        private class Contact
        {
            public string ContactNumber { get; set; }
            public string ContactType { get; set; }
        }

        private class EmploymentInfo
        {
            public DateTime JoinedDate { get; set; }
            public DateTime? LastDate { get; set; }
            public EmploymentType EmploymentType { get; set; }
            public decimal Salary { get; set; }
            public string Position { get; set; }
            public string Department { get; set; }
        }

        private enum EmploymentType
        {
            Permanent,
            PartTime,
            Contract
        }
    }
}
