using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nmbp_web.Functions
{
    public static class SearchHelpers
    {
        public static string SaveInDatabaseAndGenerateSearchQuery(string connectionString, string searchQuery, string searchOperator)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                var modified = searchQuery.Split('"').Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim());

                if (searchQuery.Count(c => c == '"') >= 2)
                {
                    modified = modified.Select(t => t.Replace(" ", " & ")).ToList();
                }
                else
                {
                    modified = searchQuery.Split(' ');
                }

                modified = modified.Select(t => $"({t})");

                searchQuery = searchOperator == "And" ? string.Join(" & ", modified) : string.Join(" | ", modified);

                var cmd = new NpgsqlCommand($"insert into search(query) values ('{searchQuery}')", conn);

                cmd.ExecuteNonQuery();

                conn.Close();
                
                return searchQuery;
            }
        }
    }
}