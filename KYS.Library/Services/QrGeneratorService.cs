using SkiaSharp;
using System;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace KYS.Library.Services
{
    public interface IQrGeneratorService
    {
        SKBitmap Draw();
        MemoryStream DrawAndToMemoryStream();
        string DrawAndToBase64();
        string DrawAndToDataUri();
        void DrawAndToFile(string filePath);
    }

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

        public QrGeneratorService(string value)
            : this(value, _defaultWidth, _defaultHeight, _defaultMargin)
        { }

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

        public QrGeneratorService(string value, string logoPath)
            : this(value, logoPath, _defaultWidth, _defaultHeight, _defaultMargin)
        { }

        public QrGeneratorService(string value, string logoPath, int width, int height, int margin)
            : this(value, logoPath, width, height, margin, width / _defaultLogoRatio, height / _defaultLogoRatio)
        { }

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

        public string Value { get { return _value; } }
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public string LogoPath { get { return _logoPath; } }
        public int? LogoWidth { get { return _logoWidth; } }
        public int? LogoHeight { get { return _logoHeight; } }
        public int Margin { get { return _margin; } }

        public SKBitmap QRCode { get; private set; }

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

        public MemoryStream DrawAndToMemoryStream()
        {
            MemoryStream ms = new MemoryStream();

            SKBitmap bitmap = Draw();

            bitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        public string DrawAndToBase64()
        {
            using MemoryStream ms = DrawAndToMemoryStream();

            return Convert.ToBase64String(ms.ToArray());
        }

        public string DrawAndToDataUri()
        {
            return $"data:image/png;base64, {DrawAndToBase64()}";
        }

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
