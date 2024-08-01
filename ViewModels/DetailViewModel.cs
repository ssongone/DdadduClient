using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ScraperDll;
using ScraperDll.Entity;

namespace DdadduBot.ViewModels
{
    public class DetailViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PublicationSummary> Items { get; set; }
        public ICommand LoadItemsCommand { get; }

        public bool IsBook { get; }
        public int Option { get; }

        public DetailViewModel()
        {
            Items = new ObservableCollection<PublicationSummary>();
            LoadItemsCommand = new Command(LoadItems);
        }

        public DetailViewModel(bool isBook, int option)
        {
            Items = new ObservableCollection<PublicationSummary>();
            LoadItemsCommand = new Command(LoadItems);
            IsBook = isBook;
            Option = option;
        }

        private async void LoadItems()
        {
            Items.Clear();

            var newItems = await GetNewItems();
            foreach (var item in newItems)
            {
                Items.Add(item);
            }
        }

        private async Task<List<PublicationSummary>> GetNewItems()
        {
            ListUrlPolicy policy = IsBook ? new BookUrlPolicy() : new MagazineUrlPolicy();
            Scraper scraper = new Scraper(policy);
            Debug.WriteLine("✅✅✅✅✅✅✅✅✅✅");
            Debug.WriteLine(policy.ToString());
            Debug.WriteLine(Option);
            List<PublicationSummary> list = await scraper.ScrapeRankingPage(Option);
            Debug.WriteLine(list[0].Name);
            return list;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
