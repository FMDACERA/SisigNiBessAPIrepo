namespace SisigNiBessWebApiAdmin.Database.Model
{
    public class SALES_INVENTORY_DETAILS
    {
        public Int64 DAILY_SALES_DETAIL_ID { get; set; }
        public Int64 DAILY_SALES_ID { get; set; }
        public Int64 BEGINNING_SOLO { get; set; }
        public Int64 ENDING_SOLO { get; set; }
        public Int64 BEGINNING_BRKD { get; set; }
        public Int64 ENDING_BRKD { get; set; }
        public Int64 BEGINNING_EGG { get; set; }
        public Int64 ENDING_EGG { get; set; }
        public decimal GCASH_AMT { get; set; }
        public decimal EXPENSE_AMT { get; set; }
        public decimal GROSS_AMT { get; set; }
        public decimal TOTAL_AMT { get; set; }
        public decimal ACTUAL_CASH { get; set; }
        public decimal QOUTA { get; set; }
        public decimal SHORT { get; set; }
        public string SENT_BY { get; set; }
        public string SENT_DATE { get; set; }

        public decimal CASH_ADVANCE { get; set; }
        public string BRANCH_NAME { get; set; }
        public DateTime SALES_DATE { get; set; }

        public Int64 OUT_SOLO { get; set; }
        public Int64 OUT_BRKD { get; set; }
        public Int64 OUT_EGG { get; set; }
        public Int64 IS_APPROVED { get; set; }
    }
}
