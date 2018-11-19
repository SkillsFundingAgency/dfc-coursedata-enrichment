using System;
using System.Collections.Generic;
using System.Text;
using Dfc.Coursedata.Enrichment.Common;
using Dfc.Coursedata.Enrichment.Services.Enums;
using Dfc.Coursedata.Enrichment.Services.Interfaces;

namespace Dfc.Coursedata.Enrichment.Services
{
    public class LarsSearchCriteria : ValueObject<LarsSearchCriteria>, ILarsSearchCriteria
    {
        public string Search { get; }
        public int Top { get; }
        public int Skip { get; }
        public bool Count => true;
        public string Filter { get; }
        public IEnumerable<LarsSearchFacet> Facets { get; }

        public LarsSearchCriteria(
            string search,
            int top,
            int skip,
            string filter = null,
            IEnumerable<LarsSearchFacet> facets = null)
        {
            Throw.IfNullOrWhiteSpace(search, nameof(search));
            Throw.IfLessThan(1, top, nameof(top));
            Throw.IfLessThan(0, skip, nameof(skip));

            Search = search;
            Top = top;
            Skip = skip;
            Filter = filter;
            Facets = facets;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Search;
            yield return Top;
            yield return Skip;
            yield return Count;
            yield return Filter;
            yield return Facets;
        }
    }
}
