using Dfc.Coursedata.Enrichment.Services.Interfaces;

namespace Dfc.Coursedata.Enrichment.Services
{
    public class LarsSearchSettings : ILarsSearchSettings
    {
        public string ApiUrl { get; set; }
        public string ApiVersion { get; set; }
        public string ApiKey { get; set; }
        public string Indexes { get; set; }
        public int ItemsPerPage { get; set; }
        public string PageParamName { get; set; }
    }
}
