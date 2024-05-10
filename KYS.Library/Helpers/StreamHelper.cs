using System;
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

        public static string ToBase64(Stream stream)
        {
            byte[] bytes = new byte[(int)stream.Length];

            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);

            return Convert.ToBase64String(bytes);
        }
    }
}
