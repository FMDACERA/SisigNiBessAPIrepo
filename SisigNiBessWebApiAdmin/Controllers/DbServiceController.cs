using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Database.Service;
using SisigNiBessWebApiAdmin.Repository;
using System.Collections.Generic;
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
        public async Task<IActionResult> ExecuteNonQuerySPAsync([FromBody] GenericSpPayload payload)
        {
            _dbServiceRepository = new DbServiceRepository();


            if (payload == null || string.IsNullOrEmpty(payload.ModelName))
            {
                return BadRequest(new { success = false, message = "Invalid payload or missing ModelName." });
            }

            try
            {
                // 1. Find the target C# Type dynamically using its class name string
                // Replace "YourWebApiNamespace.Models" with your actual models namespace
                // Replace "YourAssemblyName" with your project's assembly name (usually the project name)
                string fullTypeName = $"SisigNiBessWebApiAdmin.Database.Model.{payload.ModelName}, SisigNiBessWebApiAdmin";
                Type modelType = Type.GetType(fullTypeName);

                if (modelType == null)
                {
                    return BadRequest(new { success = false, message = $"Model type '{payload.ModelName}' not found on the server." });
                }

                // 2. Deserialize the raw inner JSON data block straight into that target C# class type
                object mappedModel;
                try
                {
                    string rawJson = payload.Data.GetRawText();
                    
                    var plainOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    mappedModel = JsonSerializer.Deserialize(rawJson, modelType, plainOptions);

                }
                catch (Exception)
                {
                    return BadRequest(new { success = false, message = $"Failed to map the 'Data' JSON object to the '{payload.ModelName}' model configuration." });
                }

                // 3. Use reflection to target your generic InsertDataFromListAsync<T> repository method

                // Use a direct string literal
                // Change DbServiceRepository.GetType() to typeof(DbServiceRepository)
                MethodInfo method = _dbServiceRepository.GetType().GetMethod("InsertDataFromListAsync");
                MethodInfo genericMethod = method.MakeGenericMethod(modelType);

                // 4. Invoke the method asynchronously and capture the Task<bool> result
                var task = (Task<bool>)genericMethod.Invoke(_dbServiceRepository, new object[] { payload.PropExemptions, mappedModel, payload.SpName });

                bool isSuccess = await task;

                if (isSuccess)
                {
                    return Ok(new { success = true, message = "Record processed successfully." });
                }

                return StatusCode(500, new { success = false, message = "Database execution failed internally." });
            }
            catch (Exception ex)
            {
                // Unpack TargetInvocationException if reflection threw the error
                var actualException = ex is TargetInvocationException ? ex.InnerException : ex;
                return StatusCode(500, new { success = false, error = actualException?.Message ?? ex.Message });
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
