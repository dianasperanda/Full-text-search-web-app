using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nmbp_web.Models
{
    public class Result
    {
        public string SearchPattern { get; set; } = string.Empty;

        public List<string> Times { get; set; } = new List<string>();
    }
}