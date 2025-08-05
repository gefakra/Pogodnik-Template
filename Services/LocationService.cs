using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Services.Interfaces;

namespace WeatherApp.Services
{
    public class LocationService : ILocationService
    {
        public async Task<string?> GetCityAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync()
                              ?? await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

                if (location == null)
                    return null;

                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                return placemarks?.FirstOrDefault()?.Locality;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка геолокации: {ex.Message}");
                return null;
            }
        }
    }
}
