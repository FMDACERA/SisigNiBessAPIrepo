namespace SisigNiBessWebApiAdmin.Database.Model
{
    public class SALES_INVENTORY
    {
        public Int64 DAILY_SALES_ID { get; set; }
        public Int64 CUTOFF_ID { get; set; }
        public Int64 BRANCH_ID { get; set; }
        public decimal GROSS_SALES { get; set; }
        public decimal CASH_ONHAND { get; set; }
        public DateTime SALES_DATE { get; set; }
        public string BRANCH_NAME { get; set; }
        public string CUTOFF_DESC { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string REASON { get; set; }

        public decimal TOTAL_GROSS { get; set; }
        public decimal TOTAL_ONHAND { get; set; }
        public Int64 IS_APPROVED { get; set; }

        // private SALES_INVENTORY_DETAILS salesinventorydetails;
        public SALES_INVENTORY_DETAILS SalesInventoryDetails { get; set; }
        //{
        //    get
        //    {
        //        salesinventorydetails = SalesRepo.GeSalesInventoryDetails(DAILY_SALES_ID);

        //        /*retry if server is disconnected*/
        //        //if (salesinventorydetails is null)
        //        //    salesinventorydetails = SalesRepo.GeSalesInventoryDetails(DAILY_SALES_ID);

        //        return salesinventorydetails;
        //    }
        //    set { salesinventorydetails = value; }
        //}
    }
}
