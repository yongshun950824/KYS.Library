using KYS.Library.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace KYS.Library.Helpers
{
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
        /// <param name="license">The license used in <see cref="ExcelPackage" />.</param>
        /// <returns>The <c>byte[]</c> instance containing the Excel file content.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] CreateExcelBook(DataSet ds,
            List<ExcelColumnFormat> excelColumnFormats = null,
            ExcelRowStyle headerRowStyle = null,
            ExcelRowStyle summaryRowStyle = null,
            List<AdditionalExcelSheet> additionalExcelSheets = null,
            LicenseContext license = LicenseContext.NonCommercial)
        {
            ArgumentNullException.ThrowIfNull(ds);

            MemoryStream outputStream = new MemoryStream();

            ExcelPackage.LicenseContext = license;
            using ExcelPackage package = new ExcelPackage(outputStream);
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

            outputStream.Position = 0;

            return outputStream.ToArray();
        }

        /// <summary>
        /// Generate Excel file with <see cref="DataTable" />.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable" /> instance.</param>
        /// <param name="excelColumnFormats">The list of columns with the format to be displayed.</param>
        /// <param name="headerRowStyle">The style of table header.</param>
        /// <param name="summaryRowStyle">The style of table body.</param>
        /// <param name="additionalExcelSheets">Additional sheet(s) to be included.</param>
        /// <param name="license">The license used in <see cref="ExcelPackage" />.</param>
        /// <returns>The <c>byte[]</c> instance containing the Excel file content.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] CreateExcelBook(DataTable dt,
            List<ExcelColumnFormat> excelColumnFormats = null,
            ExcelRowStyle headerRowStyle = null,
            ExcelRowStyle summaryRowStyle = null,
            List<AdditionalExcelSheet> additionalExcelSheets = null,
            LicenseContext license = LicenseContext.NonCommercial)
        {
            ArgumentNullException.ThrowIfNull(dt);

            // Avoid updating original source datatable
            DataTable copiedDt = dt.Copy();

            DataSet ds = new DataSet();
            ds.Tables.Add(copiedDt);

            return CreateExcelBook(ds,
                excelColumnFormats,
                headerRowStyle,
                summaryRowStyle,
                additionalExcelSheets,
                license);
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

        /// <summary>
        /// Represent the blueprint for customizing the column to be displayed in the Excel sheet.
        /// </summary>
        public class ExcelColumnFormat
        {
            /// <summary>
            /// Gets or sets the column mapped with the column name in the <see cref="DataTable"/>.
            /// </summary>
            public string ColumnName { get; set; }
            /// <summary>
            /// Gets or sets the name for the column to be displayed as the header name in Excel sheet. 
            /// </summary>
            public string DisplayedColumnName { get; set; }
            /// <summary>
            /// Gets or sets the format for the column (value).
            /// </summary>
            public string Format { get; set; }
            /// <summary>
            /// Gets or sets the horizontal alignment of the column.
            /// </summary>
            public ExcelHorizontalAlignment? HorizontalAlignment { get; set; }
            /// <summary>
            /// Gets or sets the indicator to display the sum of the values in the last row. <br />
            /// Only for numeric column used.
            /// </summary>
            public bool HasSumColumn { get; set; }
        }

        /// <summary>
        /// Represents the blueprint for adding and customizing additional sheet(s) to the Excel worksheet.
        /// </summary>
        public class AdditionalExcelSheet
        {
            private DataTable _dataTable;
            /// <summary>
            /// Gets or sets the <see cref="DataTable"/> instance. 
            /// </summary>
            public DataTable DataTable
            {
                get
                {
                    return _dataTable;
                }
                set
                {
                    ArgumentNullException.ThrowIfNull(value);

                    _dataTable = value;
                }
            }
            /// <summary>
            /// Gets or sets the column(s) and its customization.
            /// </summary>
            public List<ExcelColumnFormat> ExcelColumnFormats { get; set; } = null;
            /// <summary>
            /// Gets or sets the header row style.
            /// </summary>
            public ExcelRowStyle HeaderRowStyle { get; set; } = ExcelRowStyle.DefaultHeaderRowStyle;
            /// <summary>
            /// Gets or sets the summary (bottom) row style.
            /// </summary>
            public ExcelRowStyle SummaryRowStyle { get; set; } = null;
        }

        /// <summary>
        /// Represents the blueprint for customizing the style for whole row.
        /// </summary>
        public class ExcelRowStyle
        {
            /// <summary>
            /// Gets or sets the horizontal alignment.
            /// </summary>
            public ExcelHorizontalAlignment HorizontalAlignment { get; set; } = ExcelHorizontalAlignment.Center;
            /// <summary>
            /// Gets or sets the indicator for applying the bold style.
            /// </summary>
            public bool Bold { get; set; } = true;
            /// <summary>
            /// Gets or sets the font color.
            /// </summary>
            public Color FontColor { get; set; } = Color.Black;
            /// <summary>
            /// Gets or sets the pattern type.
            /// </summary>
            public ExcelFillStyle PatternType { get; set; } = ExcelFillStyle.Solid;
            /// <summary>
            /// Gets or sets the background color.
            /// </summary>
            public Color BackgroundColor { get; set; } = Color.Empty;
            /// <summary>
            /// Apply Auto Filter to Excel columns. (For header row only)
            /// </summary>
            public bool AutoFilter { get; set; }

            /// <summary>
            /// Gets the default header row style instance.
            /// </summary>
            public static ExcelRowStyle DefaultHeaderRowStyle
            {
                get
                {
                    return new ExcelRowStyle
                    {
                        HorizontalAlignment = ExcelHorizontalAlignment.Center,
                        Bold = true,
                        FontColor = Color.White,
                        PatternType = ExcelFillStyle.Solid,
                        BackgroundColor = ColorTranslator.FromHtml("#4472C4")
                    };
                }
            }

            /// <summary>
            /// Gets the default summary (bottom) row style instance.
            /// </summary>
            public static ExcelRowStyle DefaultSummaryRowStyle
            {
                get
                {
                    return new ExcelRowStyle
                    {
                        HorizontalAlignment = ExcelHorizontalAlignment.Right,
                        Bold = true,
                        FontColor = Color.Black,
                        PatternType = ExcelFillStyle.Solid,
                        BackgroundColor = Color.LightGray
                    };
                }
            }
        }
    }
}
