using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using WebAPIStuff.Services;
using System.Net;

namespace WebAPIStuff
{
    public partial class TryIt : Page
    {
        private readonly HttpClient _httpClient;

        public TryIt()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(handler);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44383/");
        }

        protected void TestUpload_Click(object sender, EventArgs e)
        {
            if (PdfUpload.HasFile && PdfUpload.FileName.EndsWith(".pdf"))
            {
                try
                {
                    PdfUploadHandler handler = new PdfUploadHandler();
                    Stream pdfStream = PdfUpload.PostedFile.InputStream;
                    bool isValid = handler.ValidatePdf(pdfStream);
                    UploadResult.Text = isValid ? "PDF is valid!" : "Invalid PDF.";
                }
                catch (Exception ex)
                {
                    UploadResult.Text = $"Error: {ex.Message}";
                    UploadResult.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                UploadResult.Text = "Please upload a valid PDF file.";
                UploadResult.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void TestService_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                if (PdfServiceUpload.HasFile && PdfServiceUpload.FileName.EndsWith(".pdf"))
                {
                    try
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            var fileContent = new StreamContent(PdfServiceUpload.PostedFile.InputStream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                            content.Add(fileContent, "file", PdfServiceUpload.FileName);
                            content.Add(new StringContent(IncludeFormatting.Checked.ToString().ToLower()), "includeFormatting");

                            ServiceResult.Text = "Sending request to api/pdf/to-text...";
                            var response = await _httpClient.PostAsync("api/pdf/to-text", content);
                            ServiceResult.Text = $"Response: {response.StatusCode}";
                            if (response.IsSuccessStatusCode)
                            {
                                var result = await response.Content.ReadAsStringAsync();
                                ServiceResult.Text = result.Replace("\n", "<br />");
                            }
                            else
                            {
                                ServiceResult.Text = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                                ServiceResult.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        ServiceResult.Text = $"HTTP Error: {ex.Message}. Inner: {ex.InnerException?.Message}";
                        ServiceResult.ForeColor = System.Drawing.Color.Red;
                    }
                    catch (Exception ex)
                    {
                        ServiceResult.Text = $"General Error: {ex.Message}. Inner: {ex.InnerException?.Message}";
                        ServiceResult.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    ServiceResult.Text = "Please upload a valid PDF file.";
                    ServiceResult.ForeColor = System.Drawing.Color.Red;
                }
            }));
        }
    }
}