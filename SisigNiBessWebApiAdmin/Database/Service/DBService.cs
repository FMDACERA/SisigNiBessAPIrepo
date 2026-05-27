using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using ZstdSharp.Unsafe;

namespace SisigNiBessWebApiAdmin.Database.Service
{
    public class DBService
    {
        public static string ConnectionStrng = "server=UbLEYzOvFk1jq.h.filess.io;user=root;database=sisignibess_prod;password=c5e49998c41fa96b59e6ae2d90ac00e4;port=45731";

        //public async Task<List<T>> GetDataListAsync<T>(string query) where T : new()
        //{
        //    List<T> res = new List<T>();
        //    try
        //    {
        //        using (MySqlConnection objCon = new MySqlConnection(ConnectionStrng))
        //        {
        //            objCon.Open();
        //            var q = new MySqlCommand(query, objCon);

        //            var r = await Task.Run(() =>
        //            {
        //                return q.ExecuteReaderAsync();
        //            });

        //            while (r.Read())
        //            {
        //                T t = new T();

        //                for (int inc = 0; inc < r.FieldCount; inc++)
        //                {
        //                    Type type = t.GetType();
        //                    PropertyInfo prop = type.GetProperty(r.GetName(inc));
        //                    prop.SetValue(t, r.GetValue(inc), null);
        //                }
        //                res.Add(t);
        //            }
 
        //            r.Close();
        //            q.Dispose();
        //            r.DisposeAsync();
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //    return res;
        //}

        public async Task<List<T>> GetDataListAsync<T>(string query) where T : new()
        {
            List<T> res = new List<T>();
            try
            {
                // 1. "using await" ensures clean asynchronous disposal of the connection
                await using (MySqlConnection objCon = new MySqlConnection(ConnectionStrng))
                {
                    await objCon.OpenAsync(); // Use Async open

                    await using (MySqlCommand q = new MySqlCommand(query, objCon))
                    {
                        // 2. Properly await the reader natively without Task.Run
                        await using (var r = await q.ExecuteReaderAsync())
                        {
                            // 3. Natively await row reads so the connection doesn't block the thread
                            while (await r.ReadAsync())
                            {
                                T t = new T();
                                Type type = t.GetType();

                                for (int inc = 0; inc < r.FieldCount; inc++)
                                {
                                    // Checking for DBNull prevents mapping crashes
                                    if (!r.IsDBNull(inc))
                                    {
                                        PropertyInfo prop = type.GetProperty(r.GetName(inc));
                                        if (prop != null)
                                        {
                                            prop.SetValue(t, r.GetValue(inc), null);
                                        }
                                    }
                                }
                                res.Add(t);
                            }
                        } // Reader automatically closes and disposes asynchronously here
                    } // Command automatically disposes here
                } // Connection automatically returns to pool cleanly here
            }
            catch (Exception ex)
            {
                // Consider logging 'ex' here so you don't fly blind if a query breaks
                return null;
            }

            return res;
        }


        public async Task ExecuteNonQueryCommandAsync(string SqlCommand)
        {
            try
            {
                // 1. "await using" guarantees clean async cleanup and immediate return to the connection pool
                await using (MySqlConnection objCon = new MySqlConnection(ConnectionStrng))
                {
                    await objCon.OpenAsync(); // Natively await connection opening

                    await using (MySqlCommand QryCmd = new MySqlCommand(SqlCommand, objCon))
                    {
                        QryCmd.CommandType = CommandType.Text;

                        // 2. Natively await the non-query execution without Task.Run blocking a thread
                        await QryCmd.ExecuteNonQueryAsync();
                    } // QryCmd is automatically disposed here asynchronously
                } // objCon is automatically closed and returned to the pool here
            }
            catch (Exception ex)
            {
                // Highly recommended: Log your exception here (e.g., Log.Error(ex)) 
                // so you don't silently fail if an INSERT/UPDATE breaks.
                throw;
            }
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql)
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                // 1. Asynchronously opens and manages connection disposal
                await using (var connection = new MySqlConnection(ConnectionStrng))
                {
                    await connection.OpenAsync(); // Non-blocking connection open

                    await using (var command = new MySqlCommand(sql, connection))
                    {
                        // 2. Asynchronously manages the reader lifecycle
                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    object columnValue = reader.GetValue(i);

                                    // DBNull check looks great—perfect for JSON serialization stability!
                                    row.Add(columnName, columnValue == DBNull.Value ? null : columnValue);
                                }

                                results.Add(row);
                            }
                        } // Reader cleanly unbinds asynchronously here
                    } // Command cleanly disposes here
                } // Connection returns to pool asynchronously here
            }
            catch (Exception ex)
            {
                // Recommended: Log exception details here if an ad-hoc query fails
                throw;
            }

            return results;
        }
        public async Task<Dictionary<string, object>> GetDataObjectAsync(string sql)
        {
            // Initialize as null so the calling application knows if the record actually exists
            Dictionary<string, object> result = null;

            try
            {
                // 1. Fully async management of the database connection lifecycle
                await using (var connection = new MySqlConnection(ConnectionStrng))
                {
                    await connection.OpenAsync(); // Non-blocking open

                    await using (var command = new MySqlCommand(sql, connection))
                    {
                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            // 2. Change 'while' to 'if' since we only expect a single row
                            if (await reader.ReadAsync())
                            {
                                result = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    object columnValue = reader.GetValue(i);

                                    // Seamless handling for JSON serialization
                                    result.Add(columnName, columnValue == DBNull.Value ? null : columnValue);
                                }
                            }
                        } // Reader disposes asynchronously here
                    } // Command disposes here
                } // Connection returns cleanly to the pool here
            }
            catch (Exception ex)
            {
                // Recommended: Log your exception here
                throw;
            }

            return result;
        }
        public async Task<bool> InsertDataFromListAsync<T>(List<string> propExemptions, T tableName, string spName) where T : new()
        {
            try
            {
                await using (var connection = new MySqlConnection(ConnectionStrng))
                {
                    await connection.OpenAsync();

                    Type type = tableName.GetType();
                    PropertyInfo[] properties = type.GetProperties();

                    await using (var command = new MySqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        foreach (PropertyInfo property in properties)
                        {
                            if (propExemptions == null || !propExemptions.Contains(property.Name))
                            {
                                object value = property.GetValue(tableName, null);
                                command.Parameters.AddWithValue("IN_" + property.Name, value ?? DBNull.Value);
                            }
                        }

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                // Do not use DisplayErrorMsg here. Throw the exception up so your API Controller can handle it.
                throw;
            }
        }
    }
}
