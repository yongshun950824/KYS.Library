using ICSharpCode.SharpZipLib.Zip;
using KYS.Library.Extensions;
using KYS.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace KYS.Library.Services
{
    /// <summary>
    /// Define a contract for zipping/unzipping item.
    /// </summary>
    public interface IZipService
    {
        /// <summary>
        /// Zip item(s).
        /// </summary>
        /// <returns>The byte array containing the zipped file.</returns>
        byte[] Zip();
        /// <summary>
        /// Zip item(s) and save the zipped file in the provided location.
        /// </summary>
        /// <param name="destDirPath">The directory path where the zipped file will be saved.</param>
        void ZipAndToFile(string destDirPath);
        /// <summary>
        /// Unzip the zipped file and save in the provided location.
        /// </summary>
        /// <param name="srcFilePath">The source file path of the zipped file.</param>
        /// <param name="destDirPath">The directory path where the unzipped files will be saved.</param>
        void Unzip(string srcFilePath, string destDirPath);
    };

    /// <summary>
    /// A service for managing zipping and unzipping file.
    /// </summary>
    public class ZipService : IZipService
    {
        private string _fileName;
        private string _password;
        private List<ZipFileItem> _fileItems;

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                ValidateFileName(value);

                if (!value.EndsWith(".zip"))
                    value += ".zip";

                _fileName = value.Replace("/", "_");
            }
        }

        /// <summary>
        /// Gets or sets the password for the zip file.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                ValidatePassword(value);

                _password = value;
            }
        }

        /// <summary>
        /// Gets or sets the item(s) inside the zip file.
        /// </summary>
        public List<ZipFileItem> FileItems
        {
            get { return _fileItems; }
            set
            {
                if (value.IsNullOrEmpty())
                    throw new ArgumentException($"{nameof(FileItems)} must be provided with at least 1 file.");

                _fileItems = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipService"/> class.
        /// </summary>
        public ZipService() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipService"/> class.
        /// </summary>
        /// <param name="fileName">The name for the zip file.</param>
        /// <param name="fileItems">The element(s) that includes in the zip file.</param>
        public ZipService(string fileName, List<ZipFileItem> fileItems)
        {
            FileName = fileName;
            FileItems = fileItems;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipService"/> class.
        /// </summary>
        /// <param name="fileName">The name for the zip file.</param>
        /// <param name="fileItems">The element(s) that includes in the zip file.</param>
        /// <param name="password">The password for the zip file.</param>
        public ZipService(string fileName, List<ZipFileItem> fileItems, string password)
            : this(fileName, fileItems)
        {
            Password = password;
        }

        /// <summary>
        /// Zip file(s) by creating File (<see cref="FileStream" />) and convert into byte array.
        /// </summary>
        /// <returns>A <c>byte[]</c> instance containing the zip file.</returns>
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

                zipOStream.Write(item.Contents, 0, item.Contents.Length);
            }

            zipOStream.Finish();

            MemoryStream ms = new MemoryStream();
            fs.Position = 0;
            fs.CopyTo(ms);

            zipOStream.Close();

            return ms.ToArray();
        }

        /// <summary>
        /// Zip file(s) and write file into provided directory.
        /// </summary>
        /// <param name="destDirPath">The path directory for the zip file to be located.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void ZipAndToFile(string destDirPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(destDirPath);

            MemoryStream ms = new MemoryStream(Zip());
            ms.Seek(0, SeekOrigin.Begin);

            FileHelper.WriteFile(ms, Path.Combine(destDirPath, FileName));
        }

        /// <summary>
        /// Reference: <a href="https://stackoverflow.com/a/22444096/8017690">How to extract a folder from zip file using SharpZipLib?</a>
        /// </summary>
        /// <param name="srcFilePath">The path directory (with the file name) for the zip file located.</param>
        /// <param name="destDirPath">The path directory where the unzipped file element(s) to be placed.</param>
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

        #region Helper methods
        [SuppressMessage("Usage", "S3236:Caller information parameters should not be explicitly provided",
            Justification = "Property setters always pass 'value', so nameof(FileName) is clearer.")]
        private static void ValidateFileName(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(FileName));  //NOSONAR
        }

        [SuppressMessage("Usage", "S3236:Caller information parameters should not be explicitly provided",
            Justification = "Property setters always pass 'value', so nameof(Password) is clearer.")]
        private static void ValidatePassword(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Password));  //NOSONAR
        }
        #endregion

        /// <summary>
        /// Represents the blueprint on the file element to be zipped into a zip file or unzipped from the zip file.
        /// </summary>
        public class ZipFileItem
        {
            private string _name;
            /// <summary>
            /// Gets the file name.
            /// </summary>
            public string Name
            {
                get
                {
                    // Replace name with "/" to "_", "/" will treat as directory
                    return _name.Replace("/", "_");
                }
                set { _name = value; }
            }

            /// <summary>
            /// Gets or sets the file content to be included in the zip file or extracted from the zip file.
            /// </summary>
            public byte[] Contents { get; set; }
        }
    }
}
