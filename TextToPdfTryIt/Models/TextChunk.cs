namespace TextToPdfClassic.Models
{
    public class TextChunk
    {
        public TextChunk(string text,
                         float x, float y,
                         string fontName,
                         float fontSize,
                         byte r, byte g, byte b)
        {
            Text = text;
            X = x;
            Y = y;
            FontName = fontName;
            FontSize = fontSize;
            R = r; G = g; B = b;
        }

        public string Text { get; }
        public float X { get; }
        public float Y { get; }
        public string FontName { get; }
        public float FontSize { get; }
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
    }
}
