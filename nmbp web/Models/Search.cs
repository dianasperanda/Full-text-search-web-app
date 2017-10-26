using System.Collections.Generic;

namespace nmbp_web.Models
{
    public class Search
    {
        public string SearchQuery { get; set; }

        public string Operator { get; set; }

        public string SQLQuery { get; set; }

        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}