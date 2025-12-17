using SkiaSharp;
using System;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace KYS.Library.Services
{
    /// <summary>
    /// Defines a contract for generating QR code.
    /// </summary>
    public interface IQrGeneratorService
    {
        /// <summary>
        /// Draw QR code.
        /// </summary>
        /// <returns>A <see cref="SKBitmap"/>instance for the QR code.</returns>
        SKBitmap Draw();
        /// <summary>
        /// Draw QR code into a <see cref="MemoryStream"/> instance.
        /// </summary>
        /// <returns>A <see cref="MemoryStream"/> instance containing the QR code.</returns>
        MemoryStream DrawAndToMemoryStream();
        /// <summary>
        /// Draw the QR code and generate a Base64 string.
        /// </summary>
        /// <returns>A Base64 string for the QR code.</returns>
        string DrawAndToBase64();
        /// <summary>
        /// Draw the QR code and generate a data URI string.
        /// </summary>
        /// <returns>A data URI string for the QR code.</returns>
        string DrawAndToDataUri();
        /// <summary>
        /// Draw the QR code and save it as a file.
        /// </summary>
        /// <param name="filePath">The path for the QR code to be saved.</param>
        void DrawAndToFile(string filePath);
    }

    /// <summary>
    /// A service for generating the QR code.
    /// </summary>
    public class QrGeneratorService : IQrGeneratorService
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
        /// Initializes a new instance of the <see cref="QrGeneratorService"/> class.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        public QrGeneratorService(string value)
            : this(value, _defaultWidth, _defaultHeight, _defaultMargin)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QrGeneratorService"/> class.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="width">The width for the generated QR code image.</param>
        /// <param name="height">The height for the generated QR code image.</param>
        /// <param name="margin">The margin for the generated QR code image.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public QrGeneratorService(string value, int width, int height, int margin)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            if (width <= 0)
                throw new ArgumentException($"Invalid {nameof(width)}.");

            if (height <= 0)
                throw new ArgumentException($"Invalid {nameof(height)}.");

            if (margin < 0)
                throw new ArgumentException($"Invalid {nameof(margin)}.");

            _value = value;
            _width = width;
            _height = height;
            _margin = margin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QrGeneratorService"/> class.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="logoPath">The path for the logo image which the logo will be attached in the QR code.</param>
        public QrGeneratorService(string value, string logoPath)
            : this(value, logoPath, _defaultWidth, _defaultHeight, _defaultMargin)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QrGeneratorService"/> class.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="logoPath">The path for the logo image which the logo will be attached in the QR code.</param>
        /// <param name="width">The width for the generated QR code image.</param>
        /// <param name="height">The height for the generated QR code image.</param>
        /// <param name="margin">The margin for the generated QR code image.</param>
        public QrGeneratorService(string value, string logoPath, int width, int height, int margin)
            : this(value, logoPath, width, height, margin, width / _defaultLogoRatio, height / _defaultLogoRatio)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QrGeneratorService"/> class.
        /// </summary>
        /// <param name="value">The value to be included in the QR code.</param>
        /// <param name="logoPath">The path for the logo image which the logo will be attached in the QR code.</param>
        /// <param name="width">The width for the generated QR code image.</param>
        /// <param name="height">The height for the generated QR code image.</param>
        /// <param name="margin">The margin for the generated QR code image.</param>
        /// <param name="logoWidth">The width for the generated logo.</param>
        /// <param name="logoHeight">The height for the generated logo.</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public QrGeneratorService(string value, string logoPath, int width, int height, int margin, int logoWidth, int logoHeight)
            : this(value, width, height, margin)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(logoPath);

            if (!File.Exists(logoPath))
                throw new FileNotFoundException(nameof(logoPath));

            if (width <= logoWidth)
                throw new ArgumentException("Provided logo width must be smaller than QR code width.");

            if (height <= logoHeight)
                throw new ArgumentException("Provided logo height must be smaller than QR code height.");

            _logoPath = logoPath;
            _logoWidth = logoWidth;
            _logoHeight = logoHeight;
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
        /// The <see cref="SKBitmap"/>instance for the QR code.
        /// </summary>
        public SKBitmap QRCode { get; private set; }

        /// <summary>
        /// Draw QR code.
        /// </summary>
        /// <returns>A <see cref="SKBitmap"/>instance for the QR code.</returns>
        public SKBitmap Draw()
        {
            if (QRCode != null)
                return QRCode;

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
            return barcodeBitmap;
        }

        /// <summary>
        /// Draw QR code into a <see cref="MemoryStream"/> instance.
        /// </summary>
        /// <returns>A <see cref="MemoryStream"/> instance containing the QR code.</returns>
        public MemoryStream DrawAndToMemoryStream()
        {
            MemoryStream ms = new MemoryStream();

            SKBitmap bitmap = Draw();

            bitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        /// <summary>
        /// Draw the QR code and generate a Base64 string.
        /// </summary>
        /// <returns>A Base64 string for the QR code.</returns>
        public string DrawAndToBase64()
        {
            using MemoryStream ms = DrawAndToMemoryStream();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Draw the QR code and generate a data URI string.
        /// </summary>
        /// <returns>A data URI string for the QR code.</returns>
        public string DrawAndToDataUri()
        {
            return $"data:image/png;base64, {DrawAndToBase64()}";
        }

        /// <summary>
        /// Draw the QR code and save it as a file.
        /// </summary>
        /// <param name="filePath">The path for the QR code to be saved.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void DrawAndToFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            if (!filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                filePath += ".png";

            SKBitmap bitmap = Draw();

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);
        }
    }
}
