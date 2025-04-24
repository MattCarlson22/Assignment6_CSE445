using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace TextChunkEditorService
{
    public class TextChunkService : ITextChunkService
    {
        private static readonly Dictionary<string, List<TextChunk>> Sessions = new Dictionary<string, List<TextChunk>>();
        private static readonly Dictionary<string, string> FileNames = new Dictionary<string, string>();

        public UploadResponse UploadFile(string fileName, string content)
        {
            if (string.IsNullOrEmpty(fileName) || !fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                return new UploadResponse { Error = "Please upload a .txt file." };
            }

            if (string.IsNullOrEmpty(content))
            {
                return new UploadResponse { Error = "File content is empty." };
            }

            if (content.Length > 1024 * 1024)
            {
                return new UploadResponse { Error = "File is too large (max 1MB)." };
            }

            var chunks = ParseTextChunks(content);
            if (chunks.Count == 0)
            {
                return new UploadResponse { Error = "No valid text chunks found. Please check the file format." };
            }

            var sessionId = Guid.NewGuid().ToString();
            Sessions[sessionId] = chunks;
            FileNames[sessionId] = fileName;
            return new UploadResponse { SessionId = sessionId };
        }

        public List<TextChunk> GetChunks(string sessionId)
        {
            return Sessions.ContainsKey(sessionId) ? Sessions[sessionId] : new List<TextChunk>();
        }

        public string EditChunk(string sessionId, string index, string newText)
        {
            if (!Sessions.ContainsKey(sessionId))
            {
                return "Session not found.";
            }

            if (!int.TryParse(index, out var idx) || idx < 0 || idx >= Sessions[sessionId].Count)
            {
                return "Invalid chunk index.";
            }

            Sessions[sessionId][idx].Text = newText ?? string.Empty;
            return null;
        }

        public string DeleteChunk(string sessionId, string index)
        {
            if (!Sessions.ContainsKey(sessionId))
            {
                return "Session not found.";
            }

            if (!int.TryParse(index, out var idx) || idx < 0 || idx >= Sessions[sessionId].Count)
            {
                return "Invalid chunk index.";
            }

            Sessions[sessionId].RemoveAt(idx);
            return null;
        }

        public SaveResponse SaveFile(string sessionId)
        {
            if (!Sessions.ContainsKey(sessionId) || Sessions[sessionId].Count == 0)
            {
                return new SaveResponse { Error = "No chunks to save." };
            }

            var chunks = Sessions[sessionId];
            var fileName = FileNames.ContainsKey(sessionId) ? $"modified_{FileNames[sessionId]}" : "modified_textchunks.txt";
            var content = GenerateOutputFile(chunks);
            return new SaveResponse { FileName = fileName, Content = content };
        }

        private List<TextChunk> ParseTextChunks(string content)
        {
            var chunks = new List<TextChunk>();
            var lines = content.Split('\n').Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line)).ToList();
            int currentPage = 0;
            TextChunk currentChunk = null;

            var pageRegex = new Regex(@"=== Page (\d+) ===");
            var textRegex = new Regex(@"^Text: (.*)$");
            var positionRegex = new Regex(@"Position: \(X: ([\d.]+),\s*Y: ([\d.]+)\)");
            var fontRegex = new Regex(@"^Font: (.*)$");
            var fontSizeRegex = new Regex(@"Font Size: ([\d.]+)\s*pt");
            var colorRegex = new Regex(@"^Color: (.*)$");

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var pageMatch = pageRegex.Match(line);
                if (pageMatch.Success)
                {
                    currentPage = int.Parse(pageMatch.Groups[1].Value);
                    continue;
                }

                var textMatch = textRegex.Match(line);
                if (textMatch.Success)
                {
                    if (currentChunk != null && IsValidChunk(currentChunk))
                    {
                        chunks.Add(currentChunk);
                    }
                    currentChunk = new TextChunk { Page = currentPage, Text = textMatch.Groups[1].Value ?? string.Empty, Font = "Unknown", Color = "Unknown" };
                    continue;
                }

                var positionMatch = positionRegex.Match(line);
                if (positionMatch.Success && currentChunk != null)
                {
                    currentChunk.Position = new Position
                    {
                        X = float.TryParse(positionMatch.Groups[1].Value, out var x) ? x : 0,
                        Y = float.TryParse(positionMatch.Groups[2].Value, out var y) ? y : 0
                    };
                    continue;
                }

                var fontMatch = fontRegex.Match(line);
                if (fontMatch.Success && currentChunk != null)
                {
                    currentChunk.Font = fontMatch.Groups[1].Value ?? "Unknown";
                    continue;
                }

                var fontSizeMatch = fontSizeRegex.Match(line);
                if (fontSizeMatch.Success && currentChunk != null)
                {
                    currentChunk.FontSize = float.TryParse(fontSizeMatch.Groups[1].Value, out var size) ? size : 0;
                    continue;
                }

                var colorMatch = colorRegex.Match(line);
                if (colorMatch.Success && currentChunk != null)
                {
                    currentChunk.Color = colorMatch.Groups[1].Value ?? "Unknown";
                    if (IsValidChunk(currentChunk))
                    {
                        chunks.Add(currentChunk);
                    }
                    currentChunk = null;
                    continue;
                }
            }

            if (currentChunk != null && IsValidChunk(currentChunk))
            {
                chunks.Add(currentChunk);
            }

            return chunks;
        }

        private bool IsValidChunk(TextChunk chunk)
        {
            return chunk != null && !string.IsNullOrEmpty(chunk.Text) && chunk.Position != null && chunk.FontSize >= 0;
        }

        private string GenerateOutputFile(List<TextChunk> chunks)
        {
            var output = new System.Text.StringBuilder();
            int currentPage = 0;

            foreach (var chunk in chunks)
            {
                if (chunk.Page != currentPage)
                {
                    currentPage = chunk.Page;
                    output.AppendLine($"=== Page {currentPage} ===");
                }
                output.AppendLine($"Text: {chunk.Text}");
                output.AppendLine($"  Position: (X: {chunk.Position.X:F2}, Y: {chunk.Position.Y:F2})");
                output.AppendLine($"  Font: {chunk.Font}");
                output.AppendLine($"  Font Size: {chunk.FontSize:F2} pt");
                output.AppendLine($"  Color: {chunk.Color}");
                output.AppendLine();
            }

            return output.ToString();
        }
    }
}