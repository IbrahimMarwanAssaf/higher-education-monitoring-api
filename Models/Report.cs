namespace UNIOOP.App.Models
{
    public partial class Report
    {
        public int ReportID { get; set; }
        public string Content { get; set; }
        public DateTime GeneratedAt { get; set; }

        public Report(int reportID, string content, DateTime generatedAt)
        {
            ReportID = reportID;
            Content = content;
            GeneratedAt = generatedAt;
        }
    }
}