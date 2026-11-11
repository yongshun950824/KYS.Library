using System.Data;
using System.Diagnostics.CodeAnalysis;
using KYS.Library.Excel.Options;

namespace KYS.Library.Excel.Models;

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
            ValidateDataTable(value);

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

    [SuppressMessage("Usage", "S3236:Caller information parameters should not be explicitly provided",
        Justification = "Property setters always pass 'value', so nameof(DataTable) is clearer.")]
    private static void ValidateDataTable(DataTable value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(DataTable));
    }
}
