namespace SisigNiBessWebApiAdmin.Database.Model
{
    public class BRANCH
    {
        public Int64 BRANCH_ID { get; set; }
        public string BRANCH_NAME { get; set; }
        public string BRANCH_LOCATION { get; set; }
        public Int64 BEG_SOLO { get; set; }
        public Int64 BEG_BARKADA { get; set; }
        public decimal RENT { get; set; }
        public Int64 EMP_ID { get; set; }
        // private string fullname;
        public string FULL_NAME { get; set; }
    }
}
