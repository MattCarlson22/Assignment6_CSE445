using System.Web;

namespace TextToPdfClassic.Models
{
    public class ChunkUploadModel
    {
        public HttpPostedFileBase File { get; set; }
    }
}
