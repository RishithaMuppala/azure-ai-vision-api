using IntelligentDocumentAnalysisAPI.Types;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace IntelligentDocumentAnalysisAPI.Services
{
    public class ExtractPrintedText
    {
        private readonly AzureConfigClass _azureConfig;
        public string uriBase = "";
        public ExtractPrintedText(IOptions<AzureConfigClass> azureConfig)
        {
            _azureConfig = azureConfig.Value;
            uriBase = _azureConfig.EndPoint + "vision/v2.1/ocr";
        }

        /// <summary>
        /// Gets the text visible in the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with printed text.</param>
        public async Task<string> MakeOCRRequest(IFormFile image)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", _azureConfig.ClientSecret);

                string requestParameters = "language=unk&detectOrientation=true";

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
                JToken parsedText = JToken.Parse(contentString);
                string result = "";
                foreach(var region in parsedText["regions"]){
                    foreach(var line in region["lines"])
                    {
                        foreach(var word in line["words"])
                        {
                            result += word["text"] + " ";
                        }
                        result += "\n";
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
