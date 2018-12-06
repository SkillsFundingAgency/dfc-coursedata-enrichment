using System;
using System.Collections.Generic;
using Dfc.Coursedata.Enrichment.Data.Entities.CourseModel.Courses;
using Dfc.Coursedata.Enrichment.Data.Entities.CourseModel.Qualifications;

namespace Dfc.Coursedata.Enrichment.Data.Interfaces.Courses
{
    public interface ICourse
    {
        Guid ID { get; }
        QuAP QuAP { get; }
        IEnumerable<CourseRun> CourseRun { get; }
    }
}