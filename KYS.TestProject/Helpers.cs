using System;
using System.Globalization;
using System.Resources;

namespace KYS.TestProject
{
    public static class Helpers
    {
        public static string GetTranslatedText(string input, Type resourceType, CultureInfo cultureInfo)
        {
            ResourceManager resourceManager = new ResourceManager(resourceType);

            ResourceSet rs = resourceManager.GetResourceSet(cultureInfo, true, false);

            return rs.GetObject(input)?.ToString();
        }
    }
}
