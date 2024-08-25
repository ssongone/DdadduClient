using ScraperDll.Entity;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;

namespace DdadduBot.Service
{
    public class ApiRequestService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public ApiRequestService()
        {
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }
        public string CreateFileUrlsBody(List<Publication> publications)
        {
            var fileUrls = new List<string>();

            for (int i = 0 ; i < publications.Count; i++)
            {
                fileUrls.Add($"fileUrls={publications[i].MainImageUrl}");
            }

            return string.Join("&", fileUrls);
        }

        public async Task<bool> UpdateMainImageUrls(List<Publication> publications)
        {
            var response = await UploadFilesAsync(publications);

            Debug.Write(response);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var urls = JsonConvert.DeserializeObject<UrlsResponse>(jsonResponse).Urls;

                for (int i = 0; i < urls.Count; i++)
                {
                    publications[i].MainImageUrl = urls[i];
                }

                return true;
            }
            else
            {
                return false;
            }
        }



        public async Task<HttpResponseMessage> UploadFilesAsync(List<Publication> publications)
        {
            var body = CreateFileUrlsBody(publications);

            var request = new HttpRequestMessage(HttpMethod.Post, "http://donothing.store/api/upload")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            return await _httpClient.SendAsync(request);
        }

        public class UrlsResponse
        {
            public List<string> Urls { get; set; }
        }


        public async Task  ValidatePublications(List<PublicationSummary> summaries)
        {

        }
    }
}
