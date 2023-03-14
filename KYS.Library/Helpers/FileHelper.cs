using System.IO;

namespace KYS.Library.Helpers
{
    public static class FileHelper
    {
        public static MemoryStream ToMemoryStream(byte[] byteArray)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray);
            ms.Position = 0;

            return ms;
        }

        public static byte[] ToByteArray(MemoryStream ms)
        {
            return ms.ToArray();
        }

        public static void WriteFile(MemoryStream ms, string fileName)
        {
            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            ms.WriteTo(fs);

            ms.Close();
        }

        public static void WriteFile(byte[] bytes, string fileName)
        {
            using MemoryStream ms = ToMemoryStream(bytes);
            WriteFile(ms, fileName);
        }
    }
}
