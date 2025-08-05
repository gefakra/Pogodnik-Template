using WeatherApp.ViewModels;
namespace WeatherApp;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
        BindingContext = viewModel;
    }

    private void OnSearchCompleted(object sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm && vm.GetWeatherCommand.CanExecute(null))
        {
            vm.GetWeatherCommand.Execute(null);
        }
    }
}




