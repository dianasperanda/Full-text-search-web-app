using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace nmbp_web.Functions
{
    public static class AnalysisHelpers
    {
        public static string GenerateGranulationSelectQuery(DateTime start, DateTime end, string granulation)
        {
            var select = granulation == "Day" ? GenerateDaySelect(start, end) : GenerateHourSelect(start, end);

            return select;
        }
        
        private static string GenerateDaySelect(DateTime start, DateTime end)
        {
            return "SELECT * " +
                "FROM crosstab( 'SELECT query::text q, date(time)::text s, count(date(time))::text c " +
                $"FROM search WHERE date(time) >= ''{start.ToString("yyyy-MM-dd")}'' and date(time) <= ''{end.ToString("yyyy-MM-dd")}''" +
                "group by q, s order by q, s', 'SELECT i from analysis order by i') as piv (q text, ";
        }

        private static string GenerateHourSelect(DateTime start, DateTime end)
        {
            return "SELECT * " +
                "FROM crosstab('SELECT query::text q, to_char(time, ''HH24'') s, count(to_char(time, ''HH24''))::text c " +
                $"FROM search WHERE date(time) >= ''{start.ToString("yyyy-MM-dd")}'' and date(time) <= ''{end.ToString("yyyy-MM-dd")}''" +
                "group by q, s order by q, s', 'SELECT i from analysis order by i') as piv (q text, ";
        }

        internal static List<string> GetStringListOfHours(DateTime start, DateTime end)
        {
            var stringHours = new List<string>();

            for (var h = 0; h < 24 ; h++)
            {
                stringHours.Add(h.ToString("D2"));
            }

            return stringHours;
        }

        internal static List<string> GetStringListOfDates(DateTime start, DateTime end)
        {
            var stringDates = new List<string>();

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            for (var i = start; i <= end; i = i.AddDays(1))
            {
                stringDates.Add(i.ToString("yyyy-MM-dd"));
            }

            return stringDates;
        }

    }
}