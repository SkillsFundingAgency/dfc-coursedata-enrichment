namespace Dfc.Coursedata.Enrichment.Data.Entities.CourseModel
{
    public class Course
    {
        public string CourseId { get; set; }
        public Provider Provider { get; set; }
        public Qualification Qualification { get; set; }
        public CourseDetail CourseDetail { get; set; }      // TODO: to be extended to CourseData/CourseText
    }
}
