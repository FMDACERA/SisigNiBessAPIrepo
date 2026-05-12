namespace SisigNiBessWebApiAdmin.Database.Model
{
    public class DELIVERY
    {
        public Int64 DELIVERY_ID { get; set; }
        public string DELIVERY_CODE { get; set; }
        public Int64 EXPENSE_ID { get; set; }
        public Decimal DELIVERY_AMOUNT { get; set; }
        public string DELIVERY_STATUS { get; set; }
        public DateTime DELIVERY_DATE { get; set; }
        public string? APPROVED_BY { get; set; }
        public string EXPENSE_NAME { get; set; }
        
    }
}
