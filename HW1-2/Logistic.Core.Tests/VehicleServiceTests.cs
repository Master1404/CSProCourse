using Logistic.DAL;
using Logistic.Model;
using Logistic.Models;
using Moq;
using Logistic.Models.Enum;
using AutoFixture;

namespace Logistic.Core.Tests
{
    public class VehicleServiceTests
    {
        private readonly Mock<IRepository<Vehicle>> _mockVehicleRepository;
        private readonly VehicleService _vehicleService;
        private readonly Fixture _fixture;
        public VehicleServiceTests()
        {
            _mockVehicleRepository = new Mock<IRepository<Vehicle>>();
            _vehicleService = new VehicleService(_mockVehicleRepository.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public void Create_WithValidVehicle_CallsRepositoryCreate()
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10 };

            // Act
            _vehicleService.Create(vehicle);

            // Assert
            _mockVehicleRepository.Verify(x => x.Create(vehicle), Times.Once);
        }

        [Fact]
        public void Create_WithNullVehicle_ThrowsArgumentNullException()
        {
            // Arrange
            Vehicle vehicle = null;

            // Act
            Action createVehicle = () => _vehicleService.Create(vehicle);

            // Assert
            Assert.Throws<ArgumentNullException>(createVehicle);
        }
        [Fact]
        public void GetById_WithValidId_ReturnsVehicle()
        {
            // Arrange
            var vehicleId = 1;
            var expectedVehicle = new Vehicle { Id = vehicleId, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10 };
            _mockVehicleRepository.Setup(x => x.GetById(vehicleId)).Returns(expectedVehicle);

            // Act
            var actualVehicle = _vehicleService.GetById(vehicleId);

            // Assert
            Assert.Equal(expectedVehicle, actualVehicle);
        }
        [Fact]
        public void GetAll_ReturnsListOfVehicles()
        {
            // Arrange
            var expectedVehicles = new List<Vehicle>
            {
                 new Vehicle { Id = 1, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10 },
                 new Vehicle { Id = 2, Type = VehicleType.Ship, MaxCargoWeightKg = 2000, MaxCargoVolume = 20 },
                 new Vehicle { Id = 3, Type = VehicleType.Plane, MaxCargoWeightKg = 3000, MaxCargoVolume = 30 }
            };
            _mockVehicleRepository.Setup(x => x.ReadAll()).Returns(expectedVehicles);

            // Act
            var actualVehicles = _vehicleService.GetAll();

            // Assert
            Assert.Equal(expectedVehicles, actualVehicles);
        }
        [Fact]
        public void GetAll_ReturnsAllVehicles()
        {
            // Arrange
            var expectedVehicles = _fixture.CreateMany<Vehicle>().ToList();
            _mockVehicleRepository.Setup(x => x.ReadAll()).Returns(expectedVehicles);

            // Act
            var actualVehicles = _vehicleService.GetAll();

            // Assert
            Assert.Equal(expectedVehicles, actualVehicles);
        }
        [Fact]
        public void Delete_WithValidId_DeletesVehicle()
        {
            // Arrange
            var vehicleId = 1;

            // Act
            _vehicleService.Delete(vehicleId);

            // Assert
            _mockVehicleRepository.Verify(x => x.DeleteById(vehicleId), Times.Once);
        }
        [Fact]
        public void LoadCargo_WithValidCargoAndVehicle_AddsCargoToVehicle()
        {
            // Arrange
            var vehicleId = 1;
            var cargo = new Cargo { Id = new Guid(), Weight = 500, Volume = 5 };
            var vehicle = new Vehicle { Id = vehicleId, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10 };
            _mockVehicleRepository.Setup(x => x.GetById(vehicleId)).Returns(vehicle);

            // Act
            _vehicleService.LoadCargo(cargo, vehicleId);

            // Assert
            Assert.Contains(cargo, vehicle.Cargos);
            _mockVehicleRepository.Verify(x => x.Update(vehicle), Times.Once);
        }

        [Fact]
        public void LoadCargo_WithCargoExceedingWeightLimit_ThrowsException()
        {
            // Arrange
            var vehicleId = 1;
            var cargo = new Cargo { Id = new Guid(), Weight = 1001, Volume = 5 };
            var vehicle = new Vehicle { Id = vehicleId, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10 };
            _mockVehicleRepository.Setup(x => x.GetById(vehicleId)).Returns(vehicle);

            // Act & Assert
            Assert.Throws<Exception>(() => _vehicleService.LoadCargo(cargo, vehicleId));
            Assert.DoesNotContain(cargo, vehicle.Cargos);
        }

        [Fact]
        public void LoadCargo_WithCargoExceedingVolumeLimit_ThrowsException()
        {
            // Arrange
            var vehicleId = 1;
            var cargo = new Cargo { Id = new Guid(), Weight = 500, Volume = 11 };
            var vehicle = new Vehicle { Id = vehicleId, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10 };
            _mockVehicleRepository.Setup(x => x.GetById(vehicleId)).Returns(vehicle);

            // Act & Assert
            Assert.Throws<Exception>(() => _vehicleService.LoadCargo(cargo, vehicleId));
            Assert.DoesNotContain(cargo, vehicle.Cargos);
        }
        [Fact]
        public void UnloadCargo_WithValidCargoAndVehicle_RemovesCargoFromVehicle()
        {
            // Arrange
            var vehicleId = 1;
            var cargoId = Guid.NewGuid();
            var cargo = new Cargo { Id = cargoId, Weight = 500, Volume = 5 };
            var vehicle = new Vehicle { Id = vehicleId, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10, Cargos = new List<Cargo> { cargo } };
            _mockVehicleRepository.Setup(x => x.GetById(vehicleId)).Returns(vehicle);

            // Act
            _vehicleService.UnloadCargo(vehicleId, cargoId);

            // Assert
            Assert.DoesNotContain(cargo, vehicle.Cargos);
        }

        [Fact]
        public void UnloadCargo_WithInvalidVehicleId_ThrowsArgumentException()
        {
            // Arrange
            var vehicleId = 1;
            var cargoId = Guid.NewGuid();
            var vehicle = new Vehicle { Id = 2, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10, Cargos = new List<Cargo> { new Cargo { Id = cargoId, Weight = 500, Volume = 5 } } };
            _mockVehicleRepository.Setup(x => x.GetById(vehicleId)).Returns((Vehicle)null);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _vehicleService.UnloadCargo(vehicleId, cargoId));
            Assert.Equal($"Vehicle with id {vehicleId} not found", ex.Message);
        }

        [Fact]
        public void UnloadCargo_WithInvalidCargoId_ThrowsArgumentException()
        {
            // Arrange
            var vehicleId = 1;
            var cargoId = Guid.NewGuid();
            var vehicle = new Vehicle { Id = vehicleId, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10, Cargos = new List<Cargo> { new Cargo { Id = Guid.NewGuid(), Weight = 500, Volume = 5 } } };
            _mockVehicleRepository.Setup(x => x.GetById(vehicleId)).Returns(vehicle);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _vehicleService.UnloadCargo(vehicleId, cargoId));
            Assert.Equal($"Cargo with id {cargoId} not found in vehicle with id {vehicleId}", ex.Message);
        }

        [Fact]
        public void UnloadCargo_WithLastCargoInVehicle_RemovesCargoAndUpdatesVehicle()
        {
            // Arrange
            var vehicleId = 1;
            var cargoId = Guid.NewGuid();
            var cargo = new Cargo { Id = cargoId, Weight = 500, Volume = 5 };
            var vehicle = new Vehicle { Id = vehicleId, Type = VehicleType.Car, MaxCargoWeightKg = 1000, MaxCargoVolume = 10, Cargos = new List<Cargo> { cargo } };
            _mockVehicleRepository.Setup(x => x.GetById(vehicleId)).Returns(vehicle);
            _mockVehicleRepository.Setup(x => x.ReadAll()).Returns(new List<Vehicle> { vehicle }.AsQueryable());

            // Act
            _vehicleService.UnloadCargo(vehicleId, cargoId);

            // Assert
            Assert.DoesNotContain(cargo, vehicle.Cargos);
            _mockVehicleRepository.Verify(x => x.Update(vehicle), Times.Once);
        }
    }
}
