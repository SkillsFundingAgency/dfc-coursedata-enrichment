using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.Coursedata.Enrichment.Importer.Entities.CSV;
using Provider = Dfc.ProviderPortal.Providers.Provider;

namespace Dfc.Coursedata.Enrichment.Importer.Interfaces
{
    public interface IProviderCourseData
    {
        Task<IEnumerable<Provider>> GetProviderData();
        IEnumerable<ILR> GetILRData();

        Entities.CSV.Provider GetProvider();
        List<CourseDetail> GetCourseDetails();
        List<Venue> GetVenues();
        List<Qualification> GetQualifications();
        List<Opportunity> GetOpportunities();
    }
}
