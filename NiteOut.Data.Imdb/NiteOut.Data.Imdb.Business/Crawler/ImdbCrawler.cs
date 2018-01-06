using HtmlAgilityPack;
using NiteOut.Data.Imdb.Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiteOut.Data.Imdb.Business.Crawler
{
    public class ImdbCrawler : Singleton<ImdbCrawler>
    {
        private const int zipCode = 10717;
        private const string country = "DE";
        private static string rootUrl = $"http://www.imdb.com/showtimes/location/{country}/{zipCode}?ref_=sh_lc";

        public List<string> GetShowTimeIds()
        {
            const string fromToken = "title/";
            const string toToken = "/?ref";
            var web = new HtmlWeb();
            var doc = web.Load(rootUrl);
            var domElements = doc.DocumentNode.Descendants("div").Where(div => div.HasClass("title"));
            var result = domElements.Select(de =>
            {
                var str = de.InnerHtml;
                int pFrom = str.IndexOf(fromToken) + fromToken.Length;
                int pTo = str.LastIndexOf(toToken);
                return str.Substring(pFrom, pTo - pFrom);
            }).ToList();
            return result;
        }
    }
}
