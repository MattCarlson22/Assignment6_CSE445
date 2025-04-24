using System.Runtime.Serialization;

namespace TextChunkEditorService
{
    [DataContract]
    public class TextChunk
    {
        [DataMember]
        public int Page { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public Position Position { get; set; }
        [DataMember]
        public string Font { get; set; }
        [DataMember]
        public float FontSize { get; set; }
        [DataMember]
        public string Color { get; set; }
    }
}