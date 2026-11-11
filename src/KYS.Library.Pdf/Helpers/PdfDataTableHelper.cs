using System.Data;
using CSharpFunctionalExtensions;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using KYS.Library.Extensions;
using KYS.Library.Helpers;

namespace KYS.Library.Pdf.Helpers
{
    /// <summary>
    /// Provide utility methods for <see cref="DataTable" />.
    /// </summary>
    public static class PdfDataTableHelper
    {
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
