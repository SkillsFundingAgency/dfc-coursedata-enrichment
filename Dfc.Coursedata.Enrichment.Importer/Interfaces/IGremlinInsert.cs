using System.Collections.Generic;
using Dfc.Coursedata.Enrichment.Common.Interfaces;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.Coursedata.Enrichment.Importer.Entities.CSV;
using Dfc.Coursedata.Enrichment.Services.Interfaces;
using Provider = Dfc.ProviderPortal.Providers.Provider;

namespace Dfc.Coursedata.Enrichment.Importer.Interfaces
{
    public interface IGremlinInsert
    {
        bool InsertProviders(IEnumerable<Provider> providers);
        bool InsertLars(IResult<ILarsSearchResult> larsData);
        bool InsertEdges(IEnumerable<ILR> ilrData);
        bool InsertIlrData(List<ILR> ilrData);
        void InsertCsvProvider(Entities.CSV.Provider provider);
        List<string> InsertCsvQualification(string ukprn, List<Qualification> qualifications);
        void InsertCsvCourseDetails(string ukprn, List<CourseDetail> courseDetails);
        void InsertCsvVenues(List<Venue> venues);
        void InsertCsvOpportunities(List<Opportunity> opportunities);
        void InsertCsvCourseDetailsOnly(string ukprn, List<CourseDetail> courseDetails);
    }
}
