namespace Dfc.Coursedata.Enrichment.Data.Entities.CourseModel
{
    public class CourseDetail
    {
        public string CourseDetailId { get; set; }
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseSummary { get; set; }
        public Venue Venue { get; set; }
    }
}
