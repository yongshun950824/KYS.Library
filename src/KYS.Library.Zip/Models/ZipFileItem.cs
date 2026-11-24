namespace KYS.Library.Zip.Models;

/// <summary>
/// Represents the blueprint on the file element to be zipped into a zip file or unzipped from the zip file.
/// </summary>
public sealed class ZipFileItem
{
    private string _name = null!;
    /// <summary>
    /// Gets the file name.
    /// </summary>
    public required string Name
    {
        get
        {
            // Replace '/' character to '_', '/' will treat as directory
            return _name.Replace("/", "_");
        }
        set { _name = value; }
    }

    /// <summary>
    /// Gets or sets the file content to be included in the zip file or extracted from the zip file.
    /// </summary>
    public required byte[] Contents { get; set; }
}