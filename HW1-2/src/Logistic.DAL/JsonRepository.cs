using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Logistic.ConsoleClient.Enum;
using Logistic.Core;

namespace Logistic.DAL
{
    public class JsonRepository<T>: IReportRepository<T>
    {
        private readonly string _filePath;
       
        public JsonRepository(string filePath) 
        {
            _filePath = filePath;
        }

        public string FileName { get; set; }
        public void Create(List<T> entities, string entityName)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string fileName = $"{entityName}_{timestamp}.json";
            string filePath = Path.Combine(_filePath, fileName);
            FileName = filePath;

            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, entities);
            }

            string newFileName = $"{entityName}.json";
            string newFilePath = Path.Combine(_filePath, newFileName);
            File.Delete(newFilePath);
            File.Move(filePath, newFilePath);
        }

        public List<T> Read(string fileName)
        {
            FileName = Path.Combine(_filePath, "Resources", fileName);

            using (StreamReader file = File.OpenText(FileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (List<T>)serializer.Deserialize(file, typeof(List<T>));
            }
        }
    }
}
