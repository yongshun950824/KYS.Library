using KYS.Library.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace KYS.TestProject.HelpersUnitTests
{
    internal class XmlHelperUnitTest
    {
        private string _outputDir;
        private Person _person;
        private string _personXml;
        private XDocument _personXDoc;

        [SetUp]
        public void SetUp()
        {
            _outputDir = Path.Combine(Directory.GetCurrentDirectory(), "OutDir", "XML");
            Directory.CreateDirectory(_outputDir);

            #region Test Data initialization
            _person = new Person
            {
                Name = "Alpha",
                Age = 35,
                Cars = new List<Car>
                {
                    new Car
                    {
                        Brand = "Toyota",
                        Model = "AE 86",
                        VehicleRegistrationNo = "XXX 1"
                    },
                    new Car
                    {
                        Brand = "Proton",
                        Model = "Sage",
                        VehicleRegistrationNo = "XXX 2"
                    }
                }
            };

            _personXDoc = new XDocument(new XDeclaration("1.0", "utf-16", null),
                new XElement("Person",
                    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                    new XElement("Name", "Alpha"),
                    new XElement("Age", 35),
                    new XElement("Cars",
                        new XElement("Car",
                            new XElement("Brand", "Toyota"),
                            new XElement("Model", "AE 86"),
                            new XElement("VehicleRegistrationNo", "XXX 1")
                        ),
                        new XElement("Car",
                            new XElement("Brand", "Proton"),
                            new XElement("Model", "Sage"),
                            new XElement("VehicleRegistrationNo", "XXX 2")
                        )
                    )
                )
            );

            using StringWriter wr = new StringWriter();
            _personXDoc.Save(wr);
            _personXml = wr.ToString();
            #endregion
        }

        [Test]
        public void Beautify_WithXmlDocument_ShouldBeautify()
        {
            // Arrange
            string inputXml = _personXml;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(inputXml);

            // Act
            string xml = xmlDoc.Beautify();

            // Assert
            Assert.IsNotNull(xml);
            Assert.IsNotEmpty(xml);
            Assert.AreEqual(_personXml, xml);
        }

        [Test]
        public void Beautify_WithXDocument_ShouldBeautify()
        {
            // Arrange
            XDocument inputXDoc = _personXDoc;

            // Act
            string xml = inputXDoc.Beautify();

            // Assert
            Assert.IsNotNull(xml);
            Assert.IsNotEmpty(xml);
            Assert.AreEqual(_personXml, xml);
        }

        [Test]
        public void Serialize_ShouldSerializeToXmlString()
        {
            // Arrange
            Person inputObj = _person;

            // Act
            string xml = XmlHelper.Serialize(inputObj);

            // Assert
            Assert.IsNotEmpty(xml);
            Assert.IsNotNull(xml);
            Assert.AreEqual(_personXml, xml);
        }

        [Test]
        public void WriteFile_WithXmlDocument_ShouldGenerateXmlFile()
        {
            // Arrange
            string filePath = Path.Combine(_outputDir, $"Result-{DateTime.Now:yyyyMMddHHmm}.xml");
            XmlDocument inputObj = new XmlDocument();
            inputObj.LoadXml(_personXml);

            // Act
            XmlHelper.WriteFile(inputObj, filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public void WriteFile_WithXDocument_ShouldGenerateXmlFile()
        {
            // Arrange
            string filePath = Path.Combine(_outputDir, $"Result-{DateTime.Now:yyyyMMddHHmm}.xml");
            XDocument inputObj = _personXDoc;

            // Act
            XmlHelper.WriteFile(inputObj, filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public void WriteFile_WithObject_ShouldGenerateXmlFile()
        {
            // Arrange
            string filePath = Path.Combine(_outputDir, $"Result-{DateTime.Now:yyyyMMddHHmm}.xml");
            Person inputObj = _person;

            // Act
            XmlHelper.WriteFile(inputObj, filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public void ToXDocument_FromXmlDocument_ShouldConvertToXDocument()
        {
            // Arrange
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(_personXml);

            // Act
            XDocument xDocument = xmlDocument.ToXDocument();

            // Assert
            Assert.IsNotNull(xDocument);
            Assert.AreEqual(_personXml, xDocument.Beautify());
        }

        [Test]
        public void ToXmlDocument_FromXDocument_ShouldConvertToXmlDocument()
        {
            // Arrange
            XDocument xDocument = _personXDoc;

            // Act
            XmlDocument xmlDocument = xDocument.ToXmlDocument();

            // Assert
            Assert.IsNotNull(xmlDocument);
            Assert.AreEqual(_personXml, xmlDocument.Beautify());
        }

        [Test]
        public void Deserialize_FromXmlDocument_ShouldDeserializeToClass()
        {
            // Arrange
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(_personXml);
            Person expectedResult = _person;

            // Act
            Person result = XmlHelper.Deserialize<Person>(xmlDocument);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Person>(result);
            Assert.AreEqual(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(result));
        }

        [Test]
        public void Deserialize_FromXDocument_ShouldDeserializeToClass()
        {
            // Arrange
            XDocument xDocument = _personXDoc;
            Person expectedResult = _person;

            // Act
            Person result = XmlHelper.Deserialize<Person>(xDocument);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Person>(result);
            Assert.AreEqual(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(result));
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<Car> Cars { get; set; }
    }

    public class Car
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string VehicleRegistrationNo { get; set; }
    }
}
