using System;
using System.IO;
using CSharpFunctionalExtensions;

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
        /// <returns>A <see cref="Result" /> indicating the success or failure of the operation.</returns>
        public static Result WriteFile(MemoryStream ms, string fileName)
        {
            if (ms == null)
                return Result.Failure(String.Format(DomainErrors.CannotBeNull(nameof(ms))));

            if (String.IsNullOrWhiteSpace(fileName))
                return Result.Failure(String.Format(DomainErrors.Required(nameof(fileName))));

            return Result.Try(() =>
            {
                using FileStream fs = new(fileName, FileMode.Create, FileAccess.Write);
                ms.WriteTo(fs);

                ms.Close();
            },
            ex => ex.Message);
        }

        /// <summary>
        /// Write the content from the <c>bytes</c> array in a new file.
        /// </summary>
        /// <param name="bytes">The <c>byte[]</c> instance containing the file content.</param>
        /// <param name="fileName">The name for the file to be generated.</param>
        /// <returns>A <see cref="Result" /> indicating the success or failure of the operation.</returns>
        public static Result WriteFile(byte[] bytes, string fileName)
        {
            if (bytes == null)
                return Result.Failure(String.Format(DomainErrors.CannotBeNull(nameof(bytes))));

            if (String.IsNullOrWhiteSpace(fileName))
                return Result.Failure(String.Format(DomainErrors.Required(nameof(fileName))));

            return StreamHelper.ToMemoryStream(bytes)
                .Bind(ms => WriteFile(ms, fileName));
        }

        /// <summary>
        /// Load the file content into the <see cref="MemoryStream" /> instance.
        /// </summary>
        /// <param name="filePath">The path for the file to be loaded.</param>
        /// <returns>A <see cref="Result{T}" /> containing the <see cref="MemoryStream" /> instance containing the file content.</returns>
        public static Result<MemoryStream> LoadFileToMemoryStream(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                return Result.Failure<MemoryStream>(String.Format(DomainErrors.Required(nameof(filePath))));

            if (!File.Exists(filePath))
                return Result.Failure<MemoryStream>(String.Format(DomainErrors.FileNotFound(filePath)));

            return Result.Try(
                () => LoadFileToMemoryStreamCore(filePath),
                ex => ex.Message
            );
        }

        internal static MemoryStream LoadFileToMemoryStreamCore(string filePath)
        {
            using FileStream fs = File.OpenRead(filePath);
            fs.Position = 0;

            MemoryStream ms = new();
            fs.CopyTo(ms);

            ms.Position = 0;

            return ms;
        }
    }
}
