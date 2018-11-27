using System.Collections.Generic;
using Dfc.ProviderPortal.Providers;

namespace Dfc.Coursedata.Enrichment.Data.Interfaces
{
    public interface IGremlinQuery
    {
        bool InsertProviders(IEnumerable<Provider> providers);
        Entities.Provider GetQualificationsByUkprn(string ukprn);
        void AddProviderQualificationEdge(string ukprn, List<string> larsId);
        void DeLinkQualificationsFromProvider(string ukprn, List<string> larsId);
    }
}
