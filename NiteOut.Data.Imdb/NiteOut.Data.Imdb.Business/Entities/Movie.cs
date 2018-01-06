using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiteOut.Data.Imdb.Business.Entities
{
    public class Movie
    {
        public string Title { get; set; }

        public string Country { get; set; }

        public DateTime PrimaryReleaseDate { get; set; }

        public string StoryLine { get; set; }
    }
}
