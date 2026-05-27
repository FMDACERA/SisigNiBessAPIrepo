using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Database.Service;

namespace SisigNiBessWebApiAdmin.Repository
{
    public class DbServiceRepository
    {
        public static async Task<List<T>> GetDataListAsync<T>(string query) where T : new()
        {
            var rslt = await new DBService().GetDataListAsync<T>(query);
            return rslt;
        }
        public static async Task<List<Dictionary<string, object>>> GetGetAw<T>(string query)
        {
            var rslt = await new DBService().ExecuteQueryAsync(query);
            return rslt;
        }
        public static async Task<Dictionary<string, object>> GetDataObject<T>(string query)
        {
            var rslt = await new DBService().GetDataObjectAsync(query);
            return rslt;
        }
        public static async Task ExecuteNonQueryCommandAsync(string query)
        {
            await new DBService().ExecuteNonQueryCommandAsync(query);
        }

        public static async Task<bool> InsertDataFromListAsync<T>(List<string> propExemptions, T tableName, string spName) where T : new()
        {
           return await new DBService().InsertDataFromListAsync<T>(propExemptions, tableName, spName);
        }
    }
}
