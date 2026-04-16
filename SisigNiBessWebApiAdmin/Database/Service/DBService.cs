using MySql.Data.MySqlClient;
using System.Reflection;

namespace SisigNiBessWebApiAdmin.Database.Service
{
    public class DBService
    {
        public static string ConnectionStrng = "server=UbLEYzOvFk1jq.h.filess.io;user=root;database=sisignibess_prod;password=c5e49998c41fa96b59e6ae2d90ac00e4;port=45731";
        public List<T> GetDataListAsync<T>(string query) where T : new()
        {
            List<T> res = new List<T>();
            try
            {
                using (MySqlConnection objCon = new MySqlConnection(ConnectionStrng))
                {
                    objCon.Open();
                    var q = new MySqlCommand(query, objCon);

                    //var r = await Task.Run(() =>
                    //{
                    //    // Perform intensive calculations here
                    //    return q.ExecuteReaderAsync();
                    //});
                    var r = q.ExecuteReader();
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
    }
}
