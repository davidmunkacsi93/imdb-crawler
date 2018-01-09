using NiteOut.Data.Imdb.Business.Entities;
using NiteOut.Data.Imdb.Business.Infrastructure;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiteOut.Data.Imdb.Business.Postgre
{
    public class PostgreManager : Singleton<PostgreManager>, IDisposable
    {
        #region Fields
        private const string userId = "postgres";
        private const string password = "Pa$$word123";
        private const string database = "NiteOut";
        private const string server = "127.0.0.1";
        private string connectionString = $"Server={server};User Id = {userId}; " +
             $"Password={password};Database={database};";
        private NpgsqlConnection connection;
        #endregion

        #region Properties
        public NpgsqlConnection Connection { get => connection; set => connection = value; }
        #endregion

        #region Constructor
        public PostgreManager()
        {
            try
            {
                Connection = new NpgsqlConnection(connectionString);
                Connection.Open();
                LogManager.Instance.Info($"[Postgre]Connection with database server {server} was opened.");
            }
            catch (Exception exc)
            {
                LogManager.Instance.Error($"[Postgre]Connection with database server {server} could not be established.", exc);
                throw exc;
            }
        }
        #endregion

        #region Read
        public List<object> QueryTable(string tableName)
        {
            if (!SanitizeTableName(tableName))
            {
                var exc = new ArgumentException("Table does not exist");
                LogManager.Instance.Error($"Table does not exist: {tableName}", exc);
                throw exc;
            }
            var readCommand = new NpgsqlCommand($"SELECT * FROM \"{tableName}\";", Connection);
            var reader = readCommand.ExecuteReader();
            var result = new List<object>();
            while (reader.Read())
            {
                // We can assume that the result set contains only strings. 
                result.Add(reader[0]);
            }
            readCommand.Dispose();
            reader.Close();
            return result;
        }
        #endregion

        #region Create
        public void InsertMovies(List<Movie> movies)
        {
            var dbMovies = QueryTable("MOVIE");
            foreach (var movie in movies)
            {
                InsertMovie(movie);
            }
        }

        public void InsertMovie(Movie movie)
        {
            // TODO: Better way to solve this? Serial IDs?
            var insertCommand = new NpgsqlCommand("INSERT INTO \"MOVIE\"" +
                "(\"MOVIE_ID\", \"TITLE\", \"COUNTRY\", \"PRIMARY_RELEASE_DATE\", \"STORY_LINE\", \"GENRES\", \"WEBSITE\", \"REVIEWS\", \"BUDGET\", \"ACTORS\", \"DIRECTORS\", \"WRITERS\", \"POSTER\", \"DATA_SOURCE_ID\", \"DATA_SOURCE\")" +
                $" VALUES({GetNextIdScript("MOVIE", "MOVIE_ID")}, @title, @country, @releaseDate, @storyLine, @genres, @website, @reviews, @budget, @actors, @directors, @writers, @poster, @dataSourceId, @dataSource) RETURNING  \"MOVIE_ID\"", Connection);

            try
            {
                DateTime released;
                insertCommand.Parameters.AddWithValue("title", movie.Title);
                insertCommand.Parameters.AddWithValue("country", movie.Country);
                insertCommand.Parameters.AddWithValue("releaseDate", DateTime.TryParseExact(movie.Released, "dd MMM yyyy",
                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out released) ? released : DateTime.MinValue);
                insertCommand.Parameters.AddWithValue("storyLine", movie.Plot);
                insertCommand.Parameters.Add("genres", NpgsqlDbType.Array | NpgsqlDbType.Text).Value = movie.Genre.Split(',');
                insertCommand.Parameters.AddWithValue("website", movie.Website);
                insertCommand.Parameters.Add("reviews", NpgsqlDbType.Array | NpgsqlDbType.Text).Value = new string[] { };
                insertCommand.Parameters.Add("budget", NpgsqlDbType.Array | NpgsqlDbType.Text).Value = new string[] { };
                insertCommand.Parameters.Add("actors", NpgsqlDbType.Array | NpgsqlDbType.Text).Value = movie.Actors.Split(',');
                insertCommand.Parameters.Add("directors", NpgsqlDbType.Array | NpgsqlDbType.Text).Value = movie.Director.Split(',');
                insertCommand.Parameters.Add("writers", NpgsqlDbType.Array | NpgsqlDbType.Text).Value = movie.Writer.Split(',');
                insertCommand.Parameters.AddWithValue("poster", movie.Poster);
                insertCommand.Parameters.AddWithValue("dataSourceId", movie.imdbID);
                insertCommand.Parameters.AddWithValue("dataSource", "IMDB");


                var id = (int)insertCommand.ExecuteScalar();
                insertCommand.Dispose();
                foreach (var rating in movie.Ratings)
                {
                    InsertRating(id, rating);
                }
            }
            catch (Exception exc)
            {
                LogManager.Instance.Error($"Movie {movie.imdbID} could not be inserted.", exc);
            }

        }

        public void InsertRating(int movieId, Rating rating)
        {
            // TODO: Better way to solve this? Serial IDs?
            var insertCommand = new NpgsqlCommand("INSERT INTO \"RANKING\"(\"RANKING_ID\", \"MOVIE_ID\", \"SOURCE\", \"RATING\")" +
                $" VALUES({GetNextIdScript("RANKING", "RANKING_ID")}, @movieId, @source, @rating);", Connection);


            try
            {
                insertCommand.Parameters.AddWithValue("movieId", movieId);
                insertCommand.Parameters.AddWithValue("source", rating.Source);
                insertCommand.Parameters.AddWithValue("rating", rating.Value);
                int changed = insertCommand.ExecuteNonQuery();
                LogManager.Instance.Info("");
            }
            catch (Exception exc)
            {
                LogManager.Instance.Error("Ranking could not be inserted.", exc);
            }
            insertCommand.Dispose();
        }
        #endregion

        #region Sanitize
        public bool SanitizeTableName(string tableName)
        {
            var query = "SELECT table_name FROM information_schema.tables"
                        + " WHERE table_type = 'BASE TABLE' AND table_schema = 'public'";
            var command = new NpgsqlCommand(query, Connection);
            var reader = command.ExecuteReader();

            var tableNames = new List<string>();
            while (reader.Read())
            {
                // We can assume that the result set contains only strings. 
                tableNames.Add(reader[0] as string);
            }
            reader.Close();
            return tableNames.Contains(tableName);
        }
        #endregion

        #region Helpers
        public string GetNextIdScript(string tableName, string pkName)
         => $"(SELECT COALESCE(MAX(\"{pkName}\"), 0) + 1 FROM \"{tableName}\")";
        #endregion

        #region Implementation of IDisposable
        public void Dispose()
        {
            try
            {
                Connection.Close();
                LogManager.Instance.Info($"[Postgre]Connection with database server {server} was closed.");
            }
            catch (Exception exc)
            {
                LogManager.Instance.Error($"[Postgre]Connection with database server {server} could not be closed.", exc);
                throw exc;
            }
        }
        #endregion
    }
}
