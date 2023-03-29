﻿using Logistic.ConsoleClient.Enum;
using Logistic.ConsoleClient.Model;
using Logistic.ConsoleClient.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logistic.ConsoleClient.Service
{
    public class ReportService<T> : IReportService<T>
    {
        private readonly IRepository<T> _jsonRepository;
        private readonly IRepository<T> _xmlRepository;

        public ReportService(IRepository<T> jsonRepository, IRepository<T> xmlRepository, string jsonFilePath, string xmlFilePath)
        {
            _jsonRepository = jsonRepository;
            _xmlRepository = xmlRepository;
        }

        public void CreateReport(string fileName, ReportType reportType, List<T> entities)
        {
            if (reportType == ReportType.Json)
            {
                _jsonRepository.Create(entities, "vehicles");
            }
            else if (reportType == ReportType.Xml)
            {
                _xmlRepository.Create(entities, "vehicles");
            }
            else
            {
                throw new ArgumentException("Unsupported report type");
            }
        }

        public List<T> LoadReport(string fileName, ReportType reportType)
        {

            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }
            if (reportType == ReportType.Json)
            {
                return _jsonRepository.Read(fileName);
            }
            else if (reportType == ReportType.Xml)
            {
                return _xmlRepository.Read(fileName);
            }
            else
            {
                throw new ArgumentException("Unsupported report type");
            }
        }
    }
}
