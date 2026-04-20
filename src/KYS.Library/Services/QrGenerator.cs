using System;
using System.IO;
using CSharpFunctionalExtensions;
using KYS.Library.Helpers;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using Result = CSharpFunctionalExtensions.Result;

namespace KYS.Library.Services
{
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

    /// <summary>
    /// A service for generating the QR code.
    /// </summary>
    public class QrGenerator : IQrGenerator
    {
        private readonly string _value;
        private readonly int _width;
        private readonly int _height;
        private readonly string _logoPath;
        private readonly int? _logoWidth;
        private readonly int? _logoHeight;
        private readonly int _margin;

        private static readonly int _defaultWidth = 200;
        private static readonly int _defaultHeight = 200;
        private static readonly int _defaultMargin = 0;
        private static readonly int _defaultLogoRatio = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="QrGenerator"/> class.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="width">The width for the generated QR code image.</param>
        /// <param name="height">The height for the generated QR code image.</param>
        /// <param name="margin">The margin for the generated QR code image.</param>
        private QrGenerator(string value, int width, int height, int margin)
        {
            _value = value;
            _width = width;
            _height = height;
            _margin = margin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QrGenerator"/> class.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="logoPath">The path for the logo image which the logo will be attached in the QR code.</param>
        /// <param name="width">The width for the generated QR code image.</param>
        /// <param name="height">The height for the generated QR code image.</param>
        /// <param name="margin">The margin for the generated QR code image.</param>
        /// <param name="logoWidth">The width for the generated logo.</param>
        /// <param name="logoHeight">The height for the generated logo.</param>
        private QrGenerator(string value, string logoPath, int width, int height, int margin, int logoWidth, int logoHeight)
            : this(value, width, height, margin)
        {
            _logoPath = logoPath;
            _logoWidth = logoWidth;
            _logoHeight = logoHeight;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="QrGenerator"/> class with the specified value and default width, height and margin.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <returns>A <see cref="Result{T}"/> containing the created QR generator instance.</returns>
        public static Result<QrGenerator> Create(string value)
        {
            return Create(value, _defaultWidth, _defaultHeight, _defaultMargin);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="QrGenerator"/> class with the specified value and default width, height and margin.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="width">The width for the generated QR code image.</param>
        /// <param name="height">The height for the generated QR code image.</param>
        /// <param name="margin">The margin for the generated QR code image.</param>
        /// <returns>A <see cref="Result{T}"/> containing the created QR generator instance.</returns>
        public static Result<QrGenerator> Create(string value, int width, int height, int margin)
        {
            if (String.IsNullOrWhiteSpace(value))
                return Result.Failure<QrGenerator>($"Invalid {nameof(value)}.");

            if (width <= 0)
                return Result.Failure<QrGenerator>($"Invalid {nameof(width)}.");

            if (height <= 0)
                return Result.Failure<QrGenerator>($"Invalid {nameof(height)}.");

            if (margin < 0)
                return Result.Failure<QrGenerator>($"Invalid {nameof(margin)}.");

            return Result.Success(new QrGenerator(value, width, height, margin));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="QrGenerator"/> class with the specified value and logo path, and default width, height and margin.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="logoPath">The path for the logo image which the logo will be attached in the QR code.</param>
        /// <returns>A <see cref="Result{T}"/> containing the created QR generator instance.</returns>
        public static Result<QrGenerator> CreateWithLogo(string value, string logoPath)
        {
            int width = _defaultWidth;
            int height = _defaultHeight;
            int margin = _defaultMargin;
            int logoWidth = width / _defaultLogoRatio;
            int logoHeight = height / _defaultLogoRatio;

            return CreateWithLogo(value, logoPath, width, height, margin, logoWidth, logoHeight);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="QrGenerator"/> class with the specified value, logo path, width, height and margin, and default logo width and height.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="logoPath">The path for the logo image which the logo will be attached in the QR code.</param>
        /// <param name="width">The width for the generated QR code image.</param>
        /// <param name="height">The height for the generated QR code image.</param>
        /// <param name="margin">The margin for the generated QR code image.</param>
        /// <param name="logoWidth">The width for the generated logo.</param>
        /// <param name="logoHeight">The height for the generated logo.</param>
        /// <returns>A <see cref="Result{T}"/> containing the created QR generator instance.</returns>
        public static Result<QrGenerator> CreateWithLogo(string value, string logoPath, int width, int height, int margin, int logoWidth, int logoHeight)
        {
            return Create(value, width, height, margin)
                .Bind(service =>
                {
                    if (String.IsNullOrWhiteSpace(logoPath))
                        return Result.Failure<QrGenerator>($"Invalid {nameof(logoPath)}.");

                    if (!File.Exists(logoPath))
                        return Result.Failure<QrGenerator>($"File not found: {nameof(logoPath)}.");

                    if (width <= logoWidth)
                        return Result.Failure<QrGenerator>("Provided logo width must be smaller than QR code width.");

                    if (height <= logoHeight)
                        return Result.Failure<QrGenerator>("Provided logo height must be smaller than QR code height.");

                    return Result.Success(new QrGenerator(value, logoPath, width, height, margin, logoWidth, logoHeight));
                });
        }

        /// <summary>
        /// Gets the value to be included in the QR code.
        /// </summary>
        public string Value { get { return _value; } }
        /// <summary>
        /// Gets the width for the generated QR code image.
        /// </summary>
        public int Width { get { return _width; } }
        /// <summary>
        /// Gets the height for the generated QR code image.
        /// </summary>
        public int Height { get { return _height; } }
        /// <summary>
        /// Gets the path for the logo image which the logo will be attached in the QR code.
        /// </summary>
        public string LogoPath { get { return _logoPath; } }
        /// <summary>
        /// Gets the width for the generated logo.
        /// </summary>
        public int? LogoWidth { get { return _logoWidth; } }
        /// <summary>
        /// Gets the height for the generated logo.
        /// </summary>
        public int? LogoHeight { get { return _logoHeight; } }
        /// <summary>
        /// Gets the margin for the generated QR code image.
        /// </summary>
        public int Margin { get { return _margin; } }

        /// <summary>
        /// Gets the <see cref="SKBitmap"/>instance for the QR code.
        /// </summary>
        public SKBitmap QRCode { get; private set; }

        /// <summary>
        /// Draw QR code.
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing the <see cref="SKBitmap"/> instance for the QR code.</returns>
        public Result<SKBitmap> Draw()
        {
            if (QRCode != null)
                return Result.Success(QRCode);

            var encodingOptions = new EncodingOptions
            {
                Width = _width,
                Height = _height,
                Hints = { { EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H } },
                Margin = _margin
            };

            var writer = new ZXing.SkiaSharp.BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = encodingOptions
            };

            var barcodeBitmap = writer.Write(_value);

            #region Add logo into QR
            if (!String.IsNullOrEmpty(_logoPath))
            {
                var logo = SKImage.FromEncodedData(_logoPath);
                SKBitmap logoBitmap = SKBitmap.FromImage(logo);

                SKBitmap overlay = logoBitmap.Resize(new SKImageInfo(_logoWidth.Value, _logoHeight.Value),
                    SKFilterQuality.High);

                int deltaHeight = barcodeBitmap.Height - overlay.Height;
                int deltaWidth = barcodeBitmap.Width - overlay.Width;

                using var canvas = new SKCanvas(barcodeBitmap);
                canvas.DrawBitmap(overlay, new SKPoint(deltaWidth / 2, deltaHeight / 2));
            }
            #endregion

            QRCode = barcodeBitmap;
            return Result.Success(barcodeBitmap);
        }

        /// <summary>
        /// Draw QR code into a <see cref="MemoryStream"/> instance.
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing the <see cref="MemoryStream"/> instance containing the QR code.</returns>
        public Result<MemoryStream> DrawAndToMemoryStream()
        {
            return Draw()
                .Bind(ToMemoryStream);
        }

        /// <summary>
        /// Draw the QR code and generate a Base64 string.
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing the Base64 string for the QR code.</returns>
        public Result<string> DrawAndToBase64()
        {
            return DrawAndToMemoryStream()
                .Map(ms => StreamHelper.ToBase64Async(ms)
                    .GetAwaiter()
                    .GetResult());
        }

        /// <summary>
        /// Draw the QR code and generate a data URI string.
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing the data URI string for the QR code.</returns>
        public Result<string> DrawAndToDataUri()
        {
            return DrawAndToBase64()
                .Map(base64 => $"data:image/png;base64, {base64}");
        }

        /// <summary>
        /// Draw the QR code and save it as a file.
        /// </summary>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        /// <param name="filePath">The path for the QR code to be saved.</param>
        public Result DrawAndToFile(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                return Result.Failure($"Invalid {nameof(filePath)}.");

            if (!filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                filePath += ".png";

            return Draw()
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
    }
}
