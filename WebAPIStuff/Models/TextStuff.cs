using System.Runtime.Serialization;

namespace WebAPIStuff.Models
{
    [DataContract]
    public class Point
    {
        [DataMember]
        public float X { get; set; }
        [DataMember]
        public float Y { get; set; }
    }

    [DataContract]
    public class ColorInfo
    {
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public float[] Values { get; set; }
    }

    [DataContract]
    public class TextStuff
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public Point Position { get; set; }
        [DataMember]
        public string Font { get; set; }
        [DataMember]
        public float FontSize { get; set; }
        [DataMember]
        public ColorInfo Color { get; set; }
    }


}
