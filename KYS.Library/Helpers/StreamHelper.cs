using System;
using System.IO;

namespace KYS.Library.Helpers
{
    public static class StreamHelper
    {
        public static Stream WriteStringIntoStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        public static MemoryStream ToMemoryStream(byte[] byteArray)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray, 0, byteArray.Length);
            ms.Position = 0;

            return ms;
        }

        public static byte[] ToByteArray(MemoryStream ms)
        {
            return ms.ToArray();
        }

        public static byte[] ReadBase64StringToByteArray(string base64String)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(base64String);

            byte[] bytes = Convert.FromBase64String(base64String);

            return ToByteArray(ToMemoryStream(bytes));
        }
    }
}
