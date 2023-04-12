using AutoFixture;
using AutoFixture.Xunit2;
using Logistic.DAL;
using Logistic.Model;
using Logistic.Models;
using Logistic.Models.Enum;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.Core.Tests
{
    public class WarehouseServiceTests
    {
        private readonly Mock<IRepository<Warehouse>> _mockWarehouseRepository;
        private readonly WarehouseService _warehouseService;
        private readonly Fixture _fixture;
        public WarehouseServiceTests()
        {
            _mockWarehouseRepository = new Mock<IRepository<Warehouse>>();
            _warehouseService = new WarehouseService(_mockWarehouseRepository.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public void Create_WhenValidWarehouse_CallsRepositoryCreate()
        {
            // Arrange
            var warehouse = new Warehouse { Id = 1 };

            // Act
            _warehouseService.Create(warehouse);

            // Assert
            _mockWarehouseRepository.Verify(x => x.Create(warehouse), Times.Once);
        }

        [Fact]
        public void Create_WhenNullWarehouse_ThrowsArgumentNullException()
        {
            // Arrange
            Warehouse warehouse = null;

            // Act
            Action createWarehouse = () => _warehouseService.Create(warehouse);

            // Assert
            Assert.Throws<ArgumentNullException>(createWarehouse);
        }

        [Fact]
        public void GetById_WhenValidId_ReturnsVWarehouse()
        {
            // Arrange
            var warehouseId = 1;
            var expectedWarehouse = new Warehouse { Id = warehouseId };
            _mockWarehouseRepository.Setup(x => x.GetById(warehouseId)).Returns(expectedWarehouse);

            // Act
            var actualVehicle = _warehouseService.GetById(warehouseId);

            // Assert
            Assert.Equal(expectedWarehouse, actualVehicle);
        }
     
        [Fact]
        public void GetAll_ReturnsAllWarehouse()
        {
            // Arrange
            var expectedWarehouse = _fixture.CreateMany<Warehouse>().ToList();
            _mockWarehouseRepository.Setup(x => x.ReadAll()).Returns(expectedWarehouse);
            // Act
            var actualWarehouse = _warehouseService.GetAll();

            // Assert
            Assert.Equal(expectedWarehouse, actualWarehouse);
        }

        [Theory]
        [AutoData]
        public void GetAll_ReturnsAllWarehouse1(List<Warehouse> expectedWarehouse)
        {
            // Arrange
            _mockWarehouseRepository.Setup(x => x.ReadAll()).Returns(expectedWarehouse);
            var warehouseService = new WarehouseService(_mockWarehouseRepository.Object);

            // Act
            var actualWarehouse = warehouseService.GetAll();

            // Assert
            Assert.Equal(expectedWarehouse, actualWarehouse);
        }

        [Fact]
        public void Delete_WhenValidId_DeletesVehicle()
        {
            // Arrange
            var vehicleId = 1;

            // Act
            _warehouseService.Delete(vehicleId);

            // Assert
            _mockWarehouseRepository.Verify(x => x.DeleteById(vehicleId), Times.Once);
        }

        [Fact]
        public void LoadCargo_WhenWarehouseNotFound_ShouldThrowException()
        {
            // Arrange
            var cargo = _fixture.Create<Cargo>();
            var warehouseId = 10;
            _mockWarehouseRepository.Setup(x => x.GetById(warehouseId)).Returns((Warehouse)null);

            // Act
            Action act = () => _warehouseService.LoadCargo(cargo, warehouseId);

            // Assert
            var ex = Assert.Throws<ArgumentException>(act);
            Assert.Equal($"Warehouse with id {warehouseId} not found", ex.Message);
        }

        [Fact]
        public void LoadCargo_WhenWarehouseExists_ShouldAddCargoToWarehouse()
        {
            // Arrange
            var cargo = _fixture.Create<Cargo>();
            var warehouseId = _fixture.Create<int>();
            var warehouse = new Warehouse { Id = warehouseId, Cargos = new List<Cargo>() };
            _mockWarehouseRepository.Setup(x => x.GetById(warehouseId)).Returns(warehouse);

            // Act
            _warehouseService.LoadCargo(cargo, warehouseId);

            // Assert
            _mockWarehouseRepository.Verify(x => x.Update(It.Is<Warehouse>(w => w.Id == warehouseId && w.Cargos.Contains(cargo))), Times.Once);
        }

        [Fact]
        public void UnloadCargo_WhenWarehouseNotFound_ShouldThrowException()
        {
            // Arrange
            var warehouseId = _fixture.Create<int>();
            var cargoId = _fixture.Create<Guid>();
            _mockWarehouseRepository.Setup(x => x.GetById(warehouseId)).Returns((Warehouse)null);

            // Act
            Action act = () => _warehouseService.UnloadCargo(warehouseId, cargoId);

            // Assert
            var ex = Assert.Throws<ArgumentException>(act);
            Assert.Equal($"Warehouse with id {warehouseId} not found", ex.Message);
        }

        [Fact]
        public void UnloadCargo_WhenCargoNotFoundInWarehouse_ShouldThrowException()
        {
            // Arrange
            var warehouseId = _fixture.Create<int>();
            var cargoId = _fixture.Create<Guid>();
            var warehouse = new Warehouse { Id = warehouseId, Cargos = new List<Cargo>() };
            _mockWarehouseRepository.Setup(x => x.GetById(warehouseId)).Returns(warehouse);

            // Act
            Action act = () => _warehouseService.UnloadCargo(warehouseId, cargoId);

            // Assert
            var ex = Assert.Throws<ArgumentException>(act);
            Assert.Equal($"Cargo with id {cargoId} not found in warehouse with id {warehouseId}", ex.Message);
        }

        [Fact]
        public void UnloadCargo_WhenWarehouseAndCargoExist_ShouldRemoveCargoFromWarehouse()
        {
            // Arrange
            var warehouseId = _fixture.Create<int>();
            var cargoId = _fixture.Create<Guid>();
            var cargo = new Cargo { Id = cargoId };
            var warehouse = new Warehouse { Id = warehouseId, Cargos = new List<Cargo> { cargo } };
            _mockWarehouseRepository.Setup(r => r.GetById(warehouseId)).Returns(warehouse);

            // Act
            _warehouseService.UnloadCargo(warehouseId, cargoId);

            // Assert
            _mockWarehouseRepository.Verify(r => r.Update(It.Is<Warehouse>(w => w.Id == warehouseId && !w.Cargos.Contains(cargo))), Times.Once);
        }
    }
}
