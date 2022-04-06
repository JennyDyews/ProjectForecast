#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Threading.Tasks;

using Assignment_A2_01.Models;
using Assignment_A2_01.ModelsSampleData;
namespace Assignment_A2_01.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "cc40f1dc262e435b979752a8a9845a75";

        public async Task<NewsApiData> GetNewsAsync()
        {

#if UseNewsApiSample      
            //NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync("sports");

#else
            
            //Your code here to read live data
           
           
#endif
            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category=sports&apiKey={apiKey}";

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();

            //Your Code to convert WeatherApiData to Forecast using Linq.
            NewsApiData newsapidata = new NewsApiData();

            newsapidata.Articles = new List<Article>();

            nd.Articles.ForEach(wdListItem => { newsapidata.Articles.Add(GetNewsArticle(wdListItem)); });

            return newsapidata;
        }

        private Article GetNewsArticle(Article ndList)
        {

            Article article = new Article();
            article.Title = ndList.Title;
            article.Description = ndList.Description;
            return article;
        }

    }
}

