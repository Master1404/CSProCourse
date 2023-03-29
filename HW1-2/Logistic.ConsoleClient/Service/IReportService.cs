using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logistic.ConsoleClient.Enum;

namespace Logistic.ConsoleClient.Service
{
    public interface IReportService<T>
    {
        void CreateReport(string fileName, ReportType reportType, List<T> entities);
        List<T> LoadReport(string fileName, ReportType reportType);
    }
}
