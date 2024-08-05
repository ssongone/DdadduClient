using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DdadduBot.Service;
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
        public DetailViewModel() { }

        private DetailViewModel(bool isBook, int option)
        {
            Items = new ObservableCollection<PublicationSummaryDto>();
            LoadItemsCommand = new Command(LoadItems);
            IsBook = isBook;
            Option = option;
        }

        private async void LoadItems()
        {
            if (ScraperServiceManager.ServiceExists(IsBook, Option))
            {
                await Application.Current.MainPage.DisplayAlert("경고", "현재 작업이 진행 중입니다.", "확인");
                return;
            }

            IsLoading = true;
            Items.Clear();

            var newItems = await GetNewItems();
            for (int i = 0; i < newItems.Count; i++)
            {
                Items.Add(new PublicationSummaryDto(i+1, newItems[i]));
            }

            IsLoading = false;
            ScraperServiceManager.RemoveService(IsBook, Option);
        }

        private async Task<List<PublicationSummary>> GetNewItems()
        {
            return await ScraperServiceManager.GetService(IsBook, Option).GetPublicationSummaries();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
