using NiteOut.Data.Imdb.Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NiteOut.Data.Imdb.Business.Omdb
{
    public class OmdbManager : Singleton<OmdbManager>
    {
        private const string apiKey = "dbeb81bd";
        private static string rootUrl = $"http://www.omdbapi.com/?apikey={apiKey}";

        public string QueryMovieBySearch(string searchKey)
        {
            string url = rootUrl;
            if (string.IsNullOrEmpty(searchKey))
            {
                var exception = new ArgumentException("Search key can not be empty!");
                LogManager.Instance.Error("Search key can not be empty!", exception);
                throw exception;
            }

            url += $"&s={searchKey}";

            return GetRequest(url);
        }

        public string QueryMovieByTitle(string title, string year)
        {
            string url = rootUrl;
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(year))
            {
                var exception = new ArgumentException("Both title and year can not be empty!");
                LogManager.Instance.Error("Both title and year can not be empty!", exception);
                throw exception;
            }
            if (!string.IsNullOrEmpty(title))
            {
                url += $"&t={title}";
            }

            if (!string.IsNullOrEmpty(year))
            {
                url += $"&y={year}";
            }

            return GetRequest(url);
        }

        private string GetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    LogManager.Instance.Info($"[OMDb] Calling URL {rootUrl} was succesful.");
                }
                LogManager.Instance.Info($"[OMDb] Calling URL {rootUrl} was not succesful.");
                return response.Content.ReadAsStringAsync().Result;

            }
        }
    }
}
