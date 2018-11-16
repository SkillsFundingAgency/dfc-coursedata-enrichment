using System.Collections.Generic;
using Dfc.Coursedata.Enrichment.Services.Enums;

namespace Dfc.Coursedata.Enrichment.Services.Interfaces
{
    public interface ILarsSearchCriteria
    {
        string Search { get; }
        string Filter { get; }
        IEnumerable<LarsSearchFacet> Facets { get; }
        bool Count { get; }
    }
}
