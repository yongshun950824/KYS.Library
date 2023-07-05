using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;

namespace KYS.Library.Helpers
{
    public static class ZipHelper
    {
        /// <summary>
        /// Zip file(s) and convert into byte array. 
        /// <br /><br />
        /// Reference: <a href="https://stackoverflow.com/a/276347">Create Zip archive from multiple in memory files in C#</a>
        /// </summary>
        /// <param name="fileItems"></param>
        /// <returns></returns>
        public static byte[] Zip(List<ZipFileItem> fileItems)
        {
            using MemoryStream ms = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(ms);

            foreach (var item in fileItems)
            {
                ZipEntry entry = new ZipEntry(item.Name);
                zipOStream.PutNextEntry(entry);

                try
                {
                    zipOStream.Write(item.Contents, 0, item.Contents.Length);
                }
                catch
                {
                    continue;
                }
            }

            zipOStream.Finish();
            zipOStream.Close();

            return ms.ToArray();
        }

        /// <summary>
        /// Zip file(s) by creating File (FileStream) and convert into byte array.
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="fileItems"></param>
        /// <returns></returns>
        public static byte[] Zip(string zipFileName, List<ZipFileItem> fileItems)
        {
            // Replace zipFileName with "/" to "_", "/" will treat as directory
            using FileStream fs = File.Create(zipFileName.Replace("/", "_"));
            ZipOutputStream zipOStream = new ZipOutputStream(fs);

            foreach (var item in fileItems)
            {
                ZipEntry entry = new ZipEntry(item.Name);
                zipOStream.PutNextEntry(entry);

                try
                {
                    zipOStream.Write(item.Contents, 0, item.Contents.Length);
                }
                catch
                {
                    continue;
                }
            }

            zipOStream.Finish();

            MemoryStream ms = new MemoryStream();
            fs.Position = 0;
            fs.CopyTo(ms);

            zipOStream.Close();

            return ms.ToArray();
        }

        public class ZipFileItem
        {
            private string _name;
            public string Name
            {
                get
                {
                    // Replace name with "/" to "_", "/" will treat as directory
                    return _name.Replace("/", "_");
                }
                set { _name = value; }
            }

            public byte[] Contents { get; set; }
        }
    }
}
