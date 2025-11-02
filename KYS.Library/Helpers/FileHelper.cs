using System;
using System.IO;

namespace KYS.Library.Helpers
{
    public static class FileHelper
    {
        public static void WriteFile(MemoryStream ms, string fileName)
        {
            ArgumentNullException.ThrowIfNull(ms);

            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            ms.WriteTo(fs);

            ms.Close();
        }

        public static void WriteFile(byte[] bytes, string fileName)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            using MemoryStream ms = StreamHelper.ToMemoryStream(bytes);
            WriteFile(ms, fileName);
        }

        public static MemoryStream LoadFileToMemoryStream(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            using FileStream fs = File.OpenRead(filePath);
            fs.Position = 0;

            MemoryStream ms = new MemoryStream();
            fs.CopyTo(ms);

            ms.Position = 0;

            return ms;
        }
    }
}
