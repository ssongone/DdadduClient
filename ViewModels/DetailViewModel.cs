﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DdadduBot.Service;
using NPOI.OpenXmlFormats.Dml.Chart;
using ScraperDll.Entity;

namespace DdadduBot.ViewModels
{
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

        private ScraperService _scraperService;
        private ApiRequestService _apiRequestService = new ApiRequestService();

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

        private string statusMessage;
        public string StatusMessage
        {
            get => statusMessage;
            set
            {
                statusMessage = value;
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
            _scraperService = new ScraperService(isBook, option);
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
            StatusMessage = "목록을 가져오는 중";
            await LoadItems();

            // 2. 중복 확인 (서버 연동)
            StatusMessage = "목록을 확인하는 중";
            var (success, summaries) = await _apiRequestService.ValidatePublications(Items);
            
            // 3. 상품디테일 가져오기
            StatusMessage = "상세 정보를 가져오는 중";
            var publications = await _scraperService.GetPublications(summaries);

            // 4. 사진 확장자 변경 (서버 연동)
            StatusMessage = "사진 확장자를 변경하는 중";
            success = await _apiRequestService.UpdateMainImageUrls(publications);
            if (!success)
            {
                StatusMessage = "오류 발생 🙊 ";
                IsRunning = false;
                return;
            }

            // 5. 엑셀파일화
            StatusMessage = "엑셀 파일로 바꾸는 중";
            await Task.Run(() => _scraperService.ExportPublications(publications));

            StatusMessage = "✅✅✅";
            IsRunning = false;
        }

        private async Task LoadItems()
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
            return await _scraperService.GetPublicationSummaries();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
