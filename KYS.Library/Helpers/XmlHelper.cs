using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for the XML.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Beautify the XML string.
        /// </summary>
        /// <param name="xmlDocument">The <see cref="XmlDocument" /> instance this method extends.</param>
        /// <returns>The beautified XML string.</returns>
        public static string Beautify(this XmlDocument xmlDocument)
        {
            using StringWriter sw = new StringWriter();
            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = Formatting.Indented };
            xmlDocument.Save(sw);

            return sw.ToString();
        }

        /// <summary>
        /// Beautify the XML string.
        /// </summary>
        /// <param name="xDocument">The <see cref="XDocument" /> instance this method extends.</param>
        /// <returns>The beautified XML string.</returns>
        public static string Beautify(this XDocument xDocument)
        {
            using StringWriter sw = new StringWriter();
            xDocument.Save(sw);

            return sw.ToString();
        }

        /// <summary>
        /// Serialize an object of type <typeparamref name="T"/> into the XML.
        /// </summary>
        /// <typeparam name="T">The type of <c>obj</c>. Must be a reference type with a public parameterless constructor.</typeparam>
        /// <param name="obj">The value to be serialized.</param>
        /// <param name="formatting">The <see cref="Formatting" /> option.</param>
        /// <returns>Serialized object of type <typeparamref name="T"/> in XML.</returns>
        public static string Serialize<T>(T obj, Formatting formatting = Formatting.Indented)
            where T : class, new()
        {
            using StringWriter sw = new StringWriter();
            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = formatting };

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(sw, obj);

            return sw.ToString();
        }

        /// <summary>
        /// Deserializes an <see cref="XmlDocument"/> into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The target type to deserialize into. Must be a reference type with a public parameterless constructor.
        /// </typeparam>
        /// <param name="xmlDocument">
        /// The XML document containing the serialized representation of the object.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> created from the contents of the XML document.
        /// </returns>
        public static T Deserialize<T>(XmlDocument xmlDocument)
            where T : class, new()
        {
            using TextReader textReader = new StringReader(xmlDocument.OuterXml);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T result = (T)serializer.Deserialize(textReader);

            return result;
        }

        /// <summary>
        /// Deserializes an <see cref="XDocument"/> into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The target type to deserialize into. Must be a reference type with a public parameterless constructor.
        /// </typeparam>
        /// <param name="xDocument">
        /// The XML document containing the serialized representation of the object.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> created from the contents of the XML document.
        /// </returns>
        public static T Deserialize<T>(XDocument xDocument)
            where T : class, new()
        {
            using TextReader textReader = new StringReader(xDocument.ToString());

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T result = (T)serializer.Deserialize(textReader);

            return result;
        }

        /// <summary>
        /// Write <see cref="XmlDocument"/> into file stream.
        /// </summary>
        /// <param name="xmlDocument">The <see cref="XmlDocument" /> instance.</param>
        /// <param name="filePath">The path for the file to be written.</param>
        /// <param name="formatting">The <see cref="Formatting" /> option.</param>
        /// <returns>The <see cref="Stream" /> instance containing the XML file.</returns>
        public static Stream WriteFile(XmlDocument xmlDocument,
            string filePath,
            Formatting formatting = Formatting.Indented)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = formatting };
            xmlDocument.Save(xmlTextWriter);

            return xmlTextWriter.BaseStream;
        }

        /// <summary>
        /// Write <see cref="XDocument"/> into file stream.
        /// </summary>
        /// <param name="xDocument">The <see cref="XDocument" /> instance.</param>
        /// <param name="filePath">The path for the file to be written.</param>
        /// <param name="formatting">The <see cref="Formatting" /> option.</param>
        /// <returns>The <see cref="Stream" /> instance containing the XML file.</returns>
        public static Stream WriteFile(XDocument xDocument,
            string filePath,
            Formatting formatting = Formatting.Indented)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = formatting };
            xDocument.Save(xmlTextWriter);

            return xmlTextWriter.BaseStream;
        }

        /// <summary>
        /// Serialize an object of type <typeparamref name="T"/> into the XML and write it into a file stream.
        /// </summary>
        /// <typeparam name="T">
        /// The target type to deserialize into. Must be a reference type with a public parameterless constructor.
        /// </typeparam>
        /// <param name="obj">The value to be serialized.</param>
        /// <param name="filePath">The path for the file to be written.</param>
        /// <param name="formatting">The <see cref="Formatting" /> option.</param>
        /// <returns>The <see cref="Stream" /> instance containing the XML file.</returns>
        public static Stream WriteFile<T>(T obj,
            string filePath,
            Formatting formatting = Formatting.Indented)
            where T : class, new()
        {
            using StreamWriter sw = new StreamWriter(filePath);
            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = formatting };

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(sw, obj);

            return xmlTextWriter.BaseStream;
        }

        /// <summary>
        /// Convert <see cref="XDocument"/> instance to a <see cref="XmlDocument"/>.
        /// </summary>
        /// <param name="xDocument">The <see cref="XDocument"/> instance this method extends.</param>
        /// <returns>The <see cref="XmlDocument"/> instance created from the <see cref="XDocument"/> contents.</returns>
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            using XmlReader xmlReader = xDocument.CreateReader();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlReader);

            return xmlDocument;
        }

        /// <summary>
        /// Convert <see cref="XmlDocument"/> instance to a <see cref="XDocument"/>.
        /// </summary>
        /// <param name="xmlDocument">The <see cref="XmlDocument"/> instance this method extends.</param>
        /// <returns>The <see cref="XDocument"/> instance created from the <see cref="XmlDocument"/> contents.</returns>
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            return XDocument.Parse(xmlDocument.OuterXml);
        }
    }
}
