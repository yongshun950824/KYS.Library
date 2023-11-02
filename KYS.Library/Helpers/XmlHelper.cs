using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KYS.Library.Helpers
{
    public static class XmlHelper
    {
        public static string Beautify(this XmlDocument xmlDocument)
        {
            using StringWriter sw = new StringWriter();
            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = Formatting.Indented };
            xmlDocument.Save(sw);

            return sw.ToString();
        }

        public static string Beautify(this XDocument xDocument)
        {
            using StringWriter sw = new StringWriter();
            xDocument.Save(sw);

            return sw.ToString();
        }

        public static string Serialize<T>(T obj, Formatting formatting = Formatting.Indented)
            where T : class, new()
        {
            using StringWriter sw = new StringWriter();

            XmlWriterSettings settings = new XmlWriterSettings();
            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = formatting };

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(sw, obj);

            return sw.ToString();
        }

        public static T Deserialize<T>(XmlDocument xmlDocument)
            where T : class, new()
        {
            using TextReader textReader = new StringReader(xmlDocument.OuterXml);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T result = (T)serializer.Deserialize(textReader);

            return result;
        }

        public static T Deserialize<T>(XDocument xDocument)
            where T : class, new()
        {
            using TextReader textReader = new StringReader(xDocument.ToString());

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T result = (T)serializer.Deserialize(textReader);

            return result;
        }

        public static Stream WriteFile(XmlDocument xmlDocument,
            string filePath,
            Formatting formatting = Formatting.Indented)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = formatting };
            xmlDocument.Save(xmlTextWriter);

            return xmlTextWriter.BaseStream;
        }

        public static Stream WriteFile(XDocument xDocument,
            string filePath,
            Formatting formatting = Formatting.Indented)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = formatting };
            xDocument.Save(xmlTextWriter);

            return xmlTextWriter.BaseStream;
        }

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

        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            using XmlReader xmlReader = xDocument.CreateReader();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlReader);

            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            return XDocument.Parse(xmlDocument.OuterXml);
        }
    }
}
