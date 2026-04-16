using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Repository;

namespace SisigNiBessWebApiAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesInventoryController : ControllerBase
    {
        [HttpGet(Name = "GetInventoryList")]
        public List<SALES_INVENTORY> GetInventoryList()
        {
            return SalesRepositoty.GetLstOfSalesInventoryGroupByDateAsync();
        }
        [HttpGet]
        [Route("GetBranchChartDetails")]
        public List<CHART_MODEL> GetBranchChartDetails([FromQuery] Int64 CutoffId)
        {
            return SalesRepositoty.GetBranchChartDetailsAsync(CutoffId);
        }
    }
}
