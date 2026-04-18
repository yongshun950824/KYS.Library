using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using NUnit.Framework;

namespace KYS.Library.Tests
{
    public class Assert : NUnit.Framework.Assert
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        /// <summary>
        /// Asserts that the (string) portion is part of (full) string. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="expected">Full string</param>
        /// <param name="actual">Part of string</param>
        public static void StringContains(string expected, string actual)
        {
            That(() => expected.Contains(actual), Is.True, null, null);
        }

        public static void BeEquilaventTo(object expected, object actual)
        {
            That(() => JsonSerializer.Serialize(actual, _serializerOptions),
                Is.EqualTo(JsonSerializer.Serialize(expected, _serializerOptions)).IgnoreCase,
                null,
                null);
        }

        public static void NotEquilaventTo(object expected, object actual)
        {
            That(() => JsonSerializer.Serialize(actual, _serializerOptions),
                Is.Not.EqualTo(JsonSerializer.Serialize(expected, _serializerOptions)).IgnoreCase,
                null,
                null);
        }
    }
}
