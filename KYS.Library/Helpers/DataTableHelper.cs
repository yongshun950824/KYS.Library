using CsvHelper;
using CsvHelper.Configuration;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using KYS.Library.Extensions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Data;
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
            using TextWriter writer = new StreamWriter(ms);

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
            LicenseContext license = LicenseContext.NonCommercial)
        {
            ExcelPackage.LicenseContext = license;

            const int START_ROW = 1;
            const int START_COL = 1;

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

        private static string DataTableToCSV(
            DataTable dt,
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

        public class ExcelHeaderStyle
        {
            public bool Bold { get; set; } = true;
            public ExcelFillStyle PatternType { get; set; } = ExcelFillStyle.Solid;
            public Color BackgroundColor { get; set; } = Color.LightGray;
        }
    }
}
