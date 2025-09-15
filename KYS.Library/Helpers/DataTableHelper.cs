using CsvHelper;
using CsvHelper.Configuration;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using KYS.Library.Extensions;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace KYS.Library.Helpers
{
    public static class DataTableHelper
    {
        /// <summary>
        /// Write DataTable into text/CSV file.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="printHeaders"></param>
        /// <param name="delimiter"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static byte[] WriteToTextFile(DataTable dt,
            bool printHeaders = true,
            string delimiter = ";",
            CultureInfo cultureInfo = null)
        {
            using MemoryStream ms = new MemoryStream();
            using StreamWriter writer = new StreamWriter(ms);

            string csvString = null;

            if (!dt.IsNullOrEmpty())
                csvString = DataTableToCSV(dt, printHeaders, delimiter, cultureInfo);

            writer.Write(csvString);

            writer.Flush();

            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Write DataTable into Excel file.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheetName"></param>
        /// <param name="printHeaders"></param>
        /// <param name="headerStyle"></param>
        /// <param name="license"></param>
        /// <returns></returns>
        public static byte[] WriteToExcelFile(DataTable dt,
            string sheetName,
            bool printHeaders = true,
            ExcelHeaderStyle headerStyle = default,
            Dictionary<string, string> columnNameDict = null,
            List<ExcelColumnFormat> excelColumnFormats = null,
            LicenseContext license = LicenseContext.NonCommercial)
        {
            ExcelPackage.LicenseContext = license;

            const int START_ROW = 1;
            const int START_COL = 1;

            RenameDataTableHeaderColumn(dt, columnNameDict);

            using MemoryStream ms = new MemoryStream();
            using ExcelPackage package = new ExcelPackage(ms);

            ExcelWorksheet sheet = package.Workbook.Worksheets.Add(sheetName);

            #region Set header style
            if (headerStyle == null)
                headerStyle = new ExcelHeaderStyle();

            using var range = sheet.Cells[START_ROW, START_COL, START_ROW, dt.Columns.Count];
            range.Style.Font.Bold = headerStyle.Bold;
            range.Style.Fill.PatternType = headerStyle.PatternType;
            range.Style.Fill.BackgroundColor.SetColor(headerStyle.BackgroundColor);
            #endregion

            sheet.Cells[START_ROW, START_COL].LoadFromDataTable(dt, printHeaders);

            #region Set column format
            int totalRow = dt.Rows.Count;
            if (!excelColumnFormats.IsNullOrEmpty()
                && totalRow > 0)
            {
                foreach (var excelColumnFormat in excelColumnFormats)
                {
                    foreach (int column in excelColumnFormat.Columns)
                    {
                        if (!String.IsNullOrEmpty(excelColumnFormat.Format))
                            sheet.Cells[2, column, totalRow + 1, column].Style.Numberformat.Format = excelColumnFormat.Format;

                        if (excelColumnFormat.HorizontalAlignment != null)
                            sheet.Cells[2, column, totalRow + 1, column].Style.HorizontalAlignment = excelColumnFormat.HorizontalAlignment.Value;
                    }
                }
            }
            #endregion

            package.Save();

            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Write DataTable into PDF file.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="printHeaders"></param>
        /// <param name="tableHeaderStyle"></param>
        /// <param name="tableBodyStyle"></param>
        /// <returns></returns>
        public static byte[] WriteToPdfFile(DataTable dt,
            bool printHeaders = true,
            Style tableHeaderStyle = null,
            Style tableBodyStyle = null)
        {
            using MemoryStream ms = new MemoryStream();
            using PdfWriter pdfWriter = new PdfWriter(ms);

            PdfDocument pdfDoc = new PdfDocument(pdfWriter);
            Document document = new Document(pdfDoc);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            document.SetFont(font)
                .SetFontSize(10);

            #region Sample Header style
            if (tableHeaderStyle == null)
                tableHeaderStyle = new Style()
                    .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER);
            #endregion

            if (!dt.IsNullOrEmpty())
                document.Add(DataTableToPdfTable(dt, printHeaders, tableHeaderStyle, tableBodyStyle));

            document.Close();

            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Write DataTable into JSON file.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="isIndented"></param>
        /// <returns></returns>
        public static byte[] WriteToJsonFile(DataTable dt, bool isIndented = true)
        {
            Formatting formatting = isIndented switch
            {
                true => Formatting.Indented,
                _ => Formatting.None
            };

            string jsonString = JsonConvert.SerializeObject(dt, formatting);

            Stream stream = StreamHelper.ReadStringIntoStream(jsonString);
            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);

            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Read CSV from file into DataTable.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataTable ReadCSV(string filePath)
        {
            using StreamReader sr = new StreamReader(filePath);

            return ReadCSV(sr);
        }

        /// <summary>
        /// Read CSV from Stream into DataTable.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataTable ReadCSV(Stream stream)
        {
            using StreamReader sr = new StreamReader(stream);

            return ReadCSV(sr);
        }

        private static DataTable ReadCSV(StreamReader sr)
        {
            DataTable dt = new DataTable();

            string[] headers = sr.ReadLine().Split(',');
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }

            while (!sr.EndOfStream)
            {
                string[] cols = sr.ReadLine().Split(",");
                DataRow dtRow = dt.NewRow();

                for (int i = 0; i < headers.Length; i++)
                {
                    dtRow[i] = cols[i];
                }

                dt.Rows.Add(dtRow);
            }

            return dt;
        }

        private static string DataTableToCSV(DataTable dt,
            bool printHeaders = true,
            string delimiter = ";",
            CultureInfo cultureInfo = null)
        {
            StringWriter sw = new StringWriter();

            CsvConfiguration config = new CsvConfiguration(cultureInfo ?? CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter
            };

            using var csvWriter = new CsvWriter(sw, config);
            if (printHeaders)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    csvWriter.WriteField(column.ColumnName);
                }

                csvWriter.NextRecord();
            }

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    csvWriter.WriteField(row[i]);
                }

                csvWriter.NextRecord();
            }

            return sw.ToString()
                .Replace("\"", "");
        }

        private static Table DataTableToPdfTable(DataTable dt,
            bool printHeaders = true,
            Style headerStyle = null,
            Style bodyStyle = null)
        {
            Table table = new Table(dt.Columns.Count)
                .SetWidth(UnitValue.CreatePercentValue(100));

            #region Table Headers
            List<string> columnNames = dt.Columns
                .OfType<DataColumn>()
                .Select(x => x.ColumnName)
                .ToList();

            if (printHeaders)
            {
                foreach (string columnName in columnNames)
                {
                    var cell = new Cell()
                        .Add(new Paragraph(columnName));

                    if (headerStyle != null)
                        cell.AddStyle(headerStyle);

                    table.AddHeaderCell(cell);
                }
            }
            #endregion

            #region Table Body
            foreach (DataRow dtRow in dt.Rows)
            {
                foreach (string columnName in columnNames)
                {
                    var cell = new Cell()
                        .Add(new Paragraph(dtRow[columnName]?.ToString()));

                    if (bodyStyle != null)
                        cell.AddStyle(bodyStyle);

                    table.AddCell(cell);
                }
            }
            #endregion

            return table;
        }

        private static void RenameDataTableHeaderColumn(DataTable dt, Dictionary<string, string> columnNameDict)
        {
            if (columnNameDict == null)
                return;

            foreach (DataColumn col in dt.Columns)
            {
                if (columnNameDict.TryGetValue(col.ColumnName, out string replacedColumnName))
                    col.ColumnName = replacedColumnName;
            }
        }

        private static void ApplyColumnsFormat(ExcelWorksheet sheet, 
            DataTable dt, 
            List<ExcelColumnFormat> excelColumnFormats)
        {
            int totalRow = dt.Rows.Count;

            if (excelColumnFormats.IsNullOrEmpty() || totalRow == 0)
                return;

            foreach (var excelColumnFormat in excelColumnFormats)
            {
                foreach (int column in excelColumnFormat.Columns)
                {
                    ApplyColumnFormat(sheet, excelColumnFormat, totalRow, column);
                }
            }
        }

        private static void ApplyColumnFormat(ExcelWorksheet sheet, 
            ExcelColumnFormat excelColumnFormat,
            int totalRow,
            int column)
        {
            if (!String.IsNullOrEmpty(excelColumnFormat.Format))
                sheet.Cells[2, column, totalRow + 1, column].Style.Numberformat.Format = excelColumnFormat.Format;

            if (excelColumnFormat.HorizontalAlignment != null)
                sheet.Cells[2, column, totalRow + 1, column].Style.HorizontalAlignment = excelColumnFormat.HorizontalAlignment.Value;
        }

        public class ExcelHeaderStyle
        {
            public bool Bold { get; set; } = true;
            public ExcelFillStyle PatternType { get; set; } = ExcelFillStyle.Solid;
            public Color BackgroundColor { get; set; } = Color.LightGray;
        }

        /// <summary>
        /// A setting class to define the displayed format/pattern for Excel columns
        /// </summary>
        public class ExcelColumnFormat
        {
            public string Format { get; set; }
            public int[] Columns { get; set; }
            public ExcelHorizontalAlignment? HorizontalAlignment { get; set; }
        }
    }
}
