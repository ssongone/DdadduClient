using System.Windows.Input;
using ScraperDll.Entity;

namespace DdadduBot.ViewModels
{
    public class SettingsViewModel
    {
        public string DefaultImageUrl { get; set; }
        public string BookMargin { get; set; }
        public string MagazineMargin { get; set; }

        public ICommand SaveCommand { get; }

        public SettingsViewModel()
        {
            DefaultImageUrl = ScraperConfig.DEFAULT_IMAGE_URL;
            BookMargin = ScraperConfig.BOOK_MARGIN.ToString();
            MagazineMargin = ScraperConfig.MAGAZINE_MARGIN.ToString();

            SaveCommand = new Command(SaveSettings);
        }

        private async void SaveSettings()
        {
            if (string.IsNullOrWhiteSpace(DefaultImageUrl))
            {
                await Application.Current.MainPage.DisplayAlert("Error!", "기본 이미지 주소를 확인하세요", "확인");
                return;
            }

            if (!decimal.TryParse(BookMargin, out decimal bookMargin) || bookMargin < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", "책 마진이 이상합니다", "확인");
                return;
            }

            if (!decimal.TryParse(MagazineMargin, out decimal magazineMargin) || magazineMargin < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", "잡지 마진이 이상합니다", "확인");
                return;
            }

            ScraperConfig.DEFAULT_IMAGE_URL = DefaultImageUrl;
            ScraperConfig.BOOK_MARGIN = bookMargin;
            ScraperConfig.MAGAZINE_MARGIN = magazineMargin;

            await Application.Current.MainPage.DisplayAlert("저장 완료", "설정이 저장되었습니다.", "확인");
        }
    }
}
