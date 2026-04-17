using SisigNiBessWebApiAdmin.Controllers;
using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Database.Service;
using System.Reflection;
using System.Threading.Tasks;

namespace SisigNiBessWebApiAdmin.Repository
{
    public class SalesRepositoty
    {
        public static async Task<List<CHART_MODEL>> GetBranchChartDetailsAsync(Int64 CutoffId)
        {
            /*
            var rslt = await new DBService().GetDataListAsync<CHART_MODEL>(
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
            */

            var qry = @"
SELECT daily_sales.cutoff_id CUTOFF_ID,
	 BRANCH_NAME,
	 CONCAT(CAST(DATE_FORMAT(`cutoff`.`CUTOFF_FRM`,'%M %d, %Y') AS CHAR CHARSET utf8mb3),' - ',CAST(DATE_FORMAT(`cutoff`.`CUTOFF_TO`,'%M %d, %Y') AS CHAR CHARSET utf8mb3)) AS CUTOFF_DESC,
	 SUM(gross_sales) AS GROSS_SALES,
	 SUM(cash_onhand) AS CASH_ONHAND
FROM daily_sales 
JOIN branch_list ON daily_sales.branch_id = branch_list.branch_id
JOIN cutoff ON daily_sales.cutoff_id = cutoff.cutoff_id
WHERE daily_sales.cutoff_id = " + CutoffId + " AND daily_sales.branch_id != 6 GROUP BY daily_sales.cutoff_id,daily_sales.branch_id order by daily_sales.branch_id";

            var items = await new DbServiceController().GetDataListAsync(qry);
            var rslt = ConvertToModelList<CHART_MODEL>(items);
            return rslt;

        }
        public static async Task<List<SALES_INVENTORY>> GetLstOfSalesInventoryGroupByDateAsync()
        {
            try
            {
                var tmp = new List<SALES_INVENTORY>();
                var rslt = await new DBService().GetDataListAsync<SALES_INVENTORY>("SELECT * FROM daily_sales order by sales_date desc limit 300");

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
        public static async Task<List<SALES_INVENTORY_DETAILS>> GetSalesInventoryDetailsByDateAsync()
        {
            return await new DBService().GetDataListAsync<SALES_INVENTORY_DETAILS>
                            ("select a.*, b.branch_name, b.sales_date, (a.beginning_solo - a.ending_solo) as OUT_SOLO, (a.beginning_brkd - a.ending_brkd) as OUT_BRKD, (a.beginning_egg - a.ending_egg) as OUT_EGG, c.IS_APPROVED" +
                            " from daily_sales_details a" +
                            " join vwdailysales b" +
                            " on a.daily_sales_id = b.daily_sales_id" +
                            " join daily_sales c" +
                            " on a.daily_sales_id = c.daily_sales_id" +
                            " where b.sales_date = (SELECT sales_date FROM daily_sales ORDER BY daily_sales_id DESC LIMIT 1 ) order by b.branch_id");
        }
        public static async Task<SALES_REPORT> GetSalesReportByLastCutoffAsync()
        {
            var rslt = await new DBService().GetDataListAsync<SALES_REPORT>("select * from vwsalesreport order by cutoff_id desc limit 1");
            return rslt.FirstOrDefault();
        }


        public static List<T> ConvertToModelList<T>(List<Dictionary<string, object>> dictionaryList) where T : new()
        {
            var list = new List<T>();

            // Get properties once outside the loop for performance
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var dict in dictionaryList)
            {
                var item = new T();

                foreach (var prop in properties)
                {
                    // Match dictionary key to property name (case-insensitive)
                    if (dict.TryGetValue(prop.Name, out var value) && value != DBNull.Value)
                    {
                        // Ensure the value type matches the property type
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        try
                        {
                            object convertedValue = Convert.ChangeType(value, targetType);
                            prop.SetValue(item, convertedValue);
                        }
                        catch
                        {
                            // Log or handle type mismatch (e.g., trying to put a string into an int)
                            continue;
                        }
                    }
                }
                list.Add(item);
            }

            return list;
        }

    }
}
