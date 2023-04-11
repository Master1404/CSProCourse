using Logistic.Models;
using Logistic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logistic.Model;

namespace Logistic.Core
{
    public class WarehouseService : IService<Warehouse, int>
    {

        private readonly IRepository<Warehouse> _warehouseRepository;
     
        public WarehouseService(IRepository<Warehouse> warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public void Create(Warehouse warehouse)
        {
            if (warehouse == null)
            {
                throw new ArgumentNullException(nameof(warehouse));
            }
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
            var warehouse = _warehouseRepository.GetById(warehouseId);
            if (warehouse == null)
            {
                throw new ArgumentException($"Warehouse with id {warehouseId} not found");
            }
            warehouse.Cargos.Add(cargo);
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
            _warehouseRepository.Update(warehouse);
        }
    }
}
