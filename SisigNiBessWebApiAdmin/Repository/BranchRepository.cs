using SisigNiBessWebApiAdmin.Database.Model;
using SisigNiBessWebApiAdmin.Database.Service;

namespace SisigNiBessWebApiAdmin.Repository
{
    public class BranchRepository
    {
        public static async Task<List<BRANCH>> GetBranchesAsync()
        {
            DBService db = new();
            var rslt = await db.GetDataListAsync<BRANCH>
                        ("SELECT a.*,b.FULL_NAME FROM branch_list a JOIN employee_list b ON a.emp_id = b.employee_id " +
                        " where a.branch_name not in ('ADJUSTMENTS')");

            if (rslt.Count > 0)
                return rslt;
            else
                return new List<BRANCH>();
        }

        public static async Task SaveBranchAsync(BRANCH branch)
        {
            var sqlcommand = "insert into branch_list " +
                            "(branch_name,branch_location,rent,emp_id,beg_solo,beg_barkada) values " +
                            "('" + branch.BRANCH_NAME.ToUpper() + "','" + branch.BRANCH_LOCATION.ToUpper() + "'," + branch.RENT + "," + branch.EMP_ID + "," + branch.BEG_SOLO + "," + branch.BEG_BARKADA + ")";
            await new DBService().ExecuteNonQueryCommandAsync(sqlcommand);
        }
        public static async Task UpdateBranchAsync(BRANCH branch)
        {
            var sqlcommand = "update branch_list set branch_name = '" + branch.BRANCH_NAME.ToUpper() +
                            "', branch_location = '" + branch.BRANCH_LOCATION.ToUpper() +
                            "', rent = " + branch.RENT + ", beg_solo = " + branch.BEG_SOLO + ", beg_barkada = " + branch.BEG_BARKADA + ", emp_id = " + branch.EMP_ID + " where branch_id = " + branch.BRANCH_ID;
            await new DBService().ExecuteNonQueryCommandAsync(sqlcommand);
        }
        public static async Task DeleteBranchAsync(BRANCH branch)
        {
            var sqlcommand = "delete from branch_list where branch_id = " + branch.BRANCH_ID;
            await new DBService().ExecuteNonQueryCommandAsync(sqlcommand);
        }
        
    }
}
