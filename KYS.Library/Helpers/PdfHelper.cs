using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using KYS.Library.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for the PDF file.
    /// </summary>
    public static class PdfHelper
    {
        /// <summary>
        /// Convert HTML to PDF supported with importing custom font family. <br />
        /// <br />
        /// To use custom font family only: <br />
        /// <code>
        /// ConvertHtmlToPdf(htmlContent, false, false, false, importCustomFontFamilyFilePaths);
        /// </code>
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <param name="registerStandardPdfFonts">The <see cref="bool" /> value indicates the standard PDF font families are imported.</param>
        /// <param name="registerShippedFonts">The <see cref="bool" /> value indicates the shipped font families are imported.</param>
        /// <param name="registerSystemFonts">The <see cref="bool" /> value indicates the system font families are imported.</param>
        /// <param name="importCustomFontFamilyFilePaths">List of the custom font family file paths.</param>
        /// <returns>The <see cref="MemoryStream" /> containing the generated PDF file.</returns>
        public static MemoryStream ConvertHtmlToPdf(string htmlContent,
            bool registerStandardPdfFonts = true,
            bool registerShippedFonts = true,
            bool registerSystemFonts = true,
            List<string> importCustomFontFamilyFilePaths = null)
        {
            MemoryStream ms = new MemoryStream();
            using var pdfWriter = new PdfWriter(ms);
            pdfWriter.SetCloseStream(false);

            ConverterProperties converterProperties = GetPdfConverterProperties(registerStandardPdfFonts,
                registerShippedFonts,
                registerSystemFonts,
                importCustomFontFamilyFilePaths);
            if (converterProperties == null)
                HtmlConverter.ConvertToPdf(htmlContent, pdfWriter);
            else
                HtmlConverter.ConvertToPdf(htmlContent, pdfWriter, converterProperties);

            ms.Position = 0;

            return ms;
        }

        /// <summary>
        /// Validate the PDF Document is password-protected.
        /// </summary>
        /// <param name="source">The byte array for the file.</param>
        /// <returns>The <see cref="bool" /> value indicates the PDF file is password-protected.</returns>
        public static bool IsPasswordProtected(byte[] source)
        {
            using MemoryStream ms = new MemoryStream(source);
            using PdfReader pdfReader = new PdfReader(ms);

            return IsPasswordProtected(pdfReader);
        }

        /// <summary>
        /// Validate the PDF Document is password-protected.
        /// </summary>
        /// <param name="pdfReader">The <see cref="PdfReader" /> instance containing the <see cref="MemoryStream" /> with the PDF file.</param>
        /// <returns>The <see cref="bool" /> value indicates the PDF file is password-protected.</returns>
        public static bool IsPasswordProtected(PdfReader pdfReader)
        {
            try
            {
                PdfDocument pdfDocument = new PdfDocument(pdfReader);
                pdfDocument.Close();

                return false;
            }
            catch (BadPasswordException)
            {
                return true;
            }
        }

        /// <summary>
        /// Unlock (decrypt) PDF document if it is password-protected. 
        /// </summary>
        /// <param name="source">The byte array for the file.</param>
        /// <param name="ownerPass">The password for the file.</param>
        /// <returns>Byte array for the unlocked PDF file.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] UnlockPdfDocument(byte[] source, string ownerPass)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerPass);

            if (!IsPasswordProtected(source))
                return source;

            byte[] OWNERPASS = Encoding.Default.GetBytes(ownerPass);

            ByteArrayOutputStream stream = new ByteArrayOutputStream();

            using MemoryStream ms = new MemoryStream(source);
            ReaderProperties readerProperties = new ReaderProperties().SetPassword(OWNERPASS);
            using PdfReader pdfReader = new PdfReader(ms, readerProperties);

            PdfWriter pdfWriter = new PdfWriter(stream);

            PdfDocument pdfDocument = new PdfDocument(pdfReader, pdfWriter);
            pdfDocument.Close();

            return stream.ToArray();
        }

        /// <summary>
        /// Lock (encrypt) PDF document with password.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ownerPass"></param>
        /// <param name="userPass"></param>
        /// <returns>Byte array for the unlocked PDF file.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] LockPdfDocument(byte[] source, string ownerPass, string userPass = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerPass);

            if (String.IsNullOrEmpty(userPass))
                userPass = ownerPass;

            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            using MemoryStream ms = new MemoryStream(source);
            using PdfReader pdfReader = new PdfReader(ms);

            byte[] OWNERPASS = Encoding.Default.GetBytes(ownerPass);
            byte[] USERPASS = Encoding.Default.GetBytes(userPass);

            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetStandardEncryption(USERPASS,
                OWNERPASS,
                EncryptionConstants.ALLOW_PRINTING,
                EncryptionConstants.ENCRYPTION_AES_128);

            PdfWriter pdfWriter = new PdfWriter(stream, writerProperties);

            PdfDocument pdfDocument = new PdfDocument(pdfReader, pdfWriter);
            pdfDocument.Close();

            return stream.ToArray();
        }

        /// <summary>
        /// Initialize PDF Converter properties (for importing custom font (.ttf) files etc.)
        /// <br /><br />
        /// To use the imported font without system font. 
        /// <br />
        /// <code>
        /// GetPdfConverterProperties(false, false, false, importCustomFontFamilyFilePaths)
        /// </code>
        /// </summary>
        /// <param name="registerStandardPdfFonts"><c>Boolean</c> indicates the standard PDF font families are imported.</param>
        /// <param name="registerShippedFonts"><c>Boolean</c> indicates the shipped font families are imported.</param>
        /// <param name="registerSystemFonts"><c>Boolean</c> indicates the system font families are imported.</param>
        /// <param name="importCustomFontFamilyFilePaths">List of the custom font family file paths.</param>
        /// <returns><c>ConverterProperties</c> instance to be used for <c>HtmlConverter.ConvertToPdf</c> method.</returns>
        private static ConverterProperties GetPdfConverterProperties(bool registerStandardPdfFonts = true,
            bool registerShippedFonts = true,
            bool registerSystemFonts = true,
            List<string> importCustomFontFilePaths = null)
        {
            if (registerStandardPdfFonts
                && registerShippedFonts
                && registerSystemFonts
                && importCustomFontFilePaths.IsNullOrEmpty())
                return null;

            importCustomFontFilePaths ??= new List<string>();

            DefaultFontProvider fontProvider;
            if (registerStandardPdfFonts && registerShippedFonts && registerSystemFonts)
                fontProvider = new DefaultFontProvider();
            else
                fontProvider = new DefaultFontProvider(registerStandardPdfFonts, registerShippedFonts, registerSystemFonts);

            foreach (string font in importCustomFontFilePaths.Where(x => File.Exists(x)))
            {
                fontProvider.AddFont(font);
            }

            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(fontProvider);

            return converterProperties;
        }
    }
}
