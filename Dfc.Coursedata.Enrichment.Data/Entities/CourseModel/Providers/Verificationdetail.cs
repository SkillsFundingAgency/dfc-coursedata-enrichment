using Dfc.Coursedata.Enrichment.Data.Interfaces.Providers;

namespace Dfc.Coursedata.Enrichment.Data.Entities.CourseModel.Providers
{
    public class Verificationdetail : IVerificationdetail
    {
        public string VerificationAuthority { get; set; }
        public string VerificationID { get; set; }
    }
}
