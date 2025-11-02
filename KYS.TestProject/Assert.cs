using NUnit.Framework;

namespace KYS.TestProject
{
    public class Assert : NUnit.Framework.Assert
    {
        /// <summary>
        ///     Asserts that the (string) portion is part of (full) string. Returns without throwing an exception when
        ///     inside a multiple assert block.
        /// </summary>
        /// <param name="arg1">Full string</param>
        /// <param name="arg2">Part of string</param>
        public static void StringContains(string arg1, string arg2)
        {
            That(() => arg1.Contains(arg2), Is.True, null, null);
        }
    }
}
