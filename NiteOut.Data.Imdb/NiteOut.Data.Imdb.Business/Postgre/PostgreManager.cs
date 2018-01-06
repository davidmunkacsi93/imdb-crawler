﻿using NiteOut.Data.Imdb.Business.Entities;
using NiteOut.Data.Imdb.Business.Infrastructure;
using Npgsql;
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
        private const string database = "IMDB";
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
        public void InsertMovie(Movie movie)
        {

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
            while(reader.Read())
            {
                // We can assume that the result set contains only strings. 
                tableNames.Add(reader[0] as string);
            }
            reader.Close();
            return tableNames.Contains(tableName);
        }
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
