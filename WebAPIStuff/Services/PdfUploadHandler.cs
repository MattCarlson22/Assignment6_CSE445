using System.IO;
using iTextSharp.text.pdf;

namespace WebAPIStuff.Services
{
    public class PdfUploadHandler
    {
        public bool ValidatePdf(Stream pdfStream)
        {
            try
            {
                pdfStream.Position = 0;
                using (PdfReader reader = new PdfReader(pdfStream))
                {
                    return reader.NumberOfPages > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}