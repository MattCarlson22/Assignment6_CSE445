using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace TextChunkEditorService
{
    [ServiceContract]
    public interface ITextChunkService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "upload", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        UploadResponse UploadFile(string fileName, string content);

        [OperationContract]
        [WebGet(UriTemplate = "chunks/{sessionId}", ResponseFormat = WebMessageFormat.Json)]
        List<TextChunk> GetChunks(string sessionId);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "edit/{sessionId}/{index}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string EditChunk(string sessionId, string index, string newText);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "delete/{sessionId}/{index}", ResponseFormat = WebMessageFormat.Json)]
        string DeleteChunk(string sessionId, string index);

        [OperationContract]
        [WebGet(UriTemplate = "save/{sessionId}", ResponseFormat = WebMessageFormat.Json)]
        SaveResponse SaveFile(string sessionId);
    }

    [DataContract]
    public class UploadResponse
    {
        [DataMember]
        public string SessionId { get; set; }
        [DataMember]
        public string Error { get; set; }
    }

    [DataContract]
    public class SaveResponse
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string Error { get; set; }
    }
}