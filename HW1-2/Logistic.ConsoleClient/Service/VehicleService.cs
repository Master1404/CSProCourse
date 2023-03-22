using Logistic.ConsoleClient.Model;
using Logistic.ConsoleClient.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient.Service
{
    public class VehicleService
    {
        private readonly InMemoryRepository<Vehicle, int> _vehicleRepository;
        private readonly InMemoryRepository<Warehouse, int> _warehouseRepository;
        private Vehicle _vehicle;

        public VehicleService(
            InMemoryRepository<Vehicle, int> vehicleRepository,
            InMemoryRepository<Warehouse, int> warehouseRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _warehouseRepository = warehouseRepository ?? throw new ArgumentNullException(nameof(warehouseRepository));
        }
        public int CurrentWeight
        {
            get
            {
                return _vehicle.Cargos?.Sum(c => c.Weight) ?? 0;
            }
        }

        public double CurrentVolume
        {
            get
            {
                return _vehicle.Cargos?.Sum(c => c.Volume) ?? 0;
            }
        }

        public void Create(Vehicle vehicle)
        {
            _vehicleRepository.Create(vehicle);
        }
        public Vehicle GetById(int vehicleId)
        {
            return _vehicleRepository.GetById(vehicleId);
        }
        public List<Vehicle> GetAll()
        {
            return _vehicleRepository.ReadAll().ToList();
        }
        public void Delete(int id)
        {
            _vehicleRepository.DeleteById(id);
        }

        public void LoadCargo(Cargo cargo, int vehicleId)
        {
            var vehicle = GetById(vehicleId);

            
            if (vehicle.Cargos == null)
            {
                vehicle.Cargos = new List<Cargo>();
            }

            if (cargo != null)
            {
                int weight = 0;
                double volume = 0;
                foreach (var item in vehicle.Cargos)
                {
                    volume += item.Volume;
                    weight += item.Weight;
                }

                weight += cargo.Weight;
                if (weight > vehicle.MaxCargoWeightKg)
                {
                    throw new Exception($"Current Weight = {CurrentWeight} kg," +
                        $" imposible to add this cargo({cargo.Weight} kg), because maxWeigth {vehicle.MaxCargoWeightKg} kg \n");
                }

                volume += cargo.Volume;
                if (volume > vehicle.MaxCargoVolume)
                {
                    throw new Exception($"Current Volume {CurrentVolume} cubic meters," +
                        $" imposible to add this cargo({cargo.Volume} cubic meters),because maxWeigth {vehicle.MaxCargoVolume} cubic meters \n");
                }

                if (weight < vehicle.MaxCargoWeightKg && volume < vehicle.MaxCargoVolume)
                {
                    vehicle.Cargos.Add(cargo);
                }
            }
        }

        public void UnloadCargo(int vehicleId, Guid cargoId)
        {
            var vehicle = _vehicleRepository.GetById(vehicleId);

            if (vehicle == null)
            {
                throw new ArgumentException($"Vehicle with id {vehicleId} not found");
            }

            var cargo = vehicle.Cargos.FirstOrDefault(c => c.Id == cargoId);

            if (cargo == null)
            {
                throw new ArgumentException($"Cargo with id {cargoId} not found in vehicle with id {vehicleId}");
            }

            vehicle.Cargos.Remove(cargo);

            if (vehicle.Cargos.Count == 0)
            {
                // Notify warehouse that vehicle is empty
                _warehouseRepository.ReadAll().FirstOrDefault();
            }
        }
    }
}