namespace DdadduBot;
using DdadduBot.ViewModels;
using ScraperDll;

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

	public DetailPage(bool isBook, int option)
	{
		InitializeComponent();
		BindingContext = DetailViewModel.GetInstance(isBook, option);
	}

}