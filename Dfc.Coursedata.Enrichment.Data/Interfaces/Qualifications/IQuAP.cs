using System;
using Dfc.Coursedata.Enrichment.Data.Entities.CourseModel.Providers;
using Dfc.Coursedata.Enrichment.Data.Entities.CourseModel.Qualifications;

namespace Dfc.Coursedata.Enrichment.Data.Interfaces.Qualifications
{
    public interface IQuAP
    {
        Guid ID { get; }
        Qualification Qualification { get; }
        Provider Provider { get; }
    }
}
