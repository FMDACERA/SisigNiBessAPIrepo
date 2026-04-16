using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Database.Service;

namespace SisigNiBessWebApiAdmin.Repository
{
    public class SalesRepositoty
    {
        public static List<CHART_MODEL> GetBranchChartDetailsAsync(Int64 CutoffId)
        {
            var rslt = new DBService().GetDataListAsync<CHART_MODEL>(
@"
SELECT daily_sales.cutoff_id CUTOFF_ID,
	 BRANCH_NAME,
	 CONCAT(CAST(DATE_FORMAT(`cutoff`.`CUTOFF_FRM`,'%M %d, %Y') AS CHAR CHARSET utf8mb3),' - ',CAST(DATE_FORMAT(`cutoff`.`CUTOFF_TO`,'%M %d, %Y') AS CHAR CHARSET utf8mb3)) AS CUTOFF_DESC,
	 SUM(gross_sales) AS GROSS_SALES,
	 SUM(cash_onhand) AS CASH_ONHAND
FROM daily_sales 
JOIN branch_list ON daily_sales.branch_id = branch_list.branch_id
JOIN cutoff ON daily_sales.cutoff_id = cutoff.cutoff_id
WHERE daily_sales.cutoff_id = " + CutoffId + " AND daily_sales.branch_id != 6 GROUP BY daily_sales.cutoff_id,daily_sales.branch_id order by daily_sales.branch_id");
            return rslt;
        }
        public static List<SALES_INVENTORY> GetLstOfSalesInventoryGroupByDateAsync()
        {
            try
            {
                var tmp = new List<SALES_INVENTORY>();
                var rslt = new DBService().GetDataListAsync<SALES_INVENTORY>("SELECT * FROM daily_sales order by sales_date desc limit 300");

                foreach (var item in rslt)
                {
                    if (!tmp.Where(p => p.SALES_DATE == item.SALES_DATE).Any())
                    {
                        var haspending = rslt.Where(p => p.SALES_DATE == item.SALES_DATE && p.IS_APPROVED == 0).Any();
                        item.TOTAL_GROSS = rslt.Where(p => p.SALES_DATE == item.SALES_DATE).Sum(o => o.GROSS_SALES);
                        item.TOTAL_ONHAND = rslt.Where(p => p.SALES_DATE == item.SALES_DATE).Sum(o => o.CASH_ONHAND);
                        item.IS_APPROVED = haspending ? 0 : 1;
                        tmp.Add(item);
                    }
                }

                return tmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
