using IntelliJ.Lang.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Services.Interfaces;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace WeatherApp.ViewModels
{
    /// <summary>
    /// ViewModel для главного экрана. Управляет погодной информацией и вводом города.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        readonly IWeatherService _weatherService;
        readonly IStorageService _storageService;

        public ObservableCollection<ForecastItem> Forecast { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand GetWeatherCommand { get; }
        public ICommand OpenCalendarCommand { get; }
        public ICommand OpenNotesCommand { get; }
        public ICommand LoadWeatherByLocationCommand { get; }

        string _cityName;
        public string CityName
        {
            get => _cityName;
            set
            {
                if (_cityName != value)
                {
                    _cityName = value;
                    OnPropertyChanged(nameof(CityName));
                }
            }
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value); 
        }
        private WeatherInfo _weather;
        public WeatherInfo Weather
        {
            get => _weather;
            set => SetProperty(ref _weather, value);
        }

        private WeatherInfo? _weatherInfo;
        public WeatherInfo? WeatherInfo    
        {
            get => _weatherInfo;
            set
            {
                if (SetProperty(ref _weatherInfo, value))
                    OnPropertyChanged(nameof(HasWeatherInfo));
            }

        }

        public bool HasWeatherInfo => WeatherInfo != null;

        public MainViewModel(IWeatherService weatherService, IStorageService storageService)
        {
            _weatherService = weatherService; _storageService = storageService;
            CityName = _storageService.GetLastCity();
            GetWeatherCommand = new Command(async () => await LoadWeatherAsync());
            OpenCalendarCommand = new Command(async () => await NavigateToCalendarAsync());
            OpenNotesCommand = new Command(async () => await NavigateToNotesAsync());
            LoadWeatherByLocationCommand = new Command(async () => await LoadWeatherByLocationAsync());
        }

        private async Task LoadWeatherAsync()
        {
            if (string.IsNullOrWhiteSpace(CityName)) return;
            WeatherInfo = await _weatherService.GetWeatherAsync(CityName);
            _storageService.SaveLastCity(WeatherInfo.City);

            var forecast = await _weatherService.GetForecastAsync(CityName);
            Forecast.Clear();
            foreach (var item in forecast)
                Forecast.Add(item);
        }

        private async Task NavigateToCalendarAsync()
        {
            await Shell.Current.GoToAsync("calendarpage");
        }

        private async Task NavigateToNotesAsync()
        {
            //await Shell.Current.GoToAsync("notespage");
            await Shell.Current.GoToAsync($"/notespage?mode=list");
        }

        private async Task LoadWeatherByLocationAsync()
        {
            try
            {
                IsBusy = true;

                var weather = await _weatherService.GetWeatherByLocationAsync();
                var forecast = await _weatherService.GetForecastByLocationAsync();

                Weather = weather;
                Forecast = new ObservableCollection<ForecastItem>(forecast);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await Shell.Current.DisplayAlert("Ошибка", "Не удалось определить погоду по геопозиции", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
