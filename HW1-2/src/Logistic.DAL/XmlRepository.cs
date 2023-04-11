using Logistic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Logistic.DAL
{
    public class XmlRepository<T> : IReportRepository<T>
    {
        private readonly string _directoryPath;

        public XmlRepository(string directoryPath)
        {
            _directoryPath = directoryPath;
            Directory.CreateDirectory(_directoryPath );
        }

        public string FileName { get; set; }

        public void Create(List<T> entities, string entityName)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string fileName = $"{entityName}_{timestamp}.xml";
            string filePath = Path.Combine(_directoryPath, fileName);
            FileName = fileName;

            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (StreamWriter file = new StreamWriter(filePath, false))
            {
                serializer.Serialize(file, entities);
            }
            string newFileName = $"{entityName}.xml";
            string newFilePath = Path.Combine(_directoryPath, newFileName);
            File.Delete(newFilePath);
            File.Move(filePath, newFilePath);
        }

        public List<T> Read(string fileName)
        {
            string filePath = Path.Combine(_directoryPath, "Resources", fileName);
            FileName = fileName;
            if (!File.Exists(filePath))
            {
                 throw new FileNotFoundException($"The file {filePath} does not exist.");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (StreamReader file = new StreamReader(filePath))
            {
                return (List<T>)serializer.Deserialize(file);
            }
        }
    }
}
