﻿using Logistic.Models;
using Logistic.Models.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.DAL.Tests
{
    public class JsonRepositoryTest
    {
        private readonly JsonRepository<Vehicle> _jsonRepository;
        private readonly string _filePath = AppDomain.CurrentDomain.BaseDirectory;
        public JsonRepositoryTest()
        {
            _jsonRepository = new JsonRepository<Vehicle>(_filePath);
        }

        [Fact]
        public void Read_WhenValidJson_DeserializeSucessfully()
        {

            // Arrange
            var testPath = Path.Combine("json_serializer_Vehicle_test.json");

            // Act
            var result = _jsonRepository.Read(testPath);

            // Assert
            Assert.Equal(0, result[0].Id); 
            Assert.Equal("ABC", result[0].Number); 
            Assert.Equal(10, result[0].MaxCargoWeightKg);
            Assert.Equal(11, result[0].CurrentCargoWeight);
            Assert.Equal(12, result[0].CurrentCargoVolume);
            Assert.Equal(13, result[0].MaxCargoWeightPnd);
            Assert.Equal(14, result[0].MaxCargoVolume);
            Assert.Equal(VehicleType.Car, result[0].Type);
        }
       
        [Fact]
        public void Create_WhenValidEntity_SerializeSucessfully()
        {
            // Arrange
            var testDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CreateTest");
            Directory.CreateDirectory(testDirectory);
            var testPath = Path.Combine(testDirectory, "json_serializer_Vehicle_test.json");

            // Act
            var entities = new List<Vehicle>();
            entities.Add(new Vehicle
            {
                Id = 0,
                Number = "ABC",
                MaxCargoWeightKg = 10,
                CurrentCargoWeight = 11,
                CurrentCargoVolume = 12,
                MaxCargoWeightPnd = 13,
                MaxCargoVolume = 14,
                Type = VehicleType.Car
            });     

            _jsonRepository.Create(entities, testPath);

            // Assert
            var result = _jsonRepository.Read(testPath);
            Assert.Equal(0, result[0].Id);
            Assert.Equal("ABC", result[0].Number);
            Assert.Equal(10, result[0].MaxCargoWeightKg);
            Assert.Equal(11, result[0].CurrentCargoWeight);
            Assert.Equal(12, result[0].CurrentCargoVolume);
            Assert.Equal(13, result[0].MaxCargoWeightPnd);
            Assert.Equal(14, result[0].MaxCargoVolume);
            Assert.Equal(VehicleType.Car, result[0].Type);
        }
    }
}
