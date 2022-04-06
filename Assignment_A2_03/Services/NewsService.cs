#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Assignment_A2_03.Models;
using Assignment_A2_03.ModelsSampleData;
using System.Collections.Generic;

namespace Assignment_A2_03.Services
{
    public class NewsService
    {
        ConcurrentDictionary<(string, NewsCategory), News> _Cached1 = new ConcurrentDictionary<(string, NewsCategory), News>();
        public EventHandler<string> NewsAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "cc40f1dc262e435b979752a8a9845a75";
        public async Task<News> GetNewsAsync(NewsCategory category)
        {
#if UseNewsApiSample      
            //NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

#else
            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";

           // Your code here to get live data

#endif

            //var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";

            //HttpResponseMessage response = await httpClient.GetAsync(uri);
            //response.EnsureSuccessStatusCode();

            //NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();
            News news = null;

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            NewsCategory cat = category;
            var key = (date, cat);


            if (!_Cached1.TryGetValue(key, out news))
            {

                var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";
                news = await ReadNewsApiAsync(uri);
                _Cached1[key] = news;
                OnNewsAvailable($"News in category availble:{category}");
            }

            else
                OnNewsAvailable($"Cahced News in category availble:{category}");


            return news;

        }
        protected virtual void OnNewsAvailable(string c)
        {
            NewsAvailable?.Invoke(this, c);
        }

        private async Task<News> ReadNewsApiAsync(string uri)
        {

            uri = $"https://newsapi.org/v2/top-headlines?country=se&category={NewsCategory.general}&apiKey={apiKey}"; //soupossed to be cat

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();


            News news = new News();

            news.Articles = new List<NewsItem>();

            nd.Articles.ForEach(a => { news.Articles.Add(GetNewsItem(a)); });

            return news;

        }

         private NewsItem GetNewsItem(Article wdListItem)
         {
            NewsItem newsitem = new NewsItem();

            newsitem.DateTime = wdListItem.PublishedAt;

            newsitem.Title = wdListItem.Title;

            return newsitem;

         }

    }
}