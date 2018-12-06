using System;
using Dfc.Coursedata.Enrichment.Data.Entities.CourseModel.Venues;

namespace Dfc.Coursedata.Enrichment.Data.Interfaces.Courses
{
    public interface ICourseRun
    {
        Venue Venue { get; }
        string Price { get; }
        string Duration { get; }
        string StudyMode { get; }
        string Attendance { get; }
        Guid CourseID { get; }
        string CourseURL { get; }
        string Pattern { get; }
        string Requirements { get; }

    }
}