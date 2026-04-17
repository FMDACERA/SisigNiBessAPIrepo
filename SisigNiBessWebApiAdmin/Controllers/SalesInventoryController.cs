using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Repository;
using System.Threading.Tasks;

namespace SisigNiBessWebApiAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesInventoryController : ControllerBase
    {
        [HttpGet(Name = "GetInventoryList")]
        public async Task<List<SALES_INVENTORY>> GetInventoryList()
        {
            return await SalesRepositoty.GetLstOfSalesInventoryGroupByDateAsync();
        }
        [HttpGet]
        [Route("GetBranchChartDetails")]
        public async Task<List<CHART_MODEL>> GetBranchChartDetails([FromQuery] Int64 CutoffId)
        {
            return await SalesRepositoty.GetBranchChartDetailsAsync(CutoffId);
        }

        [HttpGet]
        [Route("GetSalesInventoryDetailsByDateAsync")]
        public async Task<List<SALES_INVENTORY_DETAILS>> GetSalesInventoryDetailsByDateAsync()
        {
            return await SalesRepositoty.GetSalesInventoryDetailsByDateAsync();
        }
        [HttpGet]
        [Route("GetSalesReportByLastCutoffAsync")]
        public async Task<SALES_REPORT> GetSalesReportByLastCutoffAsync()
        {
            return await SalesRepositoty.GetSalesReportByLastCutoffAsync();
        }
    }
}
