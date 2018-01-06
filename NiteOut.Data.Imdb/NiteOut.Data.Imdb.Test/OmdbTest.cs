using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiteOut.Data.Imdb.Business.Omdb;
using NiteOut.Data.Imdb.Business.Postgre;

namespace NiteOut.Data.Imdb.Test
{
    [TestClass]
    public class OmdbTest
    {
        [TestMethod]
        public void TestGetMovieByTitle() => Assert.IsNotNull(OmdbManager.Instance.GetMovieByTitle("star", "2017"));

        [TestMethod]
        public void TestGetMovieById() => Assert.IsNotNull(OmdbManager.Instance.GetMovieById("tt0076759"));

        [TestMethod]
        public void TestSearchMovie() => Assert.IsNotNull(OmdbManager.Instance.SearchMovie("star"));

        [TestMethod]
        public void TestInsertMove()
        {
            var movie = OmdbManager.Instance.GetMovieById("tt0076759");
            PostgreManager.Instance.InsertMovie(movie);
        }
    }
}
