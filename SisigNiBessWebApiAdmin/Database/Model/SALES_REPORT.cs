namespace SisigNiBessWebApiAdmin.Database.Model
{
    public class SALES_REPORT
    {
        public Int64 CUTOFF_ID { get; set; }
        public decimal GROSS_SALES { get; set; }
        public decimal CASH_ONHAND { get; set; }
        public decimal EXPENSES { get; set; }
        public decimal GCASH { get; set; }
        public string IS_LOCKED { get; set; }
        public Decimal NET_PROFIT { get; set; }
    }
}
