using System.Collections.Generic;
using System.Text.RegularExpressions;
using TextToPdfClassic.Models;

namespace TextToPdfClassic.Services
{
    public static class ChunkParser
    {
        private static readonly Regex _chunkRx = new Regex(
        @"Text:\s*(?<text>.+?)\r?\n\s*Position:\s*\(X:\s*(?<x>[\d\.]+),\s*Y:\s*(?<y>[\d\.]+)\)\r?\n\s*Font:\s*(?<font>.+?)\r?\n\s*Font Size:\s*(?<size>[\d\.]+)\s*pt\r?\n\s*Color:\s*RGB\((?<r>\d+),\s*(?<g>\d+),\s*(?<b>\d+)\)",
        RegexOptions.Multiline | RegexOptions.Compiled);

        public static IEnumerable<TextChunk> Parse(string fileText)
        {
            foreach (Match m in _chunkRx.Matches(fileText))
            {
                yield return new TextChunk(
                    m.Groups["text"].Value.Trim(),
                    float.Parse(m.Groups["x"].Value),
                    float.Parse(m.Groups["y"].Value),
                    m.Groups["font"].Value.Trim(),
                    float.Parse(m.Groups["size"].Value),
                    byte.Parse(m.Groups["r"].Value),
                    byte.Parse(m.Groups["g"].Value),
                    byte.Parse(m.Groups["b"].Value));
            }
        }
    }
}
