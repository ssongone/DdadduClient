using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ScraperDll;
using Microsoft.Maui.Controls;
using ScraperDll.Entity;

namespace DdadduBot.ViewModels
{
    public class DetailViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PublicationSummary> Items { get; set; }
        public ICommand LoadItemsCommand { get; }

        public DetailViewModel()
        {
            Items = new ObservableCollection<PublicationSummary>();
            LoadItemsCommand = new Command(LoadItems);
        }

        private async void LoadItems()
        {
            Items.Clear();

            // 여기에 실제 데이터를 가져오는 로직을 추가할 수 있습니다.
            var newItems = await GetNewItems();
            foreach (var item in newItems)
            {
                Items.Add(item);
            }
        }

        private async Task<List<PublicationSummary>> GetNewItems()
        {
            Scraper scraper = new Scraper(new MagazineUrlPolicy());
            List<PublicationSummary> magazineList = await scraper.ScrapeRankingPage(0);
            // 여기서 실제 데이터를 가져옵니다.
            return magazineList;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
