using NUnit.Framework;
using OfficeOpenXml;

[SetUpFixture]
public class EpplusLicenseSetup
{
    [OneTimeSetUp]
    public void ConfigureLicense()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
}