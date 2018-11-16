using System.Threading.Tasks;
using Dfc.Coursedata.Enrichment.Common.Interfaces;

namespace Dfc.Coursedata.Enrichment.Services.Interfaces
{
    public interface ILarsSearchService
    {
        Task<IResult<ILarsSearchResult>> SearchAsync(ILarsSearchCriteria criteria);

        // TODO: add SearchAsync / GetAsync to return all
    }
}
