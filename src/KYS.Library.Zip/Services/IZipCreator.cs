using CSharpFunctionalExtensions;

namespace KYS.Library.Zip.Services;

/// <summary>
/// Define a contract for zipping item.
/// </summary>
public interface IZipCreator
{
    /// <summary>
    /// Zip item(s).
    /// </summary>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the byte array of the created zip file if successful; 
    /// otherwise, a failed result with an error message.
    /// </returns>
    Result<byte[]> Zip();

    /// <summary>
    /// Zip item(s) and save the zipped file in the provided location.
    /// </summary>
    /// <param name="destDirPath">The directory path where the zipped file will be saved.</param>
    /// <returns>
    /// A <see cref="Result"/> that indicates success if the zip file is created and saved successfully; 
    /// otherwise, a failed result with an error message describing the failure reason.
    /// </returns>
    Result ZipAndToFile(string destDirPath);
}
