using System.Data;
using System.Globalization;
using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using KYS.Library.Extensions;
using Newtonsoft.Json;

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
    }
}
