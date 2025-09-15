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
    public static class PdfHelper
    {
        /// <summary>
        /// Convert HTML to PDF supported with importing custom font family. <br />
        /// <br />
        /// To use custom font family only: <br />
        /// <c>ConvertHtmlToPdf(htmlContent, false, false, false, importCustomFontFamilyFilePaths);</c>
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <param name="registerStandardPdfFonts"></param>
        /// <param name="registerShippedFonts"></param>
        /// <param name="registerSystemFonts"></param>
        /// <param name="importCustomFontFamilyFilePaths"></param>
        /// <returns></returns>
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
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsPasswordProtected(byte[] source)
        {
            using MemoryStream ms = new MemoryStream(source);
            using PdfReader pdfReader = new PdfReader(ms);

            return IsPasswordProtected(pdfReader);
        }

        /// <summary>
        /// Validate the PDF Document is password-protected.
        /// </summary>
        /// <param name="pdfReader"></param>
        /// <returns></returns>
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
        /// <param name="source"></param>
        /// <param name="ownerPass"></param>
        /// <returns></returns>
        public static byte[] UnlockPdfDocument(byte[] source, string ownerPass)
        {
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
        /// <returns></returns>
        public static byte[] LockPdfDocument(byte[] source, string ownerPass, string userPass = null)
        {
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
        /// <c>GetPdfConverterProperties(false, false, false, importCustomFontFamilyFilePaths)</c>
        /// </summary>
        /// <param name="registerStandardPdfFonts"></param>
        /// <param name="registerShippedFonts"></param>
        /// <param name="registerSystemFonts"></param>
        /// <param name="importCustomFontFilePaths">Absolute custom font file paths.</param>
        /// <returns></returns>
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
