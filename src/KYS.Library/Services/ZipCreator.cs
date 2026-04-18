using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CSharpFunctionalExtensions;
using ICSharpCode.SharpZipLib.Zip;
using KYS.Library.Extensions;
using KYS.Library.Helpers;

namespace KYS.Library.Services
{
    /// <summary>
    /// Define a contract for zipping item.
    /// </summary>
    public interface IZipCreator
    {
        /// <summary>
        /// Zip item(s).
        /// </summary>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the byte array of the created zip file if successful; 
        /// otherwise, a failed result with an error message.
        /// </returns>
        Result<byte[]> Zip();

        /// <summary>
        /// Zip item(s) and save the zipped file in the provided location.
        /// </summary>
        /// <param name="destDirPath">The directory path where the zipped file will be saved.</param>
        /// <returns>
        /// A <see cref="Result"/> that indicates success if the zip file is created and saved successfully; 
        /// otherwise, a failed result with an error message describing the failure reason.
        /// </returns>
        Result ZipAndToFile(string destDirPath);
    }

    /// <summary>
    /// A service for managing zipping and unzipping file.
    /// </summary>
    public class ZipCreator : IZipCreator
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

                // Replace zipFileName with "/" to "_", "/" will treat as directory
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
        /// Initializes a new instance of the <see cref="ZipCreator"/> class.
        /// </summary>
        public ZipCreator() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipCreator"/> class.
        /// </summary>
        /// <param name="fileName">The name for the zip file.</param>
        /// <param name="fileItems">The element(s) that includes in the zip file.</param>
        public ZipCreator(string fileName, List<ZipFileItem> fileItems)
        {
            FileName = fileName;
            FileItems = fileItems;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipCreator"/> class.
        /// </summary>
        /// <param name="fileName">The name for the zip file.</param>
        /// <param name="fileItems">The element(s) that includes in the zip file.</param>
        /// <param name="password">The password for the zip file.</param>
        public ZipCreator(string fileName, List<ZipFileItem> fileItems, string password)
            : this(fileName, fileItems)
        {
            Password = password;
        }

        /// <summary>
        /// Creates a zip file in memory containing all items from <see cref="FileItems"/> with optional password protection.
        /// </summary>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the byte array of the created zip file if successful; 
        /// otherwise, a failed result with an error message.
        /// </returns>
        public Result<byte[]> Zip()
        {
            using FileStream fs = File.Create(FileName);
            ZipOutputStream zipOStream = new(fs);

            if (!String.IsNullOrEmpty(Password))
                zipOStream.Password = Password;

            foreach (var item in FileItems)
            {
                ZipEntry entry = new(item.Name);
                zipOStream.PutNextEntry(entry);

                zipOStream.Write(item.Contents, 0, item.Contents.Length);
            }

            zipOStream.Finish();

            MemoryStream ms = new();
            fs.Position = 0;
            fs.CopyTo(ms);

            zipOStream.Close();

            return Result.Success(ms.ToArray());
        }

        /// <summary>
        /// Creates a zip file from all items in <see cref="FileItems"/> and saves it to the specified directory.
        /// </summary>
        /// <param name="destDirPath">
        /// The directory path where the zip file will be saved. The file name is determined by <see cref="FileName"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Result"/> that indicates success if the zip file is created and saved successfully; 
        /// otherwise, a failed result with an error message describing the failure reason.
        /// </returns>
        public Result ZipAndToFile(string destDirPath)
        {
            if (String.IsNullOrWhiteSpace(destDirPath))
                return Result.Failure($"{nameof(destDirPath)} must be provided.");

            return Zip()
                .Bind(zipBytes =>
                {
                    MemoryStream ms = new(zipBytes);
                    ms.Seek(0, SeekOrigin.Begin);

                    FileHelper.WriteFile(ms, Path.Combine(destDirPath, FileName));

                    return Result.Success();
                });
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
    }

    /// <summary>
    /// Represents the blueprint on the file element to be zipped into a zip file or unzipped from the zip file.
    /// </summary>
    public sealed class ZipFileItem
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
