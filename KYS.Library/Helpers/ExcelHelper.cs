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
        /// <returns></returns>
        public static byte[] CreateExcelBook(DataSet ds,
            List<ExcelColumnFormat> excelColumnFormats = null,
            ExcelRowStyle headerRowStyle = null,
            ExcelRowStyle summaryRowStyle = null,
            List<AdditionalExcelSheet> additionalExcelSheets = null)
        {
            MemoryStream outputStream = new MemoryStream();

            using ExcelPackage package = new ExcelPackage(outputStream);
            foreach (DataTable dt in ds.Tables)
            {
                CreateExcelWorksheet(package,
                    dt,
                    excelColumnFormats,
                    headerRowStyle,
                    summaryRowStyle);
            }

            if (!additionalExcelSheets.IsNullOrEmpty())
            {
                foreach (AdditionalExcelSheet sheet in additionalExcelSheets)
                {
                    CreateExcelWorksheet(package,
                        sheet.DataTable,
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
        /// <returns></returns>
        public static byte[] CreateExcelBook(DataTable dt,
            List<ExcelColumnFormat> excelColumnFormats = null,
            ExcelRowStyle headerRowStyle = null,
            ExcelRowStyle summaryRowStyle = null,
            List<AdditionalExcelSheet> additionalExcelSheets = null)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return CreateExcelBook(ds,
                excelColumnFormats,
                headerRowStyle,
                summaryRowStyle,
                additionalExcelSheets);
        }

        private static void CreateExcelWorksheet(ExcelPackage package,
            DataTable dt,
            List<ExcelColumnFormat> excelColumnFormats = null,
            ExcelRowStyle headerRowStyle = null,
            ExcelRowStyle summaryRowStyle = null)
        {
            headerRowStyle ??= ExcelRowStyle.DefaultHeaderRowStyle;
            summaryRowStyle ??= ExcelRowStyle.DefaultSummaryRowStyle;

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
                ApplyColumnFormats(worksheet, dt, excelColumnFormats, summaryRowStyle, totalRow, totalColumn);
            }

            worksheet.Cells.AutoFitColumns();
        }

        private static void ApplyColumnFormats(ExcelWorksheet worksheet,
            DataTable dt,
            List<ExcelColumnFormat> excelColumnFormats,
            ExcelRowStyle summaryRowStyle,
            int totalRow,
            int totalColumn)
        {
            if (excelColumnFormats == null || excelColumnFormats.Count == 0)
                return;

            int columnIndex = 1;
            foreach (var excelColumnFormat in excelColumnFormats)
            {
                int totalRowIndex = totalRow + 1;
                if (excelColumnFormat.HasSumColumn)
                {
                    totalRowIndex += 1;
                    string sumColumnName = dt.Columns[columnIndex - 1].ColumnName;
                    object total = dt.Compute($"SUM([{sumColumnName}])", null);
                    worksheet.Cells[totalRowIndex, columnIndex].Value = total;

                    #region Styling for summary row
                    worksheet.Cells[totalRowIndex, 1, totalRowIndex, totalColumn].Style.Font.Color.SetColor(summaryRowStyle.FontColor);
                    worksheet.Cells[totalRowIndex, 1, totalRowIndex, totalColumn].Style.Font.Bold = summaryRowStyle.Bold;

                    if (summaryRowStyle.PatternType != ExcelFillStyle.None)
                    {
                        worksheet.Cells[totalRowIndex, 1, totalRowIndex, totalColumn].Style.Fill.PatternType = summaryRowStyle.PatternType;
                        worksheet.Cells[totalRowIndex, 1, totalRowIndex, totalColumn].Style.Fill.BackgroundColor.SetColor(summaryRowStyle.BackgroundColor);
                    }
                    #endregion
                }

                if (!String.IsNullOrEmpty(excelColumnFormat.Format))
                    worksheet.Cells[2, columnIndex, totalRowIndex, columnIndex].Style.Numberformat.Format = excelColumnFormat.Format;

                if (excelColumnFormat.HorizontalAlignment != null)
                    worksheet.Cells[2, columnIndex, totalRowIndex, columnIndex].Style.HorizontalAlignment = excelColumnFormat.HorizontalAlignment.Value;

                columnIndex++;
            }
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
            public DataTable DataTable { get; set; }
            public List<ExcelColumnFormat> ExcelColumnFormats { get; set; } = null;
            public ExcelRowStyle HeaderRowStyle { get; set; } = ExcelRowStyle.DefaultHeaderRowStyle;
            public ExcelRowStyle SummaryRowStyle { get; set; } = ExcelRowStyle.DefaultSummaryRowStyle;
        }

        public class ExcelRowStyle
        {
            public ExcelHorizontalAlignment HorizontalAlignment { get; set; } = ExcelHorizontalAlignment.Center;
            public bool Bold { get; set; } = true;
            public Color FontColor { get; set; } = Color.Black;
            public ExcelFillStyle PatternType { get; set; } = ExcelFillStyle.None;
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
