using System;
using System.Data.SQLite;
using System.IO;

namespace StrategyIncubator
{
    class Database
    {
        private SQLiteConnection _connection;
        private Log _log;

        /*
            DATABASE SETUP
            --------------
            TABLES:
                timestamps
                    COLUMNS:
                        unix (Long)

            LOGIC
            --------------
            There is no ID attached for each post so we take the
            post date and convert it to a unix timestamp and use
            that as an ID so we can prevent the same post being
            alerted more than once. The odds of multiple posts
            being made at the exact same second is very slim.
        */

        public Database(string database)
        {
            _log = new Log("Database", "Database.txt", 1);

            if (!File.Exists(database))
            {
                SQLiteConnection.CreateFile(database);
                _log.Write(Log.LogLevel.Success, $"New database created at {database}");
            }

            _connection = new SQLiteConnection($"Data Source={database};Version=3;");

            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                _log.Write(Log.LogLevel.Error, $"Error opening sqlite database {database}: {ex.Message}");
            }
            finally
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _log.Write(Log.LogLevel.Error, $"Database could not open successfully");
                else
                    runsql("CREATE TABLE IF NOT EXISTS timestamps (unix Long)");
            }
        }

        private int runsql(string sql)
        {
            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (IOException ex)
                {
                    _log.Write(Log.LogLevel.Error, $"Error executing SQL: {sql}");
                    return 0;
                }
            }
        }

        public int InsertUnix(long unix)
        {
            using (var cmd = new SQLiteCommand("INSERT INTO timestamps (unix) values (?)", _connection))
            {
                cmd.Parameters.AddWithValue("unix", unix);
                return cmd.ExecuteNonQuery();
            }
        }

        public bool DoesUnixExist(long unix)
        {
            using (var cmd = new SQLiteCommand($"SELECT count(*) FROM timestamps WHERE unix = '{unix}'", _connection))
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
