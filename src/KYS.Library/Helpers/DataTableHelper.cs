using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CSharpFunctionalExtensions;
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
        /// <returns>A <see cref="Result{T}" /> containing <see cref="byte[]" /> instance which is the file.</returns>
        public static Result<byte[]> WriteToTextFile(DataTable dt,
            bool printHeaders = true,
            string delimiter = ";",
            CultureInfo cultureInfo = null)
        {
            string csvString = null;

            if (!dt.IsNullOrEmpty())
                csvString = DataTableToCSV(dt, printHeaders, delimiter, cultureInfo);

            using Stream stream = StreamHelper.WriteStringIntoStream(csvString);
            using MemoryStream ms = new();
            stream.CopyTo(ms);

            ms.Position = 0;
            return Result.Success(ms.ToArray());
        }

        /// <summary>
        /// Write <see cref="DataTable" /> into Excel file.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable" /> instance.</param>
        /// <param name="headerRowStyle">The style for the header row.</param>
        /// <param name="license">The license used in <see cref="ExcelPackage" />.</param>
        /// <returns>A <see cref="Result{T}" /> containing the <see cref="byte[]" /> instance which is the Excel file content.</returns>
        public static Result<byte[]> WriteToExcelFile(DataTable dt,
            ExcelHelper.ExcelRowStyle headerRowStyle = null,
            List<ExcelHelper.ExcelColumnFormat> excelColumnFormats = null,
            LicenseContext license = LicenseContext.NonCommercial)
        {
            return ExcelHelper.CreateExcelBook(dt, excelColumnFormats, headerRowStyle, license: license);
        }

        /// <summary>
        /// Write <see cref="DataTable" /> into PDF file.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable" /> instance.</param>
        /// <param name="printHeaders">The <see cref="bool" /> value indicates the header to be included.</param>
        /// <param name="tableHeaderStyle">The style for the table header.</param>
        /// <param name="tableBodyStyle">The style for the table content.</param>
        /// <returns>A <see cref="Result{T}" /> containing the <see cref="byte[]" /> instance which is the PDF file content.</returns>
        public static Result<byte[]> WriteToPdfFile(DataTable dt,
            bool printHeaders = true,
            Style tableHeaderStyle = null,
            Style tableBodyStyle = null)
        {
            if (dt == null)
                return Result.Failure<byte[]>(DomainErrors.CannotBeNull(nameof(dt)));

            using MemoryStream ms = new();
            using PdfWriter pdfWriter = new(ms);
            pdfWriter.SetCloseStream(false);

            PdfDocument pdfDoc = new(pdfWriter);
            Document document = new(pdfDoc);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            document.SetFont(font)
                .SetFontSize(10);

            #region Sample Header style
            tableHeaderStyle ??= new Style()
                .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER);
            #endregion

            if (!dt.IsNullOrEmpty())
                document.Add(DataTableToPdfTable(dt, printHeaders, tableHeaderStyle, tableBodyStyle));

            document.Close();

            ms.Position = 0;
            return Result.Success(ms.ToArray());
        }

        /// <summary>
        /// Write <see cref="DataTable" /> into JSON file.
        /// </summary>
        /// <param name="dt">The <see cref="DataTable" /> instance.</param>
        /// <param name="isIndented">The <see cref="bool" /> value indicates to print the JSON as indented.</param>
        /// <returns>A <see cref="Result{T}" /> containing the <see cref="byte[]" /> instance which is the JSON file content.</returns>
        public static Result<byte[]> WriteToJsonFile(DataTable dt, bool isIndented = true)
        {
            Formatting formatting = isIndented switch
            {
                true => Formatting.Indented,
                _ => Formatting.None
            };

            string jsonString = JsonConvert.SerializeObject(dt, formatting);

            using Stream stream = StreamHelper.WriteStringIntoStream(jsonString);
            using MemoryStream ms = new();
            stream.CopyTo(ms);

            ms.Position = 0;

            return Result.Success(ms.ToArray());
        }

        /// <summary>
        /// Read CSV from file into <see cref="DataTable" />.
        /// </summary>
        /// <param name="filePath">The path for the CSV file to be read.</param>
        /// <returns>A <see cref="Result{T}" /> containing the <see cref="DataTable" /> instance with the data row(s).</returns>
        public static Result<DataTable> ReadCSV(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                return Result.Failure<DataTable>(DomainErrors.Required(nameof(filePath)));

            if (!filePath.EndsWith(".csv"))
                return Result.Failure<DataTable>($"Provided {nameof(filePath)} must be a CSV file.");

            using StreamReader sr = new(filePath);

            return Result.Success(ReadCSV(sr));
        }

        /// <summary>
        /// Read CSV from <see cref="Stream" /> into <see cref="DataTable" />.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> instance containing the CSV file.</param>
        /// <returns>A <see cref="Result{T}" /> containing the <see cref="DataTable" /> instance with the data row(s).</returns>
        public static Result<DataTable> ReadCSV(Stream stream)
        {
            if (stream == null)
                return Result.Failure<DataTable>(DomainErrors.CannotBeNull(nameof(stream)));

            using StreamReader sr = new(stream);

            return Result.Success(ReadCSV(sr));
        }

        private static DataTable ReadCSV(StreamReader sr)
        {
            DataTable dt = new();

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
            StringWriter sw = new();

            CsvConfiguration config = new(cultureInfo ?? CultureInfo.InvariantCulture)
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
