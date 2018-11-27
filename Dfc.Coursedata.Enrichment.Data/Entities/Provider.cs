using System.Collections.Generic;

namespace Dfc.Coursedata.Enrichment.Data.Entities
{
    public class Provider
    {
        public string Id { get; set; }
        public string ProviderName { get; set; }
        public IEnumerable<Qualification> Qualifications { get; set; }
    }
}
