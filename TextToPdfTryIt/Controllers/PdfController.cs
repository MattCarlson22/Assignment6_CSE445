using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TextToPdfClassic.Models;
using TextToPdfClassic.Services;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Text;

namespace TextToPdfClassic.Controllers
{
    [RoutePrefix("api/pdf")]
    public class PdfController : ApiController
    {
        [HttpPost, Route("convert-file")]
        public async Task<HttpResponseMessage> ConvertFile()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return Request.CreateErrorResponse(
                        HttpStatusCode.UnsupportedMediaType,
                        "multipart/form-data required");

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                var file = provider.Contents.FirstOrDefault();
                if (file == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                                                       "File part missing");

                // --- guard: reject PDFs immediately ----------------------
                var first4 = await file.ReadAsByteArrayAsync();
                if (first4.Length >= 4 &&
                    first4[0] == 0x25 && first4[1] == 0x50 &&   // %PDF
                    first4[2] == 0x44 && first4[3] == 0x46)
                {
                    return Request.CreateErrorResponse(
                        HttpStatusCode.UnsupportedMediaType,
                        "This endpoint expects a *text* chunk file, not a PDF.");
                }

                // rewind
                file = new ByteArrayContent(first4);            // simple reset for demo
                file.Headers.ContentDisposition =
                    provider.Contents.First().Headers.ContentDisposition;

                var raw = Encoding.UTF8.GetString(first4);
                raw += await provider.Contents.First().ReadAsStringAsync();

                var chunks = ChunkParser.Parse(raw).ToList();
                if (!chunks.Any())
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                                                       "No valid text blocks were found.");

                var pdfBytes = ITextPdfGenerator.Generate(chunks);

                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdfBytes)
                };
                resp.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/pdf");
                resp.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    { FileName = "Generated.pdf" };
                return resp;
            }
            catch (Exception ex)
            {
                // 1. write the exception to VS output
                System.Diagnostics.Debug.WriteLine("### SERVER EXCEPTION ###");
                System.Diagnostics.Debug.WriteLine(ex);

                // 2. return it to the browser
                return Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    ex.ToString());
            }
        }


    }
}
