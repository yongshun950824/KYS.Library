using CSharpFunctionalExtensions;
using SkiaSharp;

namespace KYS.Library.Qr.Services;

/// <summary>
/// Defines a contract for generating QR code.
/// </summary>
public interface IQrGenerator
{
    /// <summary>
    /// Draw QR code.
    /// </summary>
    /// <returns>A <see cref="Result{T}"/> containing the <see cref="SKBitmap"/> instance for the QR code.</returns>
    Result<SKBitmap> Draw();

    /// <summary>
    /// Draw QR code into a <see cref="MemoryStream"/> instance.
    /// </summary>
    /// <returns>A <see cref="Result{T}"/> containing the <see cref="MemoryStream"/> instance containing the QR code.</returns>
    Result<MemoryStream> DrawAndToMemoryStream();

    /// <summary>
    /// Draw the QR code and generate a Base64 string.
    /// </summary>
    /// <returns>A <see cref="Result{T}"/> containing the Base64 string for the QR code.</returns>
    Result<string> DrawAndToBase64();

    /// <summary>
    /// Draw the QR code and generate a data URI string.
    /// </summary>
    /// <returns>A <see cref="Result{T}"/> containing the data URI string for the QR code.</returns>
    Result<string> DrawAndToDataUri();

    /// <summary>
    /// Draw the QR code and save it as a file.
    /// </summary>
    /// <param name="filePath">The path for the QR code to be saved.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Result DrawAndToFile(string filePath);
}
