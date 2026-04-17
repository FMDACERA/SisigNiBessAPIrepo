using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Database.Service;
using SisigNiBessWebApiAdmin.Repository;
using System.Collections.Generic;

namespace SisigNiBessWebApiAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbServiceController : ControllerBase
    {
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


    }
}
