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

        private bool isRunning;
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                OnPropertyChanged();
            }
        }
        public DetailViewModel() { }

        private DetailViewModel(bool isBook, int option)
        {
            Items = new ObservableCollection<PublicationSummaryDto>();
            LoadItemsCommand = new Command(Run);
            IsBook = isBook;
            Option = option;
        }

        private async void Run()
        {
            if (IsRunning)
            {
                await Application.Current.MainPage.DisplayAlert("경고", "현재 작업이 진행 중입니다.", "확인");
                return;
            }

            IsRunning = true;
            // 1. 리스트 로드
            LoadItems();

            // 2. 중복 확인 (서버 연동)
            // 3. 상품디테일 가져오기
            // 4. 사진 확장자 변경 (서버 연동)
            // 5. 엑셀파일화
            

            IsRunning = false;
        }

        private async void LoadItems()
        {
            Items.Clear();
            var newItems = await GetNewItems();
            for (int i = 0; i < newItems.Count; i++)
            {
                Items.Add(new PublicationSummaryDto(i+1, newItems[i]));
            }
        }

        private async Task<List<PublicationSummary>> GetNewItems()
        {
            return await new ScraperService(IsBook, Option).GetPublicationSummaries();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
