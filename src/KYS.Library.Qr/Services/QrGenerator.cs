using CSharpFunctionalExtensions;
using KYS.Library.Helpers;
using KYS.Library.Qr.Models;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using Result = CSharpFunctionalExtensions.Result;

namespace KYS.Library.Qr.Services;

/// <summary>
/// A service for generating the QR code.
/// </summary>
public class QrGenerator : IQrGenerator
{
    internal const double DEFAULT_LOGO_AREA_RATIO = 0.22;
    internal const double MIN_LOGO_AREA_RATIO = 0.01;
    internal const double MAX_LOGO_AREA_RATIO = 0.09;

    /// <summary>
    /// Draw QR code.
    /// </summary>
    /// <returns>A <see cref="Result{T}"/> containing the <see cref="SKBitmap"/> instance for the QR code.</returns>
    public Result<SKBitmap> Generate(QrCodeRequest request)
    {
        var encodingOptions = new EncodingOptions
        {
            Width = request.Width,
            Height = request.Height,
            Hints = { { EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H } },
            Margin = request.Margin
        };

        var writer = new ZXing.SkiaSharp.BarcodeWriter()
        {
            Format = BarcodeFormat.QR_CODE,
            Options = encodingOptions
        };

        var barcodeBitmap = writer.Write(request.Value);

        #region Add logo into QR
        if (!String.IsNullOrEmpty(request.LogoPath))
        {
            var logoDimensionResult = ValidateLogoDimension(request);
            if (logoDimensionResult.IsFailure)
                return Result.Failure<SKBitmap>(logoDimensionResult.Error);

            var logoDimension = logoDimensionResult.Value;
            var logo = SKImage.FromEncodedData(request.LogoPath);
            SKBitmap logoBitmap = SKBitmap.FromImage(logo);

            SKBitmap overlay = logoBitmap.Resize(new SKImageInfo(logoDimension.LogoWidth, logoDimension.LogoHeight),
                SKFilterQuality.High);

            int deltaHeight = barcodeBitmap.Height - overlay.Height;
            int deltaWidth = barcodeBitmap.Width - overlay.Width;

            using var canvas = new SKCanvas(barcodeBitmap);
            canvas.DrawBitmap(overlay, new SKPoint(deltaWidth / 2, deltaHeight / 2));
        }
        #endregion

        return Result.Success(barcodeBitmap);
    }

    /// <summary>
    /// Draw QR code into a <see cref="MemoryStream"/> instance.
    /// </summary>
    /// <returns>A <see cref="Result{T}"/> containing the <see cref="MemoryStream"/> instance containing the QR code.</returns>
    public Result<MemoryStream> GenerateAsPng(QrCodeRequest request)
    {
        return Generate(request)
            .Bind(ToMemoryStream);
    }

    /// <summary>
    /// Draw the QR code and generate a Base64 string.
    /// </summary>
    /// <returns>A <see cref="Result{T}"/> containing the Base64 string for the QR code.</returns>
    public Result<string> GenerateAsBase64(QrCodeRequest request)
    {
        return GenerateAsPng(request)
            .Bind(StreamHelper.ToBase64);
    }

    /// <summary>
    /// Draw the QR code and generate a data URI string.
    /// </summary>
    /// <returns>A <see cref="Result{T}"/> containing the data URI string for the QR code.</returns>
    public Result<string> GenerateAsDataUri(QrCodeRequest request)
    {
        return GenerateAsBase64(request)
            .Map(base64 => $"data:image/png;base64,{base64}");
    }

    /// <summary>
    /// Draw the QR code and save it as a file.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    /// <param name="filePath">The path for the QR code to be saved.</param>
    public Result SaveAsPng(QrCodeRequest request, string filePath)
    {
        if (String.IsNullOrWhiteSpace(filePath))
            return Result.Failure($"Invalid {nameof(filePath)}.");

        if (!filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            filePath += ".png";

        return Generate(request)
            .Bind(bitmap => ToFile(bitmap, filePath));
    }

    private static Result ToFile(SKBitmap bitmap, string filePath)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(filePath);
        data.SaveTo(stream);

        return Result.Success();
    }

    private static Result<MemoryStream> ToMemoryStream(SKBitmap bitmap)
    {
        MemoryStream ms = new();

        bitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
        ms.Seek(0, SeekOrigin.Begin);

        return Result.Success(ms);
    }

    private static Result<(int LogoWidth, int LogoHeight)> ValidateLogoDimension(QrCodeRequest request)
    {
        if (!request.LogoWidth.HasValue || !request.LogoHeight.HasValue)
        {
            return Result.Success((
                (int)(request.Width * DEFAULT_LOGO_AREA_RATIO),
                (int)(request.Height * DEFAULT_LOGO_AREA_RATIO)
            ));
        }

        int logoW = request.LogoWidth.Value;
        int logoH = request.LogoHeight.Value;

        // Check Aspect Ratio (prevent extreme rectangles from hitting corner markers)
        double aspectRatio = (double)Math.Max(logoW, logoH) / Math.Min(logoW, logoH);
        if (aspectRatio > 2.5)
        {
            return Result.Failure<(int, int)>("Logo aspect ratio is too wide or tall. Keep it closer to a square.");
        }

        double areaRatio = GetLogoAreaRatio(request);

        // Check Minimum Size
        if (areaRatio < MIN_LOGO_AREA_RATIO)
        {
            return Result.Failure<(int, int)>("Logo is too small and may not be visible.");
        }

        // Check Maximum Size
        if (areaRatio > MAX_LOGO_AREA_RATIO)
        {
            return Result.Failure<(int, int)>($"Logo is too large ({areaRatio:P1} of QR area). Keep it under {MAX_LOGO_AREA_RATIO:P0} for reliable scanning.");
        }

        return Result.Success((logoW, logoH));
    }

    internal static double GetLogoAreaRatio(QrCodeRequest request)
    {
        int logoW = request.LogoWidth!.Value;
        int logoH = request.LogoHeight!.Value;

        double qrArea = (double)request.Width * request.Height;
        double logoArea = (double)logoW * logoH;

        return logoArea / qrArea;
    }
}
