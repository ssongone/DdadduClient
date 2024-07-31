namespace DdadduBot;
using DdadduBot.ViewModels;

public partial class DetailPage : ContentPage
{
	public DetailPage(Item item)
	{
		InitializeComponent();
		BindingContext = item;
	}

	public DetailPage()
	{
        InitializeComponent();
    }
}