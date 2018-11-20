using System.Collections.Generic;
using Dfc.ProviderPortal.Providers;

namespace Dfc.Coursedata.Enrichment.Data.Interfaces
{
    public interface IGremlinQuery
    {
        bool InsertProviders(IEnumerable<Provider> providers);
        bool GetQualificationsByUkprn(string ukprn);
    }
}
