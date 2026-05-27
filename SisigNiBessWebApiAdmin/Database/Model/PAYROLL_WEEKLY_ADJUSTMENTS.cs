namespace SisigNiBessWebApiAdmin.Database.Model
{
    public class PAYROLL_WEEKLY_ADJUSTMENTS
    {
        public Int64 PAYWEEKADJ_ID { get; set; }
        public Int64 PAYROLL_ID { get; set; }
        public Int64 PAYADJ_ID { get; set; }
        public decimal ADJUSTMENT_AMOUNT { get; set; }
        public string ADJ_DAY { get; set; }
        public string ADJUSTMENT_TYPE { get; set; }
        public Int64 SOURCE_ID { get; set; }
        public Int64 ADDED_VALUE { get; set; }
    }
}
