using AutoMapper;
using Logistic.Core;
using Logistic.DAL;
using Logistic.Model;
using Logistic.Models;
using Logistic.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace Logistic.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : Controller
    {
        private readonly IService<Warehouse, int> _warehouseService;
        private readonly IMapper _mapper;
        public WarehouseController(IService<Warehouse, int> warehouseService, IMapper mapper)
        {
            _warehouseService = warehouseService;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<Warehouse> GetAll()
        {
            return _warehouseService.GetAll();
        }

        [HttpPost]
        public IActionResult Create(WarehouseModel warehouseModel)
        {
            var warehouse = _mapper.Map<Warehouse>(warehouseModel);
            _warehouseService.Create(warehouse);
            return Accepted();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Warehouse warehouse)
        {
            var existingWarehouse = _warehouseService.GetById(id);

            if (existingWarehouse == null)
            {
                return NotFound();
            }

            _warehouseService.Update(existingWarehouse);
            var warehouses = _warehouseService.GetAll();
            return Ok(warehouses);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var existingWarehouse = _warehouseService.GetById(id);

            if (existingWarehouse == null)
            {
                return NotFound();
            }

            _warehouseService.Delete(existingWarehouse.Id);
            var vehicles = _warehouseService.GetAll();
            return Ok(vehicles);
        }

        [HttpPost("warehouse/load")]
        public IActionResult LoadCargo(Cargo cargo, int warehouseId)
        {
            _warehouseService.LoadCargo(cargo, warehouseId);
            return Accepted();
        }

         [HttpPost("warehouse/unload")]
          public IActionResult UnloadCargo(int warehouseId, Guid cargoId)
          {
              var warehouse = _warehouseService.GetById(warehouseId);

              if (warehouse == null)
              {
                  return NotFound();
              }

              var cargo = warehouse.Cargos.FirstOrDefault(c => c.Id == cargoId); 

              if (cargo == null)
              {
                  return NotFound(); 
              }

              warehouse.Cargos.Remove(cargo); 
              _warehouseService.UnloadCargo(warehouseId, cargoId);
              _warehouseService.Update(warehouse);
              var updatedWarehouse = _warehouseService.GetById(warehouseId);
              return Ok(updatedWarehouse);
          }
    }
}
