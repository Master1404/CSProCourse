using Logistic.Core;
using Logistic.Model;
using Logistic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logistic.DAL;
using System.Security.Cryptography;

namespace Logistic.DAL
{
    public class VehicleService: IService<Vehicle, int>
    {
        private readonly IRepository<Vehicle>  _vehicleRepository;

        public VehicleService(IRepository<Vehicle> vehicleRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        public void Create(Vehicle vehicle)
        {
            if (vehicle == null)
            {
                throw new ArgumentNullException(nameof(vehicle));
            }

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
                vehicle.CurrentCargoWeight = vehicle.Cargos.Sum(c => c.Weight) + cargo.Weight;
                if (vehicle.CurrentCargoWeight > vehicle.MaxCargoWeightKg)
                {
                    throw new Exception($"Current Weight = {vehicle.CurrentCargoWeight} kg," +
                        $" impossible to add this cargo({cargo.Weight} kg), because max weight {vehicle.MaxCargoWeightKg} kg \n");
                }

                vehicle.CurrentCargoVolume = vehicle.Cargos.Sum(c => c.Volume) + cargo.Volume;
                if (vehicle.CurrentCargoVolume > vehicle.MaxCargoVolume)
                {
                    throw new Exception($"Current Volume {vehicle.CurrentCargoVolume} cubic meters," +
                        $" impossible to add this cargo({cargo.Volume} cubic meters),because max volume {vehicle.MaxCargoVolume} cubic meters \n");
                }

                if (vehicle.CurrentCargoWeight <= vehicle.MaxCargoWeightKg)
                {
                     vehicle.Cargos.Add(cargo);
                    _vehicleRepository.Update(vehicle);
                }
                else
                {
                    throw new Exception($"Impossible to add this cargo({cargo.Weight} kg), because it exceeds the maximum weight capacity of the vehicle\n");
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
                _vehicleRepository.ReadAll().FirstOrDefault();
                _vehicleRepository.Update(vehicle);
            }
        }
    }
}