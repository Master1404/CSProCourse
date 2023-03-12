using Logistic.ConsoleClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logistic.ConsoleClient.Repository
{
    public class XmlRepository<T>: ReportService<T>
    {
        private readonly string _directoryPath;

        public XmlRepository(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public void Create(List<T> entities, string entityName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fileName = $"{entityName}_{timestamp}.xml";
            string filePath = Path.Combine(_directoryPath, fileName);

            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (StreamWriter file = new StreamWriter(filePath))
            {
                serializer.Serialize(file, entities);
            }
        }

        public List<T> Read(string fileName)
        {
            string filePath = Path.Combine(_directoryPath, fileName);

            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (StreamReader file = new StreamReader(filePath))
            {
                return (List<T>)serializer.Deserialize(file);
            }
        }
    }
}
