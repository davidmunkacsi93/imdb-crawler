using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiteOut.Data.Imdb.Business.Postgre;

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
    }
}
