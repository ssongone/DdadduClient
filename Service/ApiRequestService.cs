using ScraperDll.Entity;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace DdadduBot.Service
{
    public class ApiRequestService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public ApiRequestService()
        {
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
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
            var body = JsonConvert.SerializeObject(new { fileUrls = publications.Select(p => p.MainImageUrl) });
            var request = new HttpRequestMessage(HttpMethod.Post, "http://donothing.store/api/upload")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            return await _httpClient.SendAsync(request);
        }



        public async Task<(bool, List<PublicationSummary>)>  ValidatePublications(ObservableCollection<PublicationSummaryDto> dtos)
        {
            var body = JsonConvert.SerializeObject(new { Title = dtos.Select(dto => dto.PublicationSummary.Title) });
            var response = await SaveTitleAsync(body);

            bool result;
            var resultSummaries = new List<PublicationSummary>();
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var list = JsonConvert.DeserializeObject<StatusResponse>(jsonResponse).StatusList;

                for (int i = 0; i < list.Count; i++)
                {
                    dtos[i].Status = list[i];
                    if (list[i]=="")
                    {
                        resultSummaries.Add(dtos[i].PublicationSummary);
                    }
                }
                result = true;
            }
            else
            {
                result = false;
                resultSummaries = dtos.Select(dto => dto.PublicationSummary).ToList();
            }
            return (result, resultSummaries);
        }

        public async Task<HttpResponseMessage> SaveTitleAsync(string titles)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://donothing.store/api/validate")
            {
                Content = new StringContent(titles, Encoding.UTF8, "application/json")
            };

            return await _httpClient.SendAsync(request);
        }
    }

    public class PublicationSummaryDto : INotifyPropertyChanged
    {
        public int Number { get; set; }
        private string _status;

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public PublicationSummary PublicationSummary { get; set; }

        public PublicationSummaryDto() { }

        public PublicationSummaryDto(int number, PublicationSummary publication)
        {
            Number = number;
            Status = "";
            PublicationSummary = publication;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UrlsResponse
    {
        public List<string> Urls { get; set; }
    }

    public class StatusResponse
    {
        public List<string> StatusList { get; set; }
    }
}
