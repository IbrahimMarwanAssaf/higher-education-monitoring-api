using UNIOOP.App.Models;

namespace UNIOOP.App.Interfaces
{
    public partial interface IReportGenerator
    {
        public Report GenerateReport(ReportType type);
    }
}