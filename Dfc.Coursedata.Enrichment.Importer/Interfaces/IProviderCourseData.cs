using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.ProviderPortal.Providers;

namespace Dfc.Coursedata.Enrichment.Importer.Interfaces
{
    public interface IProviderCourseData
    {
        Task<IEnumerable<Provider>> GetProviderData();
        IEnumerable<ILR> GetILRData();
    }
}
