using System.Runtime.Serialization;

namespace TextChunkEditorService
{
    [DataContract]
    public class Position
    {
        [DataMember]
        public float X { get; set; }
        [DataMember]
        public float Y { get; set; }
    }
}