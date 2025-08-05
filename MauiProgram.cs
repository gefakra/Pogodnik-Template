using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Text.Json;
using WeatherApp.Services;
using WeatherApp.Services.Interfaces;
using WeatherApp.ViewModels;

namespace WeatherApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
          .UseMauiApp<App>()
          .ConfigureFonts(f => {
              f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              f.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
              f.AddFont("Lora-Regular.ttf", "Lora");
              f.AddFont("Lora-MediumItalic.ttf", "LoraM");
          });

        // Конфигурация
        var cfgPath = Path.Combine(FileSystem.AppDataDirectory, "AppConfig.json");
        if (!File.Exists(cfgPath))
            WriteDefaultConfig(cfgPath);
        var config = JsonSerializer.Deserialize<AppConfiguration>(File.ReadAllText(cfgPath)) ?? new AppConfiguration();

        // DI
        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
        builder.Services.AddSingleton<IWeatherService, WeatherService>();
        builder.Services.AddSingleton<IStorageService, StorageService>();
        builder.Services.AddSingleton<INoteService, NoteService>();
        builder.Services.AddSingleton<ILocationService, LocationService>();
        


        builder.Services.AddDbContext<NotesDbContext>(options =>
        options.UseSqlite($"Filename={Path.Combine(FileSystem.AppDataDirectory, "notes_ef.db")}"));

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        builder.Services.AddTransient<NotesViewModel>();
        builder.Services.AddTransient<NotesPage>();

        builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddTransient<CalendarPage>();

        return builder.Build();
    }

    static void WriteDefaultConfig(string path)
    {
        using var stream = Assembly.GetExecutingAssembly()
          .GetManifestResourceStream("WeatherApp.Resources.Configuration.AppConfig.json");
        using var reader = new StreamReader(stream);
        File.WriteAllText(path, reader.ReadToEnd());
    }
}

