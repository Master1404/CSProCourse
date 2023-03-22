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
        public void CreateReport(string fileName, ReportType reportType, List<T> entities)
        {
            string extension = reportType == ReportType.Xml ? ".xml" : ".json";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName + extension);

            using (StreamWriter file = File.CreateText(filePath))
            {
                if (reportType == ReportType.Xml)
                {
                    var serializer = new XmlSerializer(typeof(List<T>));
                    serializer.Serialize(file, entities);
                }
                else if (reportType == ReportType.Json)
                {
                    string json = JsonConvert.SerializeObject(entities, Formatting.Indented);
                    file.Write(json);
                }
            }
        }
        
        public List<T> LoadReport(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileName));
            Console.WriteLine($"Loading report from path: {filePath}");

            if(!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", fileName);
            }

            string extension = Path.GetExtension(filePath);
            if (extension == ".xml")
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                using (FileStream stream = File.OpenRead(filePath))
                {
                    return (List<T>)serializer.Deserialize(stream);
                }
            }
            else if (extension == ".json")
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<T>>(json);
            }
            else
            {
                throw new NotSupportedException("File format is not supported");
            }
        }
    }
}
