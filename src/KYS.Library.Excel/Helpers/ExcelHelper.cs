using System.Data;
using CSharpFunctionalExtensions;
using KYS.Library.Excel.Models;
using KYS.Library.Excel.Options;
using KYS.Library.Extensions;
using KYS.Library.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace KYS.Library.Excel.Helpers;

/// <summary>
/// Provide utitlity methods for the Excel file.
/// </summary>
public static class ExcelHelper
{
    /// <summary>
    /// Generate Excel file with <see cref="DataSet" />.
    /// </summary>
    /// <param name="ds">The <see cref="DataSet" /> instance.</param>
    /// <param name="excelColumnFormats">The list of columns with the format to be displayed.</param>
    /// <param name="headerRowStyle">The style of table header.</param>
    /// <param name="summaryRowStyle">The style of table body.</param>
    /// <param name="additionalExcelSheets">Additional sheet(s) to be included.</param>
    /// <returns>A <see cref="Result{T}" /> containing a <see cref="byte[]" /> instance which is the Excel file content.</returns>
    /// <remarks>
    /// EPPlus requires a license to be configured by the consuming application
    /// before use, e.g. <c>ExcelPackage.LicenseContext = LicenseContext.Commercial;</c>
    /// at startup. This library does not set it.
    /// </remarks>
    public static Result<byte[]> CreateExcelBook(DataSet ds,
        List<ExcelColumnFormat> excelColumnFormats = null,
        ExcelRowStyle headerRowStyle = null,
        ExcelRowStyle summaryRowStyle = null,
        List<AdditionalExcelSheet> additionalExcelSheets = null)
    {
        if (ds == null)
            return Result.Failure<byte[]>(DomainErrors.CannotBeNull(nameof(ds)));

        return Result.Try(() =>
            {
                MemoryStream outputStream = new();

                using ExcelPackage package = new(outputStream);
                foreach (DataTable dt in ds.Tables)
                {
                    // Avoid updating original source datatable
                    DataTable copiedDt = dt.Copy();

                    CreateExcelWorksheet(package,
                        copiedDt,
                        excelColumnFormats,
                        headerRowStyle,
                        summaryRowStyle);
                }

                if (!additionalExcelSheets.IsNullOrEmpty())
                {
                    foreach (AdditionalExcelSheet sheet in additionalExcelSheets)
                    {
                        // Avoid updating original source datatable
                        DataTable copiedDt = sheet.DataTable.Copy();

                        CreateExcelWorksheet(package,
                            copiedDt,
                            sheet.ExcelColumnFormats,
                            sheet.HeaderRowStyle,
                            sheet.SummaryRowStyle);
                    }
                }
                package.Save();

                return Result.Success(outputStream.ToArray());
            },
            ex => ex.Message)
        .Bind(result => result);
    }

    /// <summary>
    /// Generate Excel file with <see cref="DataTable" />.
    /// </summary>
    /// <param name="dt">The <see cref="DataTable" /> instance.</param>
    /// <param name="excelColumnFormats">The list of columns with the format to be displayed.</param>
    /// <param name="headerRowStyle">The style of table header.</param>
    /// <param name="summaryRowStyle">The style of table body.</param>
    /// <param name="additionalExcelSheets">Additional sheet(s) to be included.</param>
    /// <returns>A <see cref="Result{T}" /> containing a <see cref="byte[]" /> instance which is the Excel file content.</returns>
    /// <remarks>
    /// EPPlus requires a license to be configured by the consuming application
    /// before use, e.g. <c>ExcelPackage.LicenseContext = LicenseContext.Commercial;</c>
    /// at startup. This library does not set it.
    /// </remarks>
    public static Result<byte[]> CreateExcelBook(DataTable dt,
        List<ExcelColumnFormat> excelColumnFormats = null,
        ExcelRowStyle headerRowStyle = null,
        ExcelRowStyle summaryRowStyle = null,
        List<AdditionalExcelSheet> additionalExcelSheets = null)
    {
        if (dt == null)
            return Result.Failure<byte[]>(DomainErrors.CannotBeNull(nameof(dt)));

        return Result.Try(() =>
            {
                // Avoid updating original source datatable
                DataTable copiedDt = dt.Copy();

                DataSet ds = new();
                ds.Tables.Add(copiedDt);

                return CreateExcelBook(ds,
                    excelColumnFormats,
                    headerRowStyle,
                    summaryRowStyle,
                    additionalExcelSheets);
            },
            ex => ex.Message)
        .Bind(result => result);
    }

    private static void CreateExcelWorksheet(ExcelPackage package,
        DataTable dt,
        List<ExcelColumnFormat> excelColumnFormats = null,
        ExcelRowStyle headerRowStyle = null,
        ExcelRowStyle summaryRowStyle = null)
    {
        bool hasHeader = true;

        headerRowStyle ??= ExcelRowStyle.DefaultHeaderRowStyle;

        #region Rearrange & Rename Data Column
        if (!excelColumnFormats.IsNullOrEmpty())
        {
            DataView dv = dt.DefaultView;
            dt = dv.ToTable(true, excelColumnFormats.Select(x => x.ColumnName).ToArray());

            foreach (DataColumn col in dt.Columns)
            {
                var excelColumnFormat = excelColumnFormats.FirstOrDefault(x => x.ColumnName == col.ColumnName
                    && !String.IsNullOrEmpty(x.DisplayedColumnName));
                if (excelColumnFormat != null)
                {
                    col.ColumnName = excelColumnFormat.DisplayedColumnName;
                }
            }
        }
        #endregion

        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);

        using (var range = worksheet.Cells[1, 1, 1, dt.Columns.Count])
        {
            range.Style.HorizontalAlignment = headerRowStyle.HorizontalAlignment;
            range.Style.Font.Bold = headerRowStyle.Bold;
            range.Style.Font.Color.SetColor(headerRowStyle.FontColor);

            if (headerRowStyle.PatternType != ExcelFillStyle.None)
            {
                range.Style.Fill.PatternType = headerRowStyle.PatternType;
                range.Style.Fill.BackgroundColor.SetColor(headerRowStyle.BackgroundColor);
            }

            range.AutoFilter = headerRowStyle.AutoFilter;
        }

        worksheet.Cells[1, 1].LoadFromDataTable(dt, true);

        int totalRow = dt.Rows.Count;
        int totalColumn = dt.Columns.Count;
        if (totalRow > 0)
        {
            ApplyColumnFormats(worksheet, dt, excelColumnFormats, totalRow, hasHeader);
        }

        #region Styling for summary row
        if (summaryRowStyle != null)
        {
            int summaryRowIndex = CalculateSummaryRowIndex(totalRow, hasHeader);

            worksheet.Cells[summaryRowIndex, 1, summaryRowIndex, totalColumn].Style.Font.Color.SetColor(summaryRowStyle.FontColor);
            worksheet.Cells[summaryRowIndex, 1, summaryRowIndex, totalColumn].Style.Font.Bold = summaryRowStyle.Bold;

            if (summaryRowStyle.PatternType != ExcelFillStyle.None)
            {
                worksheet.Cells[summaryRowIndex, 1, summaryRowIndex, totalColumn].Style.Fill.PatternType = summaryRowStyle.PatternType;
                worksheet.Cells[summaryRowIndex, 1, summaryRowIndex, totalColumn].Style.Fill.BackgroundColor.SetColor(summaryRowStyle.BackgroundColor);
            }
        }
        #endregion

        worksheet.Cells.AutoFitColumns();
    }

    private static void ApplyColumnFormats(ExcelWorksheet worksheet,
        DataTable dt,
        List<ExcelColumnFormat> excelColumnFormats,
        int totalRow,
        bool hasHeader = true)
    {
        if (excelColumnFormats == null || excelColumnFormats.Count == 0)
            return;

        int columnIndex = 1;
        int summaryRowIndex = CalculateSummaryRowIndex(totalRow, hasHeader);
        foreach (var excelColumnFormat in excelColumnFormats)
        {
            if (excelColumnFormat.HasSumColumn)
            {
                string sumColumnName = dt.Columns[columnIndex - 1].ColumnName;
                object total = dt.Compute($"SUM([{sumColumnName}])", null);
                worksheet.Cells[summaryRowIndex, columnIndex].Value = total;
            }

            if (!String.IsNullOrEmpty(excelColumnFormat.Format))
                worksheet.Cells[2, columnIndex, summaryRowIndex, columnIndex].Style.Numberformat.Format = excelColumnFormat.Format;

            if (excelColumnFormat.HorizontalAlignment != null)
                worksheet.Cells[2, columnIndex, summaryRowIndex, columnIndex].Style.HorizontalAlignment = excelColumnFormat.HorizontalAlignment.Value;

            columnIndex++;
        }
    }

    private static int CalculateSummaryRowIndex(int totalRow, bool hasHeader)
    {
        return totalRow + (hasHeader ? 1 : 0) + 1;
    }
}
