using DdadduBot.ViewModels;

namespace DdadduBot
{
    public partial class Hi : ContentPage
    {
        public Hi()
        {
            InitializeComponent();
            BindingContext = new MyViewModel();
        }

        private async void ShowDetail(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button.BindingContext as Item;
            var viewModel = BindingContext as MyViewModel;
            int index = viewModel.Items.IndexOf(item)+1;

            await Navigation.PushAsync(new DetailPage());
        }

    }
}
