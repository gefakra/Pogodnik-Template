//using Android.Net;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using WeatherApp.Models;
using WeatherApp.Services.Interfaces;
using Microsoft.Maui.Devices.Sensors;

namespace WeatherApp.Services;

/// <summary>
/// Сервис получения данных о погоде через OpenWeatherMap API.
/// </summary>
public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    private readonly IConfigurationService _configService;
    private readonly ILocationService _locationService;
    public WeatherService(IConfigurationService configService, HttpClient httpClient, ILocationService locationService)
    {
        _configService = configService;
        _httpClient = httpClient;
        _locationService = locationService;
    }

    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {        
        var url = $"{_configService.GetApiBaseUrl()}weather?q={city}&appid={_configService.GetApiKey()}&units=metric&lang=ru";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Ошибка при запросе погоды: {response.StatusCode}");

        var data = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
        if (data == null)
            throw new Exception("Ошибка разбора ответа API.");

        return new WeatherInfo
        {
            Temperature = (int)data.Main.Temp,
            Description = data.Weather[0].Description,
            Icon = GetIconName(data.Weather[0].Main),
            City = data.Name
        };
    }
    public async Task<WeatherInfo> GetWeatherByLocationAsync()
    {
        var city = await _locationService.GetCityAsync();
        if (string.IsNullOrWhiteSpace(city))
            throw new Exception("Город не определён по геолокации.");

        return await GetWeatherAsync(city);
    }

    public async Task<List<ForecastItem>> GetForecastByLocationAsync()
    {
        var city = await _locationService.GetCityAsync();
        if (string.IsNullOrWhiteSpace(city))
            throw new Exception("Город не определён по геолокации.");

        return await GetForecastAsync(city);
    }


    public async Task<List<ForecastItem>> GetForecastAsync(string city)
    {
        var forecastItems = new List<ForecastItem>();

        var url = $"{_configService.GetApiBaseUrl()}forecast?q={Uri.EscapeDataString(city)}&appid={_configService.GetApiKey()}&units=metric&lang=ru";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Ошибка при запросе прогноза: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);

        var list = json["list"];
        if (list == null || !list.Any()) return forecastItems;

        var grouped = list
            .GroupBy(i => DateTime.Parse(i["dt_txt"]!.ToString()).Date)
            .Take(5); // прогноз на 5 дней

        foreach (var day in grouped)
        {
            var noon = day
                .OrderBy(x => Math.Abs(DateTime.Parse(x["dt_txt"]!.ToString()).Hour - 12))
                .First();

            var date = DateTime.Parse(noon["dt_txt"]!.ToString()).ToString("ddd, dd MMM", new CultureInfo("ru-RU"));

            forecastItems.Add(new ForecastItem
            {
                Date = date,
                Icon = GetIconName(noon["weather"]?[0]?["main"]?.ToString() ?? ""),
                MinTemp = day.Min(i => i["main"]?["temp_min"]?.Value<double>() ?? 0),
                MaxTemp = day.Max(i => i["main"]?["temp_max"]?.Value<double>() ?? 0)
            });
        }

        return forecastItems;
    }

    private static string GetIconName(string condition) => condition switch
    {
        "Clouds" => "cloudy.png",
        "Rain" => "rain.png",
        "Clear" => "sunny.png",
        _ => "standard.png"
    };   
}
