﻿using Logistic.ConsoleClient.Model;
using Logistic.ConsoleClient.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient.Service
{
    public class WarehouseService
    {
        private readonly InMemoryRepository<Vehicle, int> _vehicleRepository;
        private readonly InMemoryRepository<Cargo, Guid> _cargoRepository;
        private readonly InMemoryRepository<Warehouse, int> _warehouseRepository;
        private readonly InMemoryRepository<Invoice, Guid> _invoiceRepository;
        private readonly WarehouseService _warehouseService;
        

        public WarehouseService(
            InMemoryRepository<Vehicle, int> vehicleRepository,
            InMemoryRepository<Cargo, Guid> cargoRepository,
            InMemoryRepository<Invoice, Guid> invoiceRepository, 
            InMemoryRepository<Warehouse, int> warehouseRepository,
             WarehouseService warehouseService)
        {
            _vehicleRepository = vehicleRepository;
            _cargoRepository = cargoRepository;
            _invoiceRepository = invoiceRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseService = warehouseService;
        }

        public WarehouseService(
            InMemoryRepository<Vehicle, int> vehicleRepository,
            InMemoryRepository<Cargo, Guid> cargoRepository,
            InMemoryRepository<Invoice, Guid> invoiceRepository,
            InMemoryRepository<Warehouse, int> warehouseRepository)
        {
            _vehicleRepository = vehicleRepository;
            _cargoRepository = cargoRepository;
            _invoiceRepository = invoiceRepository;
            _warehouseRepository = warehouseRepository;
        }

        public void Create(Warehouse warehouse)
        {
            _warehouseRepository.Create(warehouse);
        }
        public Warehouse GetById(int warehouseId)
        {
            return _warehouseRepository.GetById(warehouseId);
        }
        public List<Warehouse> GetAll()
        {
            return _warehouseRepository.ReadAll().ToList();
        }
        public void Delete(int id)
        {
            _warehouseRepository.DeleteById(id);
        }
        public void LoadCargo(Cargo cargo, int warehouseId)
        {
            // поиск Vehicle по Id
            //var warehouse = GetById(warehouseId);
            var warehouse = _warehouseRepository.GetById(warehouseId);
            if (warehouse == null)
            {
                throw new ArgumentException($"Warehouse with id {warehouseId} not found");
            }

            // загрузка груза на склад
            warehouse.Cargos.Add(cargo);

            // обновление данных о складе
            _warehouseRepository.Update(warehouse);
        }
        

        public void UnloadCargo(int warehouseId, Guid cargoId) 
        {
            var warehouse = _warehouseRepository.GetById(warehouseId);

            if (warehouse == null)
            {
                throw new ArgumentException($"Warehouse with id {warehouseId} not found");
            }

            var cargo = warehouse.Cargos.FirstOrDefault(c => c.Id == cargoId);

            if (cargo == null)
            {
                throw new ArgumentException($"Cargo with id {cargoId} not found in warehouse with id {warehouseId}");
            }

            warehouse.Cargos.Remove(cargo);

        }
    }
}
