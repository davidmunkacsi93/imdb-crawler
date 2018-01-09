using NiteOut.Data.Imdb.Business;
using NiteOut.Data.Imdb.Business.Crawler;
using NiteOut.Data.Imdb.Business.Entities;
using NiteOut.Data.Imdb.Business.Omdb;
using NiteOut.Data.Imdb.Business.Postgre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NiteOut.Data.Imdb.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogManager.Instance.Info("Application started.");
            var ids = ImdbCrawler.Instance.GetShowTimeIds();
            var movies = new List<Movie>();
            foreach (var id in ids)
            {
                movies.Add(OmdbManager.Instance.GetMovieById(id));
            }
            PostgreManager.Instance.InsertMovies(movies);
        }
    }
}
