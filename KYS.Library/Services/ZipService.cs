using ICSharpCode.SharpZipLib.Zip;
using KYS.Library.Extensions;
using KYS.Library.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KYS.Library.Services
{
    public interface IZipService
    {
        byte[] Zip();
        void ZipAndToFile(string destDirPath);
        void Unzip(string srcFilePath, string destDirPath);
    };

    public class ZipService : IZipService
    {
        private string _fileName;
        private string _password;
        private List<ZipFileItem> _fileItems;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"{nameof(FileName)} must be provided.");
                }

                if (!value.EndsWith(".zip"))
                    value += ".zip";

                _fileName = value.Replace("/", "_");
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"{nameof(Password)} must be provided.");
                }

                _password = value;
            }
        }

        public List<ZipFileItem> FileItems
        {
            get { return _fileItems; }
            set
            {
                if (value.IsNullOrEmpty())
                {
                    throw new ArgumentException($"{nameof(FileItems)} must be provided with at least 1 file.");
                }

                _fileItems = value;
            }
        }

        public ZipService() { }

        public ZipService(string fileName, List<ZipFileItem> fileItems)
        {
            FileName = fileName;
            FileItems = fileItems;
        }

        public ZipService(string fileName, List<ZipFileItem> fileItems, string password)
            : this(fileName, fileItems)
        {
            Password = password;
        }

        /// <summary>
        /// Zip file(s) by creating File (FileStream) and convert into byte array.
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="fileItems"></param>
        /// <returns></returns>
        public byte[] Zip()
        {
            // Replace zipFileName with "/" to "_", "/" will treat as directory
            using FileStream fs = File.Create(FileName);
            ZipOutputStream zipOStream = new ZipOutputStream(fs);

            if (!String.IsNullOrEmpty(Password))
                zipOStream.Password = Password;

            foreach (var item in FileItems)
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

        /// <summary>
        /// Zip file(s) and write to file.
        /// </summary>
        /// <param name="destDirPath"></param>
        public void ZipAndToFile(string destDirPath)
        {
            if (String.IsNullOrEmpty(destDirPath))
            {
                throw new ArgumentException($"{nameof(destDirPath)} must be provided.");
            }

            MemoryStream ms = new MemoryStream(Zip());
            ms.Seek(0, SeekOrigin.Begin);

            FileHelper.WriteFile(ms, Path.Combine(destDirPath, FileName));
        }

        /// <summary>
        /// Reference: <a href="https://stackoverflow.com/a/22444096/8017690">How to extract a folder from zip file using SharpZipLib?</a>
        /// </summary>
        /// <param name="srcFilePath"></param>
        /// <param name="destDirPath"></param>
        public void Unzip(string srcFilePath, string destDirPath)
        {
            if (!File.Exists(srcFilePath))
                throw new FileNotFoundException(srcFilePath);

            FastZip fastZip = new FastZip();
            if (!String.IsNullOrEmpty(Password))
                fastZip.Password = Password;

            Directory.CreateDirectory(destDirPath);

            fastZip.ExtractZip(srcFilePath, destDirPath, null);

            #region Load zip file info
            FileName = Path.GetFileName(srcFilePath);
            FileItems = Directory.GetFiles(destDirPath)
                .Select(x => new ZipFileItem
                {
                    Name = Path.GetFileName(x),
                    Contents = FileHelper.LoadFileToMemoryStream(x).ToArray()
                })
                .ToList();
            #endregion
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
