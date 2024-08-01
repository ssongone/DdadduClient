using DdadduBot.ViewModels;

namespace DdadduBot
{
    public partial class ChoiceMenuPage : ContentPage
    { 
        private bool isBook;
        public ChoiceMenuPage(bool isBook)
        {
            InitializeComponent();
            this.isBook = isBook;
            BindingContext = new MenuViewModel(isBook);
        }

        private void ShowDetail(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button.BindingContext as Item;
            var viewModel = BindingContext as MenuViewModel;
            int index = viewModel.Items.IndexOf(item) + 1;

            Navigation.PushAsync(new DetailPage(isBook, index));
        }

    }
}
