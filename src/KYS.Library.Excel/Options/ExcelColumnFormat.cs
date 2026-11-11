using System.Data;
using OfficeOpenXml.Style;

namespace KYS.Library.Excel.Options;

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

