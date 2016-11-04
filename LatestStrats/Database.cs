using System;
using System.Data.SQLite;
using System.IO;

namespace StrategyIncubator
{
    class Database : IDisposable
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

        public Database(string databaseLocation)
        {
            _log = new Log("Database", "Database.txt", 1);

            if (!File.Exists(databaseLocation))
            {
                SQLiteConnection.CreateFile(databaseLocation);
                _log.Write(Log.LogLevel.Success, $"New database created at {databaseLocation}");
            }

            _connection = new SQLiteConnection($"Data Source={databaseLocation};Version=3;");

            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                _log.Write(Log.LogLevel.Error, $"Error opening sqlite database {databaseLocation}: {ex.Message}");
            }
            finally
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _log.Write(Log.LogLevel.Error, $"Database could not open successfully");
                else
                    runSql("CREATE TABLE IF NOT EXISTS timestamps (unix Long)");
            }
        }

        private int runSql(string sql)
        {
            /* CA2100
            Should only be used to run inhouse querys which does not
            come from user input since the values does not get sanitized*/
            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (IOException ex)
                {
                    _log.Write(Log.LogLevel.Error, $"Error executing SQL: {sql}\n{ex.Message}");
                    return -1;
                }
            }
        }

        public int InsertUnix(long unix)
        {
            using (var cmd = new SQLiteCommand("INSERT INTO timestamps (unix) values (?)", _connection))
            {
                cmd.Parameters.AddWithValue("unix", unix);

                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _log.Write(Log.LogLevel.Error, $"Error inserting unix value to database. {ex.Message}");
                    return -1;
                }
            }
        }

        public bool DoesUnixExist(long unix)
        {
            using (var cmd = new SQLiteCommand("SELECT count(*) FROM timestamps WHERE unix = @UNIX_VALUE", _connection))
            {
                cmd.Parameters.Add("@UNIX_VALUE", System.Data.DbType.Int64).Value = unix;

                try
                {
                    return (int)(cmd.ExecuteScalar()) > 0;
                }
                catch (Exception ex)
                {
                    _log.Write(Log.LogLevel.Error, $"Error retrieving unix values. {ex.Message}");

                    /*We'll return false since if a database error
                    should happen then we want to avoid having the
                    bot spam the same message over and over again*/
                    return false;
                }
            }
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        public void Dispose()
        {
            _connection.Dispose();

            Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
