using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using ZstdSharp.Unsafe;

namespace SisigNiBessWebApiAdmin.Database.Service
{
    public class DBService
    {
        public static string ConnectionStrng = "server=UbLEYzOvFk1jq.h.filess.io;user=root;database=sisignibess_prod;password=c5e49998c41fa96b59e6ae2d90ac00e4;port=45731;AllowMultiQueries=True;AllowUserVariables=True;";

        public async Task<List<T>> GetDataListAsync<T>(string query) where T : new()
        {
            List<T> res = new List<T>();
            try
            {
                using (MySqlConnection objCon = new MySqlConnection(ConnectionStrng))
                {
                    objCon.Open();
                    var q = new MySqlCommand(query, objCon);

                    var r = await Task.Run(() =>
                    {
                        // Perform intensive calculations here
                        return q.ExecuteReaderAsync();
                    });

                    //MainThread.BeginInvokeOnMainThread(() =>
                    //{
                    while (r.Read())
                    {
                        T t = new T();

                        for (int inc = 0; inc < r.FieldCount; inc++)
                        {
                            Type type = t.GetType();
                            PropertyInfo prop = type.GetProperty(r.GetName(inc));
                            prop.SetValue(t, r.GetValue(inc), null);
                        }
                        res.Add(t);
                    }
                    //});

                    r.Close();
                    q.Dispose();
                    r.DisposeAsync();
                }


            }
            catch (Exception ex)
            {
                return null;
            }

            return res;
        }

        public async Task ExecuteNonQueryCommandAsync(string SqlCommand)
        {
            using (MySqlConnection objCon = new MySqlConnection(ConnectionStrng))
            {
                var QryCmd = new MySqlCommand();
                objCon.Open();

                QryCmd.Connection = objCon;
                QryCmd.CommandText = SqlCommand;
                QryCmd.CommandType = CommandType.Text;

                var r = await Task.Run(() =>
                {
                    return QryCmd.ExecuteNonQueryAsync();
                });

                QryCmd.Dispose();
            }
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql)
        {
            var results = new List<Dictionary<string, object>>();

            // 'using' ensures the connection is closed even if an error occurs
            using (var connection = new MySqlConnection(ConnectionStrng))
            {
                connection.Open();

                using (var command = new MySqlCommand(sql, connection))
                {
                    // ExecuteReaderAsync keeps the thread free during database I/O
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object columnValue = reader.GetValue(i);

                                // Handle DBNull to avoid issues during JSON serialization
                                row.Add(columnName, columnValue == DBNull.Value ? null : columnValue);
                            }

                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }
        public async Task<Dictionary<string, object>> GetDataObeject(string sql)
        {
            var results = new Dictionary<string, object>();

            // 'using' ensures the connection is closed even if an error occurs
            using (var connection = new MySqlConnection(ConnectionStrng))
            {
                connection.Open();

                using (var command = new MySqlCommand(sql, connection))
                {
                    // ExecuteReaderAsync keeps the thread free during database I/O
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                object columnValue = reader.GetValue(i);

                                // Handle DBNull to avoid issues during JSON serialization
                                row.Add(columnName, columnValue == DBNull.Value ? null : columnValue);
                            }

                            results = row;
                        }
                    }
                }
            }

            return results;
        }

    }
}
