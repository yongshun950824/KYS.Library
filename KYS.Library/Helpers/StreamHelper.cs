using System.IO;

namespace KYS.Library.Helpers
{
    public static class StreamHelper
    {
        public static Stream ReadStringIntoStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}
