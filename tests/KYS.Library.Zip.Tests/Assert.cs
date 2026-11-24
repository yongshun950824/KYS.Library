namespace KYS.Library.Zip.Tests
{
    public class Assert : NUnit.Framework.Assert
    {
        /// <summary>
        /// Asserts that the (string) portion is started with. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="expected">Expected value to be started with</param>
        /// <param name="actual">Actual value to be compared</param>
        public static void StringStartsWith(string expected, string actual)
        {
            That(() => actual.StartsWith(expected), Is.True, null, null);
        }
    }
}
