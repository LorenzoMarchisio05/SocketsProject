using System;
using System.Data;
using System.Data.SqlClient;

namespace Wheater.Commons.DB
{
    public sealed class AdoNetController
    {
        private string _connectionString;

        public AdoNetController(string connectionString)
        {
            _connectionString = connectionString;
        }


        private bool TryInitConnection(out SqlConnection connection)
        {
            try
            {
                connection = new SqlConnection
                {
                    ConnectionString = _connectionString,
                };

                connection.Open();

                return true;
            }
            catch (Exception)
            {
                connection = null;
                return false;
            }
        }


        public DataTable ExecuteQuery(SqlCommand command)
        {
            var connected = TryInitConnection(out var connection);

            if (!connected)
            {
                return default;
            }

            using (connection)
            {
                command.Connection = connection;

                var dataTable = new DataTable();

                var adapter = new SqlDataAdapter(command);

                adapter.Fill(dataTable);

                return dataTable;
            }
        }

        public int ExecuteNonQuery(SqlCommand command)
        {
            var connected = TryInitConnection(out var connection);

            if (!connected)
            {
                return -1;
            }

            using (connection)
            {
                command.Connection = connection;

                return command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(SqlCommand command)
        {
            var connected = TryInitConnection(out var connection);

            if (!connected)
            {
                return default;
            }

            using (connection)
            {
                command.Connection = connection;

                return command.ExecuteScalar();
            }
        }
    }
}