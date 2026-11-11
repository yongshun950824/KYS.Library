using System.Data;
using CSharpFunctionalExtensions;
using KYS.Library.Excel.Options;

namespace KYS.Library.Excel.Helpers;

/// <summary>
/// Provide utility methods for <see cref="DataTable" />.
/// </summary>
public static class ExcelDataTableHelper
{
    /// <summary>
    /// Write <see cref="DataTable" /> into Excel file.
    /// </summary>
    /// <param name="dt">The <see cref="DataTable" /> instance.</param>
    /// <param name="headerRowStyle">The style for the header row.</param>
    /// <returns>A <see cref="Result{T}" /> containing the <see cref="byte[]" /> instance which is the Excel file content.</returns>
    /// <remarks>
    /// EPPlus requires a license to be configured by the consuming application
    /// before use, e.g. <c>ExcelPackage.LicenseContext = LicenseContext.Commercial;</c>
    /// at startup. This library does not set it.
    /// </remarks>
    public static Result<byte[]> WriteToExcelFile(DataTable dt,
        ExcelRowStyle headerRowStyle = null,
        List<ExcelColumnFormat> excelColumnFormats = null)
    {
        return ExcelHelper.CreateExcelBook(dt, excelColumnFormats, headerRowStyle);
    }
}
