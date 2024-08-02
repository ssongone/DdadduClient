using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ScraperDll;
using ScraperDll.Entity;

namespace DdadduBot.ViewModels
{

    public class PublicationSummaryDto
    {
        public int Number { get; set; }
        public string Status { get; set; }
        public PublicationSummary PublicationSummary { get; set; }

        public PublicationSummaryDto() { }

        public PublicationSummaryDto(int number, PublicationSummary publication) 
        { 
            Number = number;
            Status = "중복";
            PublicationSummary = publication;
        }
    }

    public class DetailViewModel : INotifyPropertyChanged
    {
        private static readonly Dictionary<(bool, int), DetailViewModel> _instances = new();
        public static DetailViewModel GetInstance(bool isBook, int option)
        {
            if (!_instances.ContainsKey((isBook, option)))
            {
                _instances[(isBook, option)] = new DetailViewModel(isBook, option);
            }
            return _instances[(isBook, option)];
        }

        public DetailViewModel()
        {}

        public ObservableCollection<PublicationSummaryDto> Items { get; set; }
        public ICommand LoadItemsCommand { get; }

        public bool IsBook { get; }
        public int Option { get; }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }

        private DetailViewModel(bool isBook, int option)
        {
            Items = new ObservableCollection<PublicationSummaryDto>();
            LoadItemsCommand = new Command(LoadItems);
            IsBook = isBook;
            Option = option;
        }

        private async void LoadItems()
        {
            IsLoading = true;

            Items.Clear();

            var newItems = await GetNewItems();
            for (int i = 0; i < newItems.Count; i++)
            {
                Items.Add(new PublicationSummaryDto(i+1, newItems[i]));
            }
            IsLoading = false;
        }

        private async Task<List<PublicationSummary>> GetNewItems()
        {
            ListUrlPolicy policy = IsBook ? new BookUrlPolicy() : new MagazineUrlPolicy();
            Scraper scraper = new Scraper(policy);
            Debug.WriteLine("✅✅✅✅✅✅✅✅✅✅");
            List<PublicationSummary> list = await scraper.ScrapeRankingPage(Option);
            Debug.WriteLine(list.Count);
            return list;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
