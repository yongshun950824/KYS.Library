using System.Drawing;
using OfficeOpenXml.Style;

namespace KYS.Library.Excel.Options;

/// <summary>
/// Represents the blueprint for customizing the style for whole row.
/// </summary>
public class ExcelRowStyle
{
    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public ExcelHorizontalAlignment HorizontalAlignment { get; set; } = ExcelHorizontalAlignment.Center;
    /// <summary>
    /// Gets or sets the indicator for applying the bold style.
    /// </summary>
    public bool Bold { get; set; } = true;
    /// <summary>
    /// Gets or sets the font color.
    /// </summary>
    public Color FontColor { get; set; } = Color.Black;
    /// <summary>
    /// Gets or sets the pattern type.
    /// </summary>
    public ExcelFillStyle PatternType { get; set; } = ExcelFillStyle.Solid;
    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public Color BackgroundColor { get; set; } = Color.Empty;
    /// <summary>
    /// Apply Auto Filter to Excel columns. (For header row only)
    /// </summary>
    public bool AutoFilter { get; set; }

    /// <summary>
    /// Gets the default header row style instance.
    /// </summary>
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

    /// <summary>
    /// Gets the default summary (bottom) row style instance.
    /// </summary>
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
