using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiteOut.Data.Imdb.Business.Postgre;
using NiteOut.Data.Imdb.Business.Omdb;

namespace NiteOut.Data.Imdb.Test
{
    [TestClass]
    public class PostgreTest
    {
        [TestMethod]
        public void TestConnection()
        {
            var instance = PostgreManager.Instance;
        }

        [TestMethod]
        public void TestRead()
        {
            var result = PostgreManager.Instance.QueryTable("Test");
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void TestInsertMove()
        {
            var movie = OmdbManager.Instance.GetMovieById("tt0076759");
            PostgreManager.Instance.InsertMovie(movie);
        }

        [TestMethod]
        public void TestBulkInsert()
        {
            var movies = OmdbManager.Instance.SearchMovie("star");
            PostgreManager.Instance.InsertMovies(movies);
        }
    }
}
