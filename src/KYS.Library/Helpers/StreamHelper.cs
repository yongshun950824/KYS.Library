using System;
using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for <see cref="Stream" />.
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// Write <see cref="string" /> value into the <see cref="Stream" />.
        /// </summary>
        /// <param name="s">The <see cref="string" /> value.</param>
        /// <returns>The <see cref="Stream" /> instance containing the <c>value</c>.</returns>
        public static Stream WriteStringIntoStream(string s)
        {
            MemoryStream stream = new();
            StreamWriter writer = new(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Convert byte array to the <see cref="MemoryStream" />.
        /// </summary>
        /// <param name="byteArray">The <see cref="byte[]" /> instance.</param>
        /// <returns>A <see cref="Result{T}" /> containing the <see cref="MemoryStream" /> instance after converting from the byte array.</returns>
        public static Result<MemoryStream> ToMemoryStream(byte[] byteArray)
        {
            if (byteArray == null)
                return Result.Failure<MemoryStream>(DomainErrors.CannotBeNull(nameof(byteArray)));

            MemoryStream ms = new();
            ms.Write(byteArray, 0, byteArray.Length);
            ms.Position = 0;

            return ms;
        }

        /// <summary>
        /// Convert <see cref="MemoryStream" /> to byte array.
        /// </summary>
        /// <param name="ms">The <see cref="MemoryStream" /> instance.</param>
        /// <returns>The <see cref="byte[]" /> instance after converting from the <see cref="MemoryStream" />.</returns>
        public static byte[] ToByteArray(MemoryStream ms)
        {
            return ms.ToArray();
        }

        /// <summary>
        /// Convert Base64 string to byte array.
        /// </summary>
        /// <param name="base64String">The Base64 string.</param>
        /// <returns>A <see cref="Result{T}" /> containing the <see cref="byte[]" /> instance.</returns>
        public static Result<byte[]> ReadBase64StringToByteArray(string base64String)
        {
            if (String.IsNullOrWhiteSpace(base64String))
                return Result.Failure<byte[]>(String.Format(DomainErrors.Required(nameof(base64String))));

            return Result.Try(
                    () => Convert.FromBase64String(base64String),
                    ex => ex.Message
                )
                .Bind(ToMemoryStream)
                .Map(ToByteArray);
        }

        /// <summary>
        /// Convert <see cref="Stream" /> to a Base64 string.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> instance.</param>
        /// <returns>A <see cref="Result{T}" /> containing the Base64 string.</returns>
        public static async Task<Result<string>> ToBase64Async(Stream stream)
        {
            if (stream == null)
                return Result.Failure<string>(DomainErrors.CannotBeNull(nameof(stream)));

            using MemoryStream ms = new();
            await stream.CopyToAsync(ms);

            return Result.Success(ms.ToArray())
                .Map(bytes => Convert.ToBase64String(bytes));
        }
    }
}
