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
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for <see cref="DataTable" />.
    /// </summary>
    public static class DataTableHelper
    {
        /// <summary>
        /// Write <see cref="DataTable" /> into text/CSV file.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable" /> instance.</param>
        /// <param name="printHeaders">The <see cref="bool" /> value indicates the header to be included.</param>
        /// <param name="delimiter">The separator splits the column.</param>
        /// <param name="cultureInfo">The culture to be used.</param>
        /// <returns>The byte array for the file.</returns>
        public static byte[] WriteToTextFile(DataTable dt,
            bool printHeaders = true,
            string delimiter = ";",
            CultureInfo cultureInfo = null)
        {
            string csvString = null;

            if (!dt.IsNullOrEmpty())
                csvString = DataTableToCSV(dt, printHeaders, delimiter, cultureInfo);

            using Stream stream = StreamHelper.WriteStringIntoStream(csvString);
            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);

            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Write <see cref="DataTable" /> into Excel file.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable" /> instance.</param>
        /// <param name="headerRowStyle">The style for the header row.</param>
        /// <param name="license">The license used in <see cref="ExcelPackage" />.</param>
        /// <returns>The byte array for the file.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] WriteToExcelFile(DataTable dt,
            ExcelHelper.ExcelRowStyle headerRowStyle = null,
            List<ExcelHelper.ExcelColumnFormat> excelColumnFormats = null,
            LicenseContext license = LicenseContext.NonCommercial)
        {
            ArgumentNullException.ThrowIfNull(dt);

            return ExcelHelper.CreateExcelBook(dt, excelColumnFormats, headerRowStyle, license: license);
        }

        /// <summary>
        /// Write <see cref="DataTable" /> into PDF file.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable" /> instance.</param>
        /// <param name="printHeaders">The <see cref="bool" /> value indicates the header to be included.</param>
        /// <param name="tableHeaderStyle">The style for the table header.</param>
        /// <param name="tableBodyStyle">The style for the table content.</param>
        /// <returns>The byte array for the file.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] WriteToPdfFile(DataTable dt,
            bool printHeaders = true,
            Style tableHeaderStyle = null,
            Style tableBodyStyle = null)
        {
            ArgumentNullException.ThrowIfNull(dt);

            using MemoryStream ms = new MemoryStream();
            using PdfWriter pdfWriter = new PdfWriter(ms);
            pdfWriter.SetCloseStream(false);

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
        /// Write <see cref="DataTable" /> into JSON file.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable" /> instance.</param>
        /// <param name="isIndented">The <see cref="bool" /> value indicates to print the JSON as indented.</param>
        /// <returns>The byte array for the file.</returns>
        public static byte[] WriteToJsonFile(DataTable dt, bool isIndented = true)
        {
            Formatting formatting = isIndented switch
            {
                true => Formatting.Indented,
                _ => Formatting.None
            };

            string jsonString = JsonConvert.SerializeObject(dt, formatting);

            using Stream stream = StreamHelper.WriteStringIntoStream(jsonString);
            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);

            ms.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Read CSV from file into <see cref="DataTable" />.
        /// </summary>
        /// <param name="filePath">The path for the CSV file to be read.</param>
        /// <returns>The <see cref="DataTable" /> instance with the data row(s).</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static DataTable ReadCSV(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            if (!filePath.EndsWith(".csv"))
            {
                throw new ArgumentException($"Provided {nameof(filePath)} must be a CSV file.");
            }

            using StreamReader sr = new StreamReader(filePath);

            return ReadCSV(sr);
        }

        /// <summary>
        /// Read CSV from <see cref="Stream" /> into <see cref="DataTable" />.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> instance containing the CSV file.</param>
        /// <returns>The <see cref="DataTable" /> instance with the data row(s).</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DataTable ReadCSV(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

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
                        .Add(new Paragraph(dtRow[columnName].ToString()));

                    if (bodyStyle != null)
                        cell.AddStyle(bodyStyle);

                    table.AddCell(cell);
                }
            }
            #endregion

            return table;
        }
    }
}
