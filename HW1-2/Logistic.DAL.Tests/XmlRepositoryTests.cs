using Logistic.Models.Enum;
using Logistic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.DAL.Tests
{
    public class XmlRepositoryTests
    {
        private readonly XmlRepository<Vehicle> _xmlRepository;
        private readonly string _directoryPath = @"D:\C#\C-Pro_\CSProCourse\HW1-2\Logistic.DAL.Tests\Resources";

        public XmlRepositoryTests()
        {
            _xmlRepository = new XmlRepository<Vehicle>(_directoryPath);
        }

        [Fact]
        public void Read_WhenValidXml_DeserializeSucessfully()
        {
            // Arrange
            var testPath = Path.Combine("xml_serializer_Vehicle_test.xml");

            // Act
            var result = _xmlRepository.Read(testPath);

            // Assert
            Assert.Equal(0, result[0].Id);
            Assert.Equal("ABC", result[0].Number);
            Assert.Equal(10, result[0].MaxCargoWeightKg);
            Assert.Equal(10, result[0].CurrentCargoWeight);
            Assert.Equal(10, result[0].CurrentCargoVolume);
            Assert.Equal(10, result[0].MaxCargoWeightPnd);
            Assert.Equal(10, result[0].MaxCargoVolume);
            Assert.Equal(VehicleType.Car, result[0].Type);
        }

        [Fact]
        public void Create_WhenValidEntity_SerializeSucessfully()
        {
            // Arrange
            var testDirectory = Path.Combine(_directoryPath, "CreateTest");
            Directory.CreateDirectory(testDirectory);
            var testPath = "xml_serializer_Vehicle_test.xml";

            // Act
            var entities = new List<Vehicle>();
            entities.Add(new Vehicle
            {
                Id = 0,
                Number = "ABC",
                MaxCargoWeightKg = 10,
                CurrentCargoWeight = 10,
                CurrentCargoVolume = 10,
                MaxCargoWeightPnd = 10,
                MaxCargoVolume = 10,
                Type = VehicleType.Car
            });

            _xmlRepository.Create(entities, testPath);

            // Assert
            var result = _xmlRepository.Read(testPath);
            Assert.Single(result);
            Assert.Equal(0, result[0].Id);
            Assert.Equal("ABC", result[0].Number);
            Assert.Equal(10, result[0].MaxCargoWeightKg);
            Assert.Equal(10, result[0].CurrentCargoWeight);
            Assert.Equal(10, result[0].CurrentCargoVolume);
            Assert.Equal(10, result[0].MaxCargoWeightPnd);
            Assert.Equal(10, result[0].MaxCargoVolume);
            Assert.Equal(VehicleType.Car, result[0].Type);
        }
    }
}
