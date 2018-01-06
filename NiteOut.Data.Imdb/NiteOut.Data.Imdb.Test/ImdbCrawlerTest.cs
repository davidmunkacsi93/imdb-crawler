using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiteOut.Data.Imdb.Business.Crawler;

namespace NiteOut.Data.Imdb.Test
{
    [TestClass]
    public class ImdbCrawlerTest
    {
        [TestMethod]
        public void TestGetShowTimeIds() => ImdbCrawler.Instance.GetShowTimeIds();
    }
}
