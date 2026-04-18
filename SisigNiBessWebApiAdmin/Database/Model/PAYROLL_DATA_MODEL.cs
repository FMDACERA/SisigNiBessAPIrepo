namespace SisigNiBessWebApiAdmin.Database.Model
{
    public class PAYROLL_DATA_MODEL
    {
        public Int64 PAYROLL_ID { get; set; }
        public Int64 EMPLOYEE_ID { get; set; }
        public Int64 CUTOFF_ID { get; set; }
        public decimal GROSS_PAY { get; set; }
        public decimal ADJUSTMENTS { get; set; }
        public decimal DEDUCTIONS { get; set; }
        public decimal NET_PAY { get; set; }
        public DateTime WEEK_START { get; set; }
        public DateTime WEEK_END { get; set; }
        public string FULL_NAME { get; set; }
        public decimal DAILY_RATE { get; set; }
        public Int64 IS_LOCKED { get; set; }
    }
}
