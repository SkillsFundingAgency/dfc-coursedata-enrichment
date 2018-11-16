using System.Collections.Generic;

namespace Dfc.Coursedata.Enrichment.Services.Interfaces
{
    public interface ILarsSearchResult
    {
        string ODataContext { get; }
        int? ODataCount { get; }
        LarsSearchFacets SearchFacets { get; }
        IEnumerable<LarsSearchResultItem> Value { get; }
    }
}
