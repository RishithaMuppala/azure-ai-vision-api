using IntelligentDocumentAnalysisAPI.Types;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace IntelligentDocumentAnalysisAPI.Services
{
    public class ExtractHandWrittenTextService
    {
        private readonly AzureConfigClass _azureConfig;
        public string uriBase = "";
        public ExtractHandWrittenTextService(IOptions<AzureConfigClass> azureConfig)
        {
            _azureConfig = azureConfig.Value;
            uriBase = _azureConfig.EndPoint + "/vision/v3.1/read/analyze";
        }

        public async Task<string> ReadText(IFormFile image)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _azureConfig.ClientSecret);

                string url = uriBase;
                string operationLocation;

                byte[] byteData;

                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    byteData = memoryStream.ToArray();
                }

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                        operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                    else
                    {
                        string errorString = await response.Content.ReadAsStringAsync();
                        return JToken.Parse(errorString).ToString();
                    }
                }

                string contentString = "";
                string result;
                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    HttpResponseMessage response = await client.GetAsync(operationLocation);
                    result = await response.Content.ReadAsStringAsync();
                    ++i;
                }
                while (i < 60 && result.IndexOf("\"status\":\"succeeded\"") == -1);

                if (i == 60 && result.IndexOf("\"status\":\"succeeded\"") == -1)
                {
                    return "\nTimeout error.\n";
                }
                JToken parsedText = JToken.Parse(result);
                foreach(var readResult in parsedText["analyzeResult"]["readResults"].ToList()){
                    foreach(var line in readResult["lines"])
                    {
                        contentString += line["text"] + "\n";
                    }
                }
                return contentString.ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
