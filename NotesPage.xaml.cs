using WeatherApp.ViewModels;

namespace WeatherApp;

[QueryProperty(nameof(Mode), "mode")]
[QueryProperty(nameof(City), "city")]
[QueryProperty(nameof(Date), "date")]
public partial class NotesPage : ContentPage
{
    public string Mode
    {
        set
        {
            if (BindingContext is NotesViewModel vm)
                vm.Mode = value;    // "list" или "create"
        }
    }

    string _city;
    public string City
    {
        get => _city;
        set
        {
            _city = value;
            if (BindingContext is NotesViewModel vm)
                vm.SetCity(value);
        }
    }
    public string Date
    {
        set
        {
            if (DateTime.TryParse(value, out var parsedDate))
            {
                if (BindingContext is NotesViewModel vm)
                    vm.SelectedDate = parsedDate;
            }
        }
    }


    public NotesPage(NotesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}