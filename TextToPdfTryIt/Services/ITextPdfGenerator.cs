using System.Collections.Generic;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using TextToPdfClassic.Models;

namespace TextToPdfClassic.Services
{
    public static class ITextPdfGenerator
    {
        public static byte[] Generate(IEnumerable<TextChunk> chunks,
                              float w = 595, float h = 842)
        {
            using (var ms = new MemoryStream())
            using (var writer = new PdfWriter(ms))
            using (var pdf = new PdfDocument(writer))
            {
                pdf.AddNewPage(new PageSize(w, h));

                var canvas = new PdfCanvas(pdf.GetFirstPage());
                foreach (var c in chunks)
                {
                    PdfFont font = ResolveFont(c.FontName);
                    canvas.SaveState()
                          .BeginText()
                          .SetFontAndSize(font, c.FontSize)
                          .SetFillColor(new DeviceRgb(c.R, c.G, c.B))
                          .MoveText(c.X, c.Y)
                          .ShowText(c.Text)
                          .EndText()
                          .RestoreState();
                }
                pdf.Close();         // explicit in 7.3
                return ms.ToArray(); // after pdf is closed so the stream is flushed
            }
        }


        private static PdfFont ResolveFont(string name) =>
            name.Contains("Arial") ? PdfFontFactory.CreateFont(StandardFonts.HELVETICA)
                                   : PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
    }
}
