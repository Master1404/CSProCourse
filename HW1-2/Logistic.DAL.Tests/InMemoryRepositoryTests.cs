using AutoFixture;
using Logistic.Models;
using Logistic.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace Logistic.DAL.Tests
{
    public class InMemoryRepositoryTests
    {
        private readonly InMemoryRepository<Vehicle> _repository;
        private readonly Fixture _fixture;

        public InMemoryRepositoryTests()
        {
            _fixture = new Fixture();
            _repository = new InMemoryRepository<Vehicle>(v => v.Id);
        }

        [Fact]
        public void Create_AddsEntityToRepository()
        {
            // Arrange
            var entity = _fixture.Create<Vehicle>();
            entity.Id = 1;

            // Act
            _repository.Create(entity);

            // Assert
            _repository.ReadAll().Should().BeEquivalentTo(new List<Vehicle> { entity });
        }
        [Fact]
        public void GetById_ReturnsEntityById()
        {
            // Arrange
            var expected = new Vehicle { Id = 1, Type = VehicleType.Car, Number = "ase234", MaxCargoVolume = 10, MaxCargoWeightKg = 1000 };
               
            _repository.Create(expected);
            
            // Act
            var result = _repository.GetById(expected.Id);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetById_ReturnsNull_WhenEntityNotFound()
        {
            // Arrange
            var entityId = _fixture.Create<int>();

            // Act
            var result = _repository.GetById(entityId);

            // Assert
            result.Should().BeNull();
        }
        [Fact]
        public void Update_UpdatesEntityInRepository()
        {
            // Arrange
            var entity = new Vehicle
            {
                Id = 1,
                Number = "Honda123",
                MaxCargoVolume = 15,
                MaxCargoWeightKg = 2000
            };
            _repository.Create(entity);
            var updatedEntity = new Vehicle
            {
                Id = 1,
                Number = "Honda123",
                MaxCargoVolume = 20,
                MaxCargoWeightKg = 4000
            };

            // Act
            var result = _repository.Update(updatedEntity);

            // Assert
            result.Should().BeTrue();
            _repository.GetById(entity.Id).Should().BeEquivalentTo(updatedEntity);
        }

        [Fact]
        public void DeleteById_RemovesEntityFromRepository()
        {
            // Arrange
            var entity = new Vehicle
            {
                Id = 1,
                Number = "Honda123",
                MaxCargoVolume = 15,
                MaxCargoWeightKg = 2000
            };
            _repository.Create(entity);

            // Act
            var result = _repository.DeleteById(entity.Id);

            // Assert
            result.Should().BeTrue();
            _repository.ReadAll().Should().NotContain(entity);
        }

        [Fact]
        public void DeleteById_ReturnsFalse_WhenEntityNotFound()
        {
            // Arrange
            var entityId = _fixture.Create<int>();

            // Act
            var result = _repository.DeleteById(entityId);

            // Assert
            result.Should().BeFalse();
        }
    }
}
