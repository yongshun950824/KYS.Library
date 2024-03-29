﻿using System;
using System.IO;

namespace KYS.Library.Helpers
{
    public static class FileHelper
    {
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
            byte[] bytes = Convert.FromBase64String(base64String);

            return ToByteArray(ToMemoryStream(bytes));
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

        public static MemoryStream LoadFileToMemoryStream(string filePath)
        {
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
