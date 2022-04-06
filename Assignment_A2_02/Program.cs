using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Assignment_A2_02.Models;
using Assignment_A2_02.Services;

namespace Assignment_A2_02
{
    class Program
    {
        static void Main(string[] args)
        {

            NewsService service = new NewsService();
            service.NewsApiAvailable += ReportNewsDataAvailable;
            Task<News> t1 = null;
            Exception exception = null;

            try
            {
                //Create task and wait for completion
                for (NewsCategory i = NewsCategory.business; i < NewsCategory.technology + 1; i++)
                {
                    t1 = service.GetNewsAsync(i);
                }


                Task.WaitAll(t1);

            }
            catch (Exception ex)
            {
                //if exception write the message later
                exception = ex;
            }
            Console.WriteLine("----------------------");

            for (NewsCategory i = NewsCategory.business; i < NewsCategory.technology + 1; i++)
            {
                if (t1?.Status == TaskStatus.RanToCompletion)
                {
                    News news = t1.Result;
                    Console.WriteLine($"News in Category {i}");

                    news.Articles.ForEach(a => Console.WriteLine($"  - {a.DateTime.ToString("yyyy-MM-dd HH:mm")}: {a.Title} "));

                }
                else
                {
                    Console.WriteLine("Geolocation News service error.");
                }
            }

        }

        static void ReportNewsDataAvailable(object sender, string message)
        {
            Console.WriteLine($"Event message from news service: {message}");
        }
    }
}
        
    

