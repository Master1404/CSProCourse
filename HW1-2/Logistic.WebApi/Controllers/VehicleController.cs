using AutoMapper;
using Logistic.Core;
using Logistic.Model;
using Logistic.Models;
using Logistic.WebApi.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;

namespace Logistic.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IService<Vehicle, int> _vehicleService;
        private readonly IMapper _mapper;

        public VehicleController(IService<Vehicle, int> vehicleService, IMapper mapper)
        {
            _mapper = mapper;
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public IEnumerable<Vehicle> GetAll()
        {
            return _vehicleService.GetAll();
        }

        [HttpPost]
        public IActionResult Create(VehicleModel vehicleModel)
        {
            var vehicle = _mapper.Map<Vehicle>(vehicleModel);
             _vehicleService.Create(vehicle);
            return Accepted(); 
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Vehicle vehicle)
        {
            var existingVehicle = _vehicleService.GetById(id);

            if (existingVehicle == null)
            {
                return NotFound();
            }

            existingVehicle.Number = vehicle.Number;
            existingVehicle.Type = vehicle.Type;
            existingVehicle.MaxCargoWeightKg = vehicle.MaxCargoWeightKg;
            existingVehicle.MaxCargoVolume = vehicle.MaxCargoVolume;
            _vehicleService.Update(existingVehicle);
            var vehicles = _vehicleService.GetAll();
            return Ok(vehicles);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var existingVehicle = _vehicleService.GetById(id);

            if (existingVehicle == null)
            {
                return NotFound();
            }

            _vehicleService.Delete(existingVehicle.Id);
            var vehicles = _vehicleService.GetAll();
            return Ok(vehicles);
        }

        [HttpPost("load")]
        public IActionResult LoadCargo(Cargo cargo, int vehicleId)
        {
            _vehicleService.LoadCargo(cargo, vehicleId);
             return Accepted();
        }

        [HttpPost("/unload")]
        public IActionResult UnloadCargo(int vehicleId, Guid cargoId)
        {
            var vehicle = _vehicleService.GetById(vehicleId);

            if (vehicle == null)
            {
                return NotFound();
            }

            var cargo = vehicle.Cargos.FirstOrDefault(c => c.Id == cargoId); 

            if (cargo == null)
            {
                return NotFound(); 
            }

            vehicle.Cargos.Remove(cargo); 
            _vehicleService.UnloadCargo(vehicleId, cargoId);
            _vehicleService.Update(vehicle);
            var updatedVehicle = _vehicleService.GetById(vehicleId);
            return Ok(updatedVehicle);
        }
    }
}