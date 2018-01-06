using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiteOut.Data.Imdb.Business.Omdb;

namespace NiteOut.Data.Imdb.Test
{
    [TestClass]
    public class OmdbTest
    {
        [TestMethod]
        public void TestQueryMovie()
        {
            OmdbManager.Instance.QueryMovieByTitle("star", "2017");
        }
    }
}
