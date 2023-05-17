using System.Data;
using System.Data.SqlClient;

namespace Server.Infrastructure
{
    public sealed class AdoNetController
    {
        private string _connectionString;
        
        public AdoNetController(string connectionString)
        {
            _connectionString = connectionString;
        }


        private SqlConnection InitConnection()
        {
            var connection = new SqlConnection
            {
                ConnectionString = _connectionString,
            };
            
            connection.Open();

            return connection;
        }
        

        public DataTable ExecuteQuery(SqlCommand command)
        {
            using (var connection = InitConnection())
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
            using (var connection = InitConnection())
            {
                command.Connection = connection;
           
                return command.ExecuteNonQuery();
            }
        }
        
        public object ExecuteScalar(SqlCommand command)
        {
            using (var connection = InitConnection())
            {
                command.Connection = connection;
           
                return command.ExecuteScalar();
            }
        }
    }
}