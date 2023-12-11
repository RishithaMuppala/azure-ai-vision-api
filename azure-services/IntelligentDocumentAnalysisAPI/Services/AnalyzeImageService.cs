using IntelligentDocumentAnalysisAPI.Types;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace IntelligentDocumentAnalysisAPI.Services
{
    public class AnalyzeImageService
    {
        private readonly AzureConfigClass _azureConfig;
        public string uriBase = "";
        public AnalyzeImageService(IOptions<AzureConfigClass> azureConfig)
        {
            _azureConfig = azureConfig.Value;
            uriBase = _azureConfig.EndPoint + "vision/v3.1/analyze";
        }
       
        /// <summary>
        /// Gets the analysis of the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="image">The image file to analyze.</param>
        public async Task<string> MakeAnalysisRequest(IFormFile image)
        {
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", _azureConfig.ClientSecret);

                string requestParameters =
                    "visualFeatures=Categories,Description,Color";

                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                byte[] byteData;

                string contentString = "";

                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 2097152)
                    {
                        byteData = memoryStream.ToArray();
                        using (ByteArrayContent content = new ByteArrayContent(byteData))
                        {
                            content.Headers.ContentType =
                                new MediaTypeHeaderValue("application/octet-stream");

                            response = await client.PostAsync(uri, content);
                            contentString = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                return JToken.Parse(contentString).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
