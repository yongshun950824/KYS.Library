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
        private SKBitmap _bitmap;
        private readonly string _value;
        private readonly int _width;
        private readonly int _height;
        private readonly string _logoPath;
        private readonly int? _logoWidth;
        private readonly int? _logoHeight;

        public QrGeneratorService(string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            _value = value;

            if (_width == default)
                _width = 200;

            if (_height == default)
                _height = 200;
        }

        public QrGeneratorService(string value, int width, int height)
            : this(value)
        {
            _width = width;
            _height = height;
        }

        public QrGeneratorService(string value, int width, int height, string logoPath)
            : this(value, width, height)
        {
            if (String.IsNullOrEmpty(logoPath))
                throw new ArgumentNullException(nameof(logoPath));

            if (!File.Exists(logoPath))
                throw new FileNotFoundException(nameof(logoPath));

            _logoPath = logoPath;
            _logoWidth = width / 5;
            _logoHeight = height / 5;
        }

        public QrGeneratorService(string value, int width, int height, string logoPath, int logoWidth, int logoHeight)
            : this(value, width, height, logoPath)
        {
            if (width <= logoWidth)
                throw new ArgumentException("Provided logo width must be smaller than QR code width.");

            if (height <= logoHeight)
                throw new ArgumentException("Provided logo height must be smaller than QR code height.");

            if (width * height <= logoWidth * logoHeight)
                throw new ArgumentException("Provided logo area must be smaller than QR code area.");

            _logoWidth = logoWidth;
            _logoHeight = logoHeight;
        }

        public string Value { get { return _value; } }
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public SKBitmap QRCode { get { return _bitmap; } }

        public SKBitmap Draw()
        {
            if (_bitmap != null)
                return _bitmap;

            var encodingOptions = new EncodingOptions
            {
                Width = _width,
                Height = _height,
                Hints = { { EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H } }
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

                int deltaHeigth = barcodeBitmap.Height - overlay.Height;
                int deltaWidth = barcodeBitmap.Width - overlay.Width;

                using var canvas = new SKCanvas(barcodeBitmap);
                canvas.DrawBitmap(overlay, new SKPoint(deltaWidth / 2, deltaHeigth / 2));
            }
            #endregion

            _bitmap = barcodeBitmap;
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
            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

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
