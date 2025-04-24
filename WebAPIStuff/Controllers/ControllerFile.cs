using WebAPIStuff.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebAPIStuff.Controllers
{
    public class PdfToTextController : ApiController
    {
        private readonly PdfToTextService _pdfToTextService;

        public PdfToTextController(PdfToTextService pdfToTextService)
        {
            _pdfToTextService = pdfToTextService ?? throw new ArgumentNullException(nameof(pdfToTextService));
        }

        [HttpPost]
        [Route("api/pdf/to-text")]
        public IHttpActionResult ConvertPdfToText()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Expected multipart/form-data content.");

                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count == 0)
                    return BadRequest("No PDF file uploaded.");

                var pdfFile = httpRequest.Files[0];
                if (pdfFile == null || pdfFile.ContentLength == 0)
                    return BadRequest("Invalid or empty PDF file.");

                string includeFormattingStr = httpRequest.Form["includeFormatting"]?.ToLower();
                bool includeFormatting = includeFormattingStr == "true";

                using (var pdfStream = pdfFile.InputStream)
                {
                    byte[] textBytes = _pdfToTextService.ExtractTextToFile(pdfStream, includeFormatting);
                    var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(textBytes)
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = Path.ChangeExtension(pdfFile.FileName, ".txt")
                    };
                    return ResponseMessage(response);
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}