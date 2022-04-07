#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Assignment_A2_04.Models;
using Assignment_A2_04.ModelsSampleData;
using System.Collections.Generic;

namespace Assignment_A2_04.Services
{
    public class NewsService
    {
        public EventHandler<string> NewsAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = /*"cc40f1dc262e435b979752a8a9845a75"*/"fcea3dbaa815400ab5df3d397e7784a3";

        public async Task<News> GetNewsAsync(NewsCategory category)
        {
            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

            NewsCacheKey key = new (category);
            News news = null;

            if (!key.CacheExist)
            {

                var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";
                news = await ReadNewsApiAsync(uri);
                News.Serialize(news, key.FileName);
                OnNewsAvailable($"News in category availble:{category}");
            }
            else
            {
                News.Deserialize(key.FileName);
                OnNewsAvailable($"XML Cached in category availble:{category}");
            }

            return news;


        }
        protected virtual void OnNewsAvailable(string c)
        {
            NewsAvailable?.Invoke(this, c);
        }

        private async Task<News> ReadNewsApiAsync(string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();

            News news = new News();

            news.Articles = new List<NewsItem>();

            nd.Articles.ForEach(a => { news.Articles.Add(GetNewsItem(a)); });

            return news;

        }

        private NewsItem GetNewsItem(Article newsListItem)
        {
            NewsItem newsitem = new NewsItem();

            newsitem.DateTime = newsListItem.PublishedAt;

            newsitem.Title = newsListItem.Title;

            return newsitem;

        }

    }
}
