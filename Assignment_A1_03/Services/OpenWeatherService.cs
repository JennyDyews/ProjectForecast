using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {      

        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "f4604e9e8a94b2bf9f4e1bc2fe7f0b56"; // Your API Key

        // part of your event and cache code here
        ConcurrentDictionary<(string, string), Forecast> _Cache1 = new ConcurrentDictionary<(string, string), Forecast>();
        ConcurrentDictionary<(string, double, double), Forecast> _Cache2 = new ConcurrentDictionary<(string, double, double), Forecast>();

        public EventHandler<string> WeatherForecastAvailable;
        public async Task<Forecast> GetForecastAsync(string City)
        {
            //part of cache code here
            Forecast forecast = null;

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string city = City;
            var key = (date, city);

            if (!_Cache1.TryGetValue(key, out forecast))
            {
                //https://openweathermap.org/current
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";



                forecast = await ReadWebApiAsync(uri);
                _Cache1[key] = forecast;
                OnWeatherForecastAvailable($"New weather forecast for {City} available");
            }
            else
                OnWeatherForecastAvailable($"Cached weather forecast for {City} available");

            return forecast;

        }

       // // https://openweathermap.org/current
       // var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
       //     var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

       //     Forecast forecast = await ReadWebApiAsync(uri);

       //     OnWeatherForecastAvailable($"New weather forecast for {City} is available");
       //     return forecast;

       //     //part of event and cache code here
       //     //generate an event with different message if cached data
       //}
        protected virtual void OnWeatherForecastAvailable(string s)
        {
            WeatherForecastAvailable?.Invoke(this, s);
        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //part of cache code here

            Forecast forecast = null;
            var longit = longitude;
            var latid = latitude;
            var date1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            var key = (date1, longit, latid);
            if (!_Cache2.TryGetValue(key, out forecast))
            {
                //https://openweathermap.org/current
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

                forecast = await ReadWebApiAsync(uri);
                _Cache2[key] = forecast;
                OnWeatherForecastAvailable($"New weather forecast for {longitude}{latitude}");
            }
            else
            {
                OnWeatherForecastAvailable($"Cached weather forecast for ({longitude} {latitude}) available");
            }

            //part of event and cache code here
            //generate an event with different message if cached data

            return forecast;
        }

        private ForecastItem GetForecastItem(List wdListItem)
        {

            ForecastItem item = new ForecastItem();
            item.DateTime = UnixTimeStampToDateTime(wdListItem.dt);

            item.Temperature = wdListItem.main.temp;
            item.Description = wdListItem.weather.Count > 0 ? wdListItem.weather.First().description : "No info";
            item.WindSpeed = wdListItem.wind.speed;

            return item;

        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            // part of your read web api code here

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            //Your Code to convert WeatherApiData to Forecast using Linq.
            Forecast forecast = new Forecast();

            forecast.City = wd.city.name;


            forecast.Items = new List<ForecastItem>();

            wd.list.ForEach(wdListItem => { forecast.Items.Add(GetForecastItem(wdListItem)); });


            return forecast;

        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }

}
