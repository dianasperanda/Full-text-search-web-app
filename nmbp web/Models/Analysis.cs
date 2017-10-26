using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nmbp_web.Models
{
    public class Analysis
    {
        public DateTime Start { get; set; } = DateTime.Now;

        public DateTime End { get; set; } = DateTime.Now;

        public string Granulation { get; set; } = "Day";

        public List<Result> Result { get; set; } = new List<Models.Result>();

        public List<string> Headers { get; set; } = new List<string>();
    }
}