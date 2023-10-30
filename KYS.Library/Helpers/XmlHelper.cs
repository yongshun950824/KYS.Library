using System.IO;
using System.Text;
using System.Xml;

namespace KYS.Library.Helpers
{
    public static class XmlHelper
    {
        public static string BeautifyXml(XmlDocument xmlDocument)
        {
            using StringWriter sw = new StringWriter(new StringBuilder());
            using XmlTextWriter xmlTextWriter = new XmlTextWriter(sw) { Formatting = Formatting.Indented };
            xmlDocument.Save(sw);

            return sw.ToString();
        }
    }
}
