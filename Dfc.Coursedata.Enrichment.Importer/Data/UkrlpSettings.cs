using Dfc.Coursedata.Enrichment.Importer.Interfaces;

namespace Dfc.Coursedata.Enrichment.Importer.Data
{
    public class UkrlpSettings : IUkrlpSettings
    {
        public string UrlGetAllProviders { get; set; }
        public string Code { get; set; }
    }
}
