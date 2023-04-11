using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logistic.Models;

namespace Logistic.Core
{
    public interface IReportRepository<T>
    {
        string FileName { get; set; }
        void Create(List<T> entities, string fileName);
        List<T> Read(string fileName);
    }
}
