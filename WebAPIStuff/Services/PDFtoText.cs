using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using WebAPIStuff.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebAPIStuff.Services
{
    public interface IPdfToTextServices
    {
        byte[] ExtractTextToFile(Stream pdfStream, bool includeFormatting);
        List<TextStuff> ExtractTextChunks(Stream pdfStream);
    }

    public class PdfToTextService : IPdfToTextServices
    {
        public byte[] ExtractTextToFile(Stream pdfStream, bool includeFormatting)
        {
            if (pdfStream == null || !pdfStream.CanRead)
                throw new ArgumentException("Invalid PDF stream.");

            List<TextStuff> textChunks = ExtractTextChunks(pdfStream);

            StringBuilder textContent = new StringBuilder();
            if (includeFormatting)
            {
                for (int i = 0; i < textChunks.Count; i++)
                {
                    var chunk = textChunks[i];
                    textContent.AppendLine($"Character No. {i + 1}:");
                    textContent.AppendLine($"  Text: {chunk.Text}");
                    textContent.AppendLine($"  Position: (X: {chunk.Position.X:F2}, Y: {chunk.Position.Y:F2})");
                    textContent.AppendLine($"  Font: {chunk.Font}");
                    textContent.AppendLine($"  Font Size: {chunk.FontSize:F2} pt");

                    if (chunk.Color.Type.Equals("RGB", StringComparison.OrdinalIgnoreCase))
                        textContent.AppendLine($"  Color: RGB({chunk.Color.Values[0]:F0}, {chunk.Color.Values[1]:F0}, {chunk.Color.Values[2]:F0})");
                    else if (chunk.Color.Type.Equals("Gray", StringComparison.OrdinalIgnoreCase))
                        textContent.AppendLine($"  Color: Gray({chunk.Color.Values[0]:F0})");
                    else
                        textContent.AppendLine("  Color: Unknown");

                    textContent.AppendLine();
                }
            }
            else
            {
                foreach (var chunk in textChunks)
                {
                    textContent.AppendLine(chunk.Text);
                }
            }

            return Encoding.UTF8.GetBytes(textContent.ToString());
        }

        public List<TextStuff> ExtractTextChunks(Stream pdfStream)
        {
            List<TextStuff> chunky_monkeys = new List<TextStuff>();
            try
            {
                using (PdfReader reader = new PdfReader(pdfStream))
                {
                    for (int pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++)
                    {
                        var custom_strategy = new CustomTextExtractionStrategy();
                        PdfTextExtractor.GetTextFromPage(reader, pageNum, custom_strategy);
                        chunky_monkeys.AddRange(custom_strategy.TextChunks);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error. PDF could not be converted. Message: {ex.Message}", ex);
            }

            return chunky_monkeys;
        }
    }
    public class CustomTextExtractionStrategy : ITextExtractionStrategy
    {
        public List<TextStuff> TextChunks { get; } = new List<TextStuff>();

        public virtual void BeginTextBlock() { }
        public virtual void EndTextBlock() { }
        public virtual void RenderImage(ImageRenderInfo renderInfo) { }

        public virtual void RenderText(TextRenderInfo renderInfo)
        {
            foreach (var charInfo in renderInfo.GetCharacterRenderInfos())
            {
                //Grab each char
                string text = charInfo.GetText();
                if (!string.IsNullOrEmpty(text))
                {
                    Vector baseline = charInfo.GetBaseline().GetStartPoint();
                    float fontSize = charInfo.GetAscentLine().GetStartPoint()[1] - charInfo.GetDescentLine().GetStartPoint()[1];
                    string fontName = charInfo.GetFont().PostscriptFontName ?? "Unknown";
                    ColorInfo colorInfo = null;

                    // Get color (RGB or Gray)
                    BaseColor fillColor = charInfo.GetFillColor();
                    if (fillColor is BaseColor rgb)
                    {
                        colorInfo = new ColorInfo
                        {
                            Type = "RGB",
                            Values = new float[] { rgb.R * 255, rgb.G * 255, rgb.B * 255 }
                        };
                    }
                    else if (fillColor is GrayColor gray)
                    {
                        colorInfo = new ColorInfo
                        {
                            Type = "Gray",
                            Values = new float[] { gray.Gray * 255 }
                        };
                    }

                    TextChunks.Add(new TextStuff
                    {
                        Text = text,
                        Position = new Point { X = baseline[0], Y = baseline[1] },
                        Font = fontName,
                        FontSize = fontSize,
                        Color = colorInfo
                    });
                }
            }
        }

        public virtual string GetResultantText()
        {
            StringBuilder text = new StringBuilder();
            foreach (var chunk in TextChunks)
            {
                text.Append(chunk.Text);
            }
            return text.ToString();
        }
    }
}