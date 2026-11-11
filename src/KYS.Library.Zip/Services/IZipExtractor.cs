using CSharpFunctionalExtensions;

namespace KYS.Library.Zip.Services;

/// <summary>
/// Define a contract for unzipping item.
/// </summary>
public interface IZipExtractor
{
    /// <summary>
    /// Unzip the zipped file and save in the provided location.
    /// </summary>
    /// <param name="srcFilePath">The source file path of the zipped file.</param>
    /// <param name="destDirPath">The directory path where the unzipped files will be saved.</param>
    /// <returns>
    /// A <see cref="Result"/> that indicates success if the zip file is extracted successfully; 
    /// otherwise, a failed result with an error message describing the failure reason.
    /// </returns>
    Result Unzip(string srcFilePath, string destDirPath);
}

