namespace KYS.Library.Helpers
{
    public static class DomainErrors
    {
        public static string Required(string fieldName)
            => $"{fieldName} is required.";

        public static string CannotBeNull(string fieldName)
            => $"{fieldName} cannot be null.";

        public static string FileNotFound(string filePath)
        => $"File not found: {filePath}.";
    }
}
