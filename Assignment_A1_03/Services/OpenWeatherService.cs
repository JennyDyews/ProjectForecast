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
        ConcurrentDictionary<(int, int), int> _Cache = new ConcurrentDictionary<(int, int), int>();//Lagt till denna 

        //public async Task DisplayPrimeCountsAsync() //LA TILL ALLT DETTA
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        //Create my Cache Key
        //        int start = i * 1_000_000 + 2;
        //        int count = 1_000_000;
        //        var key = (start, count);
        //        int nrOfPrimes = 0;
        //        if (!_Cache.TryGetValue(key, out nrOfPrimes))
        //        {
        //            //It did not exist in the cache, calculate and add it to the cache
        //            nrOfPrimes = await GetPrimesCountAsync(i * 1_000_000 + 2, 1_000_000);
        //            _Cache[key] = nrOfPrimes;
        //        }

        //        //Regardless if nrOfPrimes where in the cache - I now have it in nrOfPrimes
        //        var t = $"{nrOfPrimes} primes between {start} and {start + count}";
        //        Console.WriteLine(t);
        //    }

        //    Console.WriteLine("Done!");
        //}

        //public Task<int> GetPrimesCountAsync(int start, int count)
        //{
        //    return Task.Run(() =>
        //       Enumerable.Range(start, count).Count(n =>
        //         Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        //}
    

    public EventHandler<string> WeatherForecastAvailable;

        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "f4604e9e8a94b2bf9f4e1bc2fe7f0b56"; // Your API Key

        // part of your event and cache code here

       public async Task<Forecast> GetForecastAsync(string City)
        {
            //part of cache code here

           // https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            OnWeatherForecastAvailable($"New weather forecast for {City} is available");
            return forecast;

            //part of event and cache code here
            //generate an event with different message if cached data
       }
        protected virtual void OnWeatherForecastAvailable(string s)
        {
            WeatherForecastAvailable?.Invoke(this, s);
        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //part of cache code here

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data

            OnWeatherForecastAvailable($"New weather forecast for ({latitude}, {longitude}) is available");

            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            Forecast forecast = new Forecast();

            forecast.City = wd.city.name;

            forecast.Items = new List<ForecastItem>();

            wd.list.ForEach(wdListItem => {forecast.Items.Add(GetForecastItem(wdListItem));});

            return forecast;
            
            // part of your read web api code here

            // part of your data transformation to Forecast here
            //generate an event with different message if cached data

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
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }

}
