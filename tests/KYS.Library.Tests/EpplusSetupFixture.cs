using NUnit.Framework;
using OfficeOpenXml;

namespace KYS.Library.Tests
{
    [SetUpFixture]
    public class EpplusLicenseSetup
    {
        [OneTimeSetUp]
        public void ConfigureLicense()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
    }
}