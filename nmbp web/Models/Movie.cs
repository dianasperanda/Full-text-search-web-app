using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace nmbp_web.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Categories { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public double Rank { get; set; }
    }
}