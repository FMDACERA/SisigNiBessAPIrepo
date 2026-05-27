using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Database.Service;
using SisigNiBessWebApiAdmin.Repository;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.Json;

namespace SisigNiBessWebApiAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbServiceController : ControllerBase
    {
        private DbServiceRepository _dbServiceRepository;

        //[HttpGet(Name = "GetDataListAsync")]
        //public async Task<List<T>> GetDataListAsync<T>([FromQuery] string query) where T : new()
        //{
        //    var rslt = await DbServiceRepository.GetDataListAsync<T>(query);
        //    return rslt;
        //}



        [HttpPost("ExecuteNonQueryCommandAsync")]
        public async Task ExecuteNonQueryCommandAsync([FromQuery] string query)
        {
            await DbServiceRepository.ExecuteNonQueryCommandAsync(query);
        }

        [HttpGet("GetDataListAsync")]
        public async Task<List<Dictionary<string, object>>> GetDataListAsync([FromQuery] string qry)
        {
            return await DbServiceRepository.GetGetAw<Dictionary<string, object>>(qry);
        }

        [HttpGet("GetDataObject")]
        public async Task<Dictionary<string, object>> GetDataObject([FromQuery] string qry)
        {
            return await DbServiceRepository.GetDataObject<Dictionary<string, object>>(qry);
        }

        [HttpPost("ExecuteNonQuerySPA")]
       [HttpPost("ExecuteNonQuerySPA")]
public async Task<IActionResult> ExecuteNonQuerySPAsync([FromBody] GenericSpPayload payload)
{
    if (payload == null || string.IsNullOrEmpty(payload.SpName))
    {
        return BadRequest(new { success = false, message = "Invalid payload configuration." });
    }

    try
    {
        // 1. Parse the JSON data into a raw dictionary of keys and values 
        // This completely bypasses System.Text.Json class reflection!
        var rawJsonText = payload.Data.GetRawText();
        var dataDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(rawJsonText);

        if (dataDictionary == null)
        {
            return BadRequest(new { success = false, message = "Failed to parse data object into key/value pairs." });
        }

        // 2. Open the database connection directly right here in the endpoint
        await using (var connection = new MySqlConnection(DBService.ConnectionStrng))
        {
            await connection.OpenAsync();

            await using (var command = new MySqlCommand(payload.SpName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // 3. Loop through every key-value pair sent from the branch app
                foreach (var kvp in dataDictionary)
                {
                    // If the property name is in your exemptions list (like "Id"), skip it
                    if (payload.PropExemptions != null && payload.PropExemptions.Contains(kvp.Key))
                    {
                        continue;
                    }

                    // Extract the raw value cleanly
                    object value = kvp.Value;

                    // System.Text.Json parses numbers and objects into JsonElement structures. 
                    // We extract the clean underlying value out of it safely.
                    if (value is JsonElement element)
                    {
                        switch (element.ValueKind)
                        {
                            case JsonValueKind.String:
                                value = element.GetString();
                                break;
                            case JsonValueKind.Number:
                                // Automatically handles integers, decimals, and quantities safely
                                if (element.TryGetInt64(out long l)) value = l;
                                else value = element.GetDecimal();
                                break;
                            case JsonValueKind.True:
                                value = true;
                                break;
                            case JsonValueKind.False:
                                value = false;
                                break;
                            case JsonValueKind.Null:
                                value = DBNull.Value;
                                break;
                            default:
                                value = element.GetRawText();
                                break;
                        }
                    }

                    // 4. Map it straight into your MySQL stored procedure parameter
                    command.Parameters.AddWithValue("IN_" + kvp.Key, value ?? DBNull.Value);
                }

                // 5. Execute seamlessly on your Aiven database
                await command.ExecuteNonQueryAsync();
            }
        }

        return Ok(new { success = true, message = "Record processed successfully without reflection!" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { success = false, error = ex.Message });
    }
}
    }

    public class GenericSpPayload
    {
        public List<string> PropExemptions { get; set; } = new List<string>();
        public string SpName { get; set; }
        public string ModelName { get; set; } // e.g., "INVENTORY_BEGINNING" or "DAILY_SALES"
        public JsonElement Data { get; set; }  // Flexible placeholder for your actual data object
    }
}
