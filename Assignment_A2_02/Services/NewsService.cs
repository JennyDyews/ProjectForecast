#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Assignment_A2_02.Models;
using System.Collections.Generic;
using Assignment_A2_02.ModelsSampleData;

namespace Assignment_A2_02.Services
{
    
    public class NewsService
    {
        public EventHandler<string> NewsApiAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "cc40f1dc262e435b979752a8a9845a75";

        public async Task<News> GetNewsAsync(NewsCategory category)
        {
            //#if UseNewsApiSample      
            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

            ////#else
            ////https://newsapi.org/docs/endpoints/top-headlines
            //var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";

            //// Your code here to get live data
            //HttpResponseMessage respons = await httpClient.GetAsync(uri);
            //respons.EnsureSuccessStatusCode();
            //NewsApiData nd = await respons.Content.ReadFromJsonAsync<NewsApiData>();


            News news = new News();
            news.Articles = new List<NewsItem>();
            nd.Articles.ForEach(a => news.Articles.Add(GetNewsArticle(a)));
            OnNewsApiAvailable($"News in category is available: {category}");

            return news;
        }
        //This method is responsible to raise the event
        protected virtual void OnNewsApiAvailable(string s)
        {
            NewsApiAvailable?.Invoke(this, s);
        }

        private NewsItem GetNewsArticle(Article ndArticle)
        {

            NewsItem newsItemArticle = new NewsItem();
            newsItemArticle.DateTime = ndArticle.PublishedAt;
            newsItemArticle.Title = ndArticle.Title;
            newsItemArticle.Description = ndArticle.Description;

            return newsItemArticle;
        }


    }
}
