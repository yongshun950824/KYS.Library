using NUnit.Framework;
using OfficeOpenXml;

namespace KYS.Library.Excel.Tests
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