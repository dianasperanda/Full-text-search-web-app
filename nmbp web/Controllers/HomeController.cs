using nmbp_web.Functions;
using nmbp_web.Models;
using Npgsql;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace nmbp_web.Controllers
{
    public class HomeController : Controller
    {
        private NpgsqlConnection conn = new NpgsqlConnection(connectionString: ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString);

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View(new Movie());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Movie movie)
        {
            conn.Open();

            var cmd = new NpgsqlCommand(
                "insert into movie (title, categories, summary, description, vphrase, vtitle)" +
                "values(@title, @categories, @summary, @description, " +
                "setweight(to_tsvector(coalesce(@title, '')), 'A') || setweight(to_tsvector(coalesce(@categories, '')), 'B') || setweight(to_tsvector(coalesce(@summary, '')), 'C') " +
                "|| setweight(to_tsvector(coalesce(@description,'')), 'D'), " +
                "to_tsvector('english', coalesce(@title)))",
                conn);

            cmd.Parameters.AddWithValue("@title", movie.Title);
            cmd.Parameters.AddWithValue("@categories", movie.Categories);
            cmd.Parameters.AddWithValue("@summary", movie.Summary);
            cmd.Parameters.AddWithValue("@description", movie.Description);

            cmd.ExecuteNonQuery();

            conn.Close();

            return View("Index");
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View(new Search { Operator = "And" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(Search search)
        {
            search.SearchQuery = search.SearchQuery.Trim();

            var modifiedSearchQuery = SearchHelpers.SaveInDatabaseAndGenerateSearchQuery(conn.ConnectionString, search.SearchQuery, search.Operator);

            conn.Open();

            var cmd = new NpgsqlCommand($"select \n\tmovieid,\n" +
                    $"\tts_headline(title, to_tsquery('english', '{modifiedSearchQuery}')) headline,\n" +
                    $"\tts_headline(categories, to_tsquery('english', '{modifiedSearchQuery}')) headline,\n" +
                    $"\tts_headline(summary, to_tsquery('english', '{modifiedSearchQuery}')) headline,\n" +
                    $"\tts_headline(description, to_tsquery('english', '{modifiedSearchQuery}')) headline,\n" +
                    $"\tts_rank(array[0.1, 0.2, 0.6, 1.0], vphrase, to_tsquery('english', '{modifiedSearchQuery}')) rank\n" +
                    $"from movie\n" +
                    $"where vphrase @@ to_tsquery('english', '{modifiedSearchQuery}')\n" +
                    $"order by rank desc", conn);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                search.Movies.Add(new Movie
                {
                    Id = int.Parse(reader[0].ToString()),
                    Title = reader[1].ToString(),
                    Categories = reader[2].ToString(),
                    Summary = reader[3].ToString(),
                    Description = reader[4].ToString(),
                    Rank = double.Parse(reader[5].ToString())
                });
            }

            search.SQLQuery = cmd.CommandText;

            conn.Close();

            return View(search);
        }

        [HttpGet]
        public ActionResult Analysis()
        {
            return View(new Analysis());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Analysis(Analysis analysis)
        {
            var select = AnalysisHelpers.GenerateGranulationSelectQuery(analysis.Start, analysis.End, analysis.Granulation);

            analysis.Headers = analysis.Granulation == "Day" ? AnalysisHelpers.GetStringListOfDates(analysis.Start, analysis.End) : AnalysisHelpers.GetStringListOfHours(analysis.Start, analysis.End);

            var insert = "CREATE TABLE analysis (i text); INSERT INTO analysis VALUES ";

            foreach (var item in analysis.Headers)
            {
                insert += $"('{item}'), ";
                select += $"c{analysis.Headers.IndexOf(item)} text, ";
            }

            insert = insert.TrimEnd(' ', ',');
            select = select.TrimEnd(' ', ',') + ")";
            
            conn.Open();

            using (var insertCmd = new NpgsqlCommand(insert, conn))
            {
                insertCmd.ExecuteNonQuery();
            }

            
            var selectCmd = new NpgsqlCommand(select, conn);
            using (NpgsqlDataReader reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var result = new Result() { SearchPattern = reader[0].ToString() };
                    for (int i = 1; i <= analysis.Headers.Count; i++)
                    {
                        result.Times.Add(reader[i].ToString());
                    }
                    analysis.Result.Add(result);
                }
            }
            selectCmd.Dispose();

            using (var del = new NpgsqlCommand("drop table analysis", conn))
            {
                del.ExecuteNonQuery();
            }

            return View(analysis);
        }

    }
}