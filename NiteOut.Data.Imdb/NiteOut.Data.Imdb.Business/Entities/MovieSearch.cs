using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiteOut.Data.Imdb.Business.Entities
{
    public class MovieSearch
    {
        public List<MovieStub> Search { get; set; }

        public string totalResults { get; set; }

        public string Response { get; set; }
    }
}
