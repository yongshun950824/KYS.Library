using System;
using System.IO;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for read and write file.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Write the content from the <see cref="MemoryStream" /> instance in a new file.
        /// </summary>
        /// <param name="ms">The <see cref="MemoryStream" /> instance containing the file conetnt.</param>
        /// <param name="fileName">The name for the file to be generated.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void WriteFile(MemoryStream ms, string fileName)
        {
            ArgumentNullException.ThrowIfNull(ms);

            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            ms.WriteTo(fs);

            ms.Close();
        }

        /// <summary>
        /// Write the content from the <c>bytes</c> array in a new file.
        /// </summary>
        /// <param name="bytes">The <c>byte[]</c> instance containing the file content.</param>
        /// <param name="fileName">The name for the file to be generated.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void WriteFile(byte[] bytes, string fileName)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            using MemoryStream ms = StreamHelper.ToMemoryStream(bytes);
            WriteFile(ms, fileName);
        }

        /// <summary>
        /// Load the file content into the <see cref="MemoryStream" /> instance.
        /// </summary>
        /// <param name="filePath">The path for the file to be loaded.</param>
        /// <returns>The <see cref="MemoryStream" /> instance containing the file content.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
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
