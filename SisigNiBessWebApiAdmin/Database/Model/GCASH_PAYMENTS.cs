namespace SisigNiBessWebApiAdmin.Database.Model
{
    public class GCASH_PAYMENTS
    {
        public Int64 GCPAY_ID { get; set; }
        public Int64 CUTOFF_ID { get; set; }
        public decimal GC_AMOUNT { get; set; }
        public DateTime GC_DATE { get; set; }
        public Int64 BRANCH_ID { get; set; }
        public Int64 SOURCE_ID { get; set; }

    }
}
