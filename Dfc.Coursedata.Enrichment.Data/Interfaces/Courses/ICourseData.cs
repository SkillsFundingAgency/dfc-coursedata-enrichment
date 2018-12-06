using System;

namespace Dfc.Coursedata.Enrichment.Data.Interfaces.Courses
{
    public interface ICourseData
    {
        Guid ID { get; }
        Guid CourseID { get; }
        string CourseTitle { get; }
    }
}
