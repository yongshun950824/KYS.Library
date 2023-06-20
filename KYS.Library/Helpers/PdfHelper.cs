using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using KYS.Library.Extensions;
using System.Collections.Generic;
using System.IO;

namespace KYS.Library.Helpers
{
    public class PdfHelper
    {
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

            FontProvider fontProvider = null;
            if (registerStandardPdfFonts && registerShippedFonts && registerSystemFonts)
                fontProvider = new DefaultFontProvider();
            else
                fontProvider = new DefaultFontProvider(registerStandardPdfFonts, registerShippedFonts, registerSystemFonts);

            // Use only imported font files
            //FontProvider provider = new DefaultFontProvider(false, false, false);

            foreach (string font in importCustomFontFilePaths)
            {
                if (File.Exists(font))
                    fontProvider.AddFont(font);
            }

            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(fontProvider);

            return converterProperties;
        }
    }
}
