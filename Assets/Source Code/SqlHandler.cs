using System.Data;
using System.Text;
using Mono.Data.SqliteClient;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source_Code {
    public static class SqlHandler {
        public static string DBLocation;
        
        private static IDbConnection _connection;
        private static IDbCommand _command;
        private static IDataReader _reader;
        private static string _sqlString;
        
        public static void InitSqlite() {
            _connection = new SqliteConnection(DBLocation);
            _command = _connection.CreateCommand();
            _connection.Open();

            NonQuery("PRAGMA journal_mode = WAL;");

            NonQuery("PRAGMA synchronous = OFF;");
            
            _connection.Close();
        }

        public static void InitDB() {
            NonQuery(
                $"CREATE TABLE IF NOT EXISTS Images(" +
                $"ID INTEGER UNIQUE NOT NULL PRIMARY KEY," +
                $"FilePath TEXT" +
                $");");
        }

        public static void Close() {
            if (_reader != null && !_reader.IsClosed)
                _reader.Close();
            _reader = null;

            if (_command != null)
                _command.Dispose();
            _command = null;

            if (_connection != null && _connection.State != ConnectionState.Closed)
                _connection.Close();
            _connection = null;
        }

        public static void NonQuery(string command) {
            _connection.Open();
            _command.CommandText = command;
            _command.ExecuteNonQuery();
            _connection.Close();
        }

        public static string Query(string command) {
            _connection.Open();
            _command.CommandText = command;
            _reader = _command.ExecuteReader();

            StringBuilder sb = new StringBuilder();

            while (_reader.Read()) {
                sb.Append(_reader.GetString(0)).Append(" ");
                //sb.Append(_reader.GetString(1)).Append(" ");
                sb.AppendLine();
            }

            _reader.Close();
            _connection.Close();

            return sb.ToString();
        }
    }
}
