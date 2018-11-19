using System;
using System.Collections.Generic;
using System.Text;
using Dfc.Coursedata.Enrichment.Common.Interfaces;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.Coursedata.Enrichment.Services.Interfaces;
using Dfc.ProviderPortal.Providers;

namespace Dfc.Coursedata.Enrichment.Importer.Interfaces
{
    public interface IGremlinInsert
    {
        bool InsertProviders(IEnumerable<Provider> providers);
        bool InsertLars(IResult<ILarsSearchResult> larsData);
        bool InsertEdges(IEnumerable<ILR> ilrData);
    }
}
