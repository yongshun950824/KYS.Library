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
    public static class ExcelHelper
    {
        /// <summary>
        /// Generate Excel file with DataSet.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="excelColumnFormats"></param>
        /// <param name="headerRowStyle"></param>
        /// <param name="summaryRowStyle"></param>
        /// <param name="additionalExcelSheets"></param>
        /// <param name="license"></param>
        /// <returns></returns>
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
        /// Excel file with DataTable.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="excelColumnFormats"></param>
        /// <param name="headerRowStyle"></param>
        /// <param name="summaryRowStyle"></param>
        /// <param name="additionalExcelSheets"></param>
        /// <param name="license"></param>
        /// <returns></returns>
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
                ApplyColumnFormats(worksheet, dt, excelColumnFormats, summaryRowStyle, totalRow, totalColumn, hasHeader);
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

        public class ExcelColumnFormat
        {
            public string ColumnName { get; set; }
            public string DisplayedColumnName { get; set; }
            public string Format { get; set; }
            public ExcelHorizontalAlignment? HorizontalAlignment { get; set; }
            public bool HasSumColumn { get; set; }
        }

        public class AdditionalExcelSheet
        {
            private DataTable _dataTable;
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
            public List<ExcelColumnFormat> ExcelColumnFormats { get; set; } = null;
            public ExcelRowStyle HeaderRowStyle { get; set; } = ExcelRowStyle.DefaultHeaderRowStyle;
            public ExcelRowStyle SummaryRowStyle { get; set; } = null;
        }

        public class ExcelRowStyle
        {
            public ExcelHorizontalAlignment HorizontalAlignment { get; set; } = ExcelHorizontalAlignment.Center;
            public bool Bold { get; set; } = true;
            public Color FontColor { get; set; } = Color.Black;
            public ExcelFillStyle PatternType { get; set; } = ExcelFillStyle.Solid;
            public Color BackgroundColor { get; set; } = Color.Empty;
            /// <summary>
            /// Apply Auto Filter to Excel columns. (For header row only)
            /// </summary>
            public bool AutoFilter { get; set; }

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
