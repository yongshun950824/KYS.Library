using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CSharpFunctionalExtensions;
using ICSharpCode.SharpZipLib.Zip;
using KYS.Library.Helpers;

namespace KYS.Library.Services
{
    /// <summary>
    /// Define a contract for unzipping item.
    /// </summary>
    public interface IZipExtractor
    {
        /// <summary>
        /// Unzip the zipped file and save in the provided location.
        /// </summary>
        /// <param name="srcFilePath">The source file path of the zipped file.</param>
        /// <param name="destDirPath">The directory path where the unzipped files will be saved.</param>
        /// <returns>
        /// A <see cref="Result"/> that indicates success if the zip file is extracted successfully; 
        /// otherwise, a failed result with an error message describing the failure reason.
        /// </returns>
        Result Unzip(string srcFilePath, string destDirPath);
    }

    public class ZipExtractor : IZipExtractor
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
            private set
            {
                _fileName = value;
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
            private set
            {
                _fileItems = value;
            }
        }

        /// <summary>
        /// Reference: <a href="https://stackoverflow.com/a/22444096/8017690">How to extract a folder from zip file using SharpZipLib?</a>
        /// </summary>
        /// <param name="srcFilePath">The path directory (with the file name) for the zip file located.</param>
        /// <param name="destDirPath">The path directory where the unzipped file element(s) to be placed.</param>
        /// <returns>
        /// A <see cref="Result"/> that indicates success if the zip file is extracted successfully; 
        /// otherwise, a failed result with an error message describing the failure reason.
        /// </returns>
        public Result Unzip(string srcFilePath, string destDirPath)
        {
            if (!File.Exists(srcFilePath))
                return Result.Failure($"File not found: {srcFilePath}");

            FastZip fastZip = new();

            return Result.Try(() =>
            {
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
            },
            ex => $"Failed to unzip file. Error: {ex.Message}");
        }

        #region Helper methods
        [SuppressMessage("Usage", "S3236:Caller information parameters should not be explicitly provided",
            Justification = "Property setters always pass 'value', so nameof(Password) is clearer.")]
        private static void ValidatePassword(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Password));  //NOSONAR
        }
        #endregion
    }
}
