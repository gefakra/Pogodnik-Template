using WeatherApp.ViewModels;

namespace WeatherApp;

public partial class CalendarPage : ContentPage
{
	public CalendarPage(CalendarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}