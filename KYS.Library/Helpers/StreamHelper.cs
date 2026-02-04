using System;
using System.IO;
using System.Threading.Tasks;

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
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Convert byte array to the <see cref="MemoryStream" />.
        /// </summary>
        /// <param name="byteArray">The <c>byte[]</c> instance.</param>
        /// <returns>The <see cref="MemoryStream" /> instance after converting from the byte array.</returns>
        public static MemoryStream ToMemoryStream(byte[] byteArray)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray, 0, byteArray.Length);
            ms.Position = 0;

            return ms;
        }

        /// <summary>
        /// Convert <see cref="MemoryStream" /> to byte array.
        /// </summary>
        /// <param name="ms">The <see cref="MemoryStream" /> instance.</param>
        /// <returns>The <c>byte[]</c> instance after converting from the <see cref="MemoryStream" />.</returns>
        public static byte[] ToByteArray(MemoryStream ms)
        {
            return ms.ToArray();
        }

        /// <summary>
        /// Convert Base64 string to byte array.
        /// </summary>
        /// <param name="base64String">The Base64 string.</param>
        /// <returns><c>byte[]</c> instance.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] ReadBase64StringToByteArray(string base64String)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(base64String);

            byte[] bytes = Convert.FromBase64String(base64String);

            return ToByteArray(ToMemoryStream(bytes));
        }

        /// <summary>
        /// Convert <see cref="Stream" /> to a Base64 string.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> instance.</param>
        /// <returns>The Base64 string after converting from the <see cref="Stream" /> instance.</returns>
        public static async Task<string> ToBase64Async(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            using MemoryStream ms = new();
            await stream.CopyToAsync(ms);

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
