using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Logistic.ConsoleClient.Model;
using Logistic.ConsoleClient.Service;

namespace Logistic.ConsoleClient.Repository
{
    public class JsonRepository<T>: ReportService<T>
    {
        private readonly string _filePath;

        public JsonRepository(string filePath) 
        {
            _filePath = filePath;
        }

        public void Create(List<Vehicle> entities, string entityName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fileName = $"{entityName}_{timestamp}.json";
            string filePath = Path.Combine(_filePath, fileName);

            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, entities);
            }
        }

        public List<Vehicle> Read(string fileName)
        {
            string filePath = Path.Combine(_filePath, fileName);

            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (List<Vehicle>)serializer.Deserialize(file, typeof(List<Vehicle>));
            }
        }
    }
}
