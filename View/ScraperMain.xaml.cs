namespace DdadduBot;

public partial class ScraperMain : ContentPage
{
	public ScraperMain()
	{
		InitializeComponent();
	}

    private void OnButtonClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ChoiceMenuPage(true));
    }

    private void OnButtonClicked2(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ChoiceMenuPage(false));
    }
}