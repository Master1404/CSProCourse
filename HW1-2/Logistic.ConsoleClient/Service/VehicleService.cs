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
        private readonly InMemoryRepository<Cargo, Guid> _cargoRepository;
        private readonly InMemoryRepository<Warehouse, int> _warehouseRepository;
        private readonly InMemoryRepository<Invoice, Guid> _invoiceRepository;

        public VehicleService(
            InMemoryRepository<Vehicle, int> vehicleRepository,
            InMemoryRepository<Cargo, Guid> cargoRepository,
            InMemoryRepository<Invoice, Guid> invoiceRepository,
            InMemoryRepository<Warehouse, int> warehouseRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _cargoRepository = cargoRepository ?? throw new ArgumentNullException(nameof(cargoRepository));
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
            _warehouseRepository = warehouseRepository ?? throw new ArgumentNullException(nameof(warehouseRepository));
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
            // поиск Vehicle по Id
            var vehicle = GetById(vehicleId);

            // проверка на заполненность грузовика
            if (vehicle.IsFull)
            {
                throw new Exception("Транспортное средство уже заполнено");
            }

            // загрузка груза в грузовик
            vehicle.Cargos.Add(cargo);

            // обновление данных о грузовике
            _vehicleRepository.Update(vehicle);
        }

        public void UnloadCargo( int vehicleId, Guid cargoId)
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

            if (vehicle.IsFull)
            {
                // Notify warehouse that vehicle is full and ready to be dispatched
                _warehouseRepository.ReadAll().FirstOrDefault();
                
                // Notify invoice repository that new invoice was created
                var invoice = new Invoice
                {
                    Id = Guid.NewGuid(),
                    RecipientAddress = "Адрес получателя",
                    SenderAddress = "Адрес отправителя",
                    RecipientPhoneNumber = "+380123456789",
                    SenderPhoneNumber = "+380987654321"
                };

                _invoiceRepository.Create(invoice);
            }
        }
    }
}