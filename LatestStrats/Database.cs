using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using System.Linq;
using System.IO;

namespace LatestStrats
{
    class Database
    {
        private SQLiteConnection _connection;
        private Log _log;

        public Database(string database)
        {
            _log = new Log("Database", "Database.txt", 1);

            if (!File.Exists(database))
            {
                SQLiteConnection.CreateFile(database);
                _log.Write(Log.LogLevel.Success, $"Database created at {database}");
            }

            _connection = new SQLiteConnection($"Data Source={database};Version=3;");
            _connection.Open();

            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _log.Write(Log.LogLevel.Error, $"Database could not open");
                return;
            }

            runsql("CREATE TABLE IF NOT EXISTS timestamps (unix Long)");
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
