using ScraperDll.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdadduBot.Service
{
    public class ApiRequestService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public List<Publication> Publications { get; set; }

        public ApiRequestService(List<Publication> publications)
        {
            Publications = publications;
        }
        public string CreateFileUrlsBody()
        {
            var fileUrls = new List<string>();

            foreach (var publication in Publications)
            {
                fileUrls.Add($"fileUrls={publication.MainImageUrl}");
            }

            return string.Join("&", fileUrls);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> UploadFilesAsync(List<Publication> publications)
        {
            var body = CreateFileUrlsBody();

            var request = new HttpRequestMessage(HttpMethod.Post, "http://donothing.store/api/upload")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            return await _httpClient.SendAsync(request);
        }
    }
}
