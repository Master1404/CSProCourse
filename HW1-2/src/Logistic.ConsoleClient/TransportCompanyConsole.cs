using AutoMapper.Execution;
using Logistic.ConsoleClient.Enum;
using Logistic;
using Logistic.Core;
using Logistic.Model;
using Logistic.Models;
using Logistic.Models.Enum;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;
using Logistic.DAL;

public class TransportCompanyConsole
{
    private VehicleService _vehicleService;
    private WarehouseService _warehouseService;
    private ReportService<Vehicle> _reportService;
    private readonly IReportRepository<Vehicle> _jsonRepository;
    private readonly IReportRepository<Vehicle> _xmlRepository;

    public TransportCompanyConsole(
        VehicleService vehicleService,
        WarehouseService warehouseService,
        ReportService<Vehicle> reportService,
        JsonRepository<Vehicle> jsonRepository,
        XmlRepository<Vehicle> xmlRepository)
    {
        _vehicleService = vehicleService;
        _warehouseService = warehouseService;
        _reportService = reportService;
        _jsonRepository = jsonRepository;
        _xmlRepository = xmlRepository;
    }

    public void AddVehicle()
    {
        Console.WriteLine("Введите данные транспортного средства:");

        Console.WriteLine("Номер:");
        string number = Console.ReadLine();

        Console.WriteLine("Максимальный вес груза в кг:");
        int maxWeightKg = int.Parse(Console.ReadLine());

        Console.WriteLine("Максимальный вес груза в фунтах:");
        double maxWeightPnd = double.Parse(Console.ReadLine());

        Console.WriteLine("Максимальный объем груза:");
        double maxVolume = double.Parse(Console.ReadLine());

        Console.WriteLine("Тип (0 - Авто, 1 - Корабль, 2 - Самолет, 3 - Поезд):");
        int typeInt = int.Parse(Console.ReadLine());
        VehicleType type = (VehicleType)typeInt;
        Vehicle vehicle = new Vehicle(type, maxWeightKg, maxVolume, number, maxWeightPnd);
        
        try
        {
            this._vehicleService.Create(vehicle);
            Console.WriteLine("Транспортное средство добавлено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }   
    }

    public void GetAllVehicles()
    {
        var vehicles = _vehicleService.GetAll();
        if (vehicles!=null && vehicles.Any())
        {
            foreach (var vehicle in vehicles)
            {
                Console.WriteLine(vehicle.GetInformation());
            }
        }
        else
        {
            Console.WriteLine("Транспортных средст нет в базе");
        }
    }

    public void LoadCargoToWarehouse()
    {
        Console.WriteLine("Введите ID склада ");
        int warehouseId = int.Parse(Console.ReadLine());
        Console.Write("Enter cargo code: ");
        string code = Console.ReadLine();

        Console.Write("Enter cargo weight in kg: ");
        int weight = int.Parse(Console.ReadLine());

        Console.Write("Enter cargo volume in cubic meters: ");
        double volume = double.Parse(Console.ReadLine());

        Console.Write("Enter recipient phone number: ");
        string recipientPhoneNumber = Console.ReadLine();

        Console.Write("Enter sender phone number: ");
        string senderPhoneNumber = Console.ReadLine();

        var invoice = new Invoice(recipientPhoneNumber, senderPhoneNumber);
        var cargo = new Cargo(code, weight, volume, invoice);

        var warehouse = _warehouseService.GetById(warehouseId);
        if (warehouse == null)
        {
            Console.WriteLine($"Warehouse with id {warehouseId} does not exist.");
            return;
        }
        _warehouseService.LoadCargo(cargo, warehouseId);
        Console.WriteLine($"Cargo with code {code} has been loaded into the warehouse with id {warehouseId}.");
    }

    public void UnLoadCargoToWarehouse()
    {
        Console.WriteLine("Введите ID складу для разгрузки груза:");
        int warehouseId = int.Parse(Console.ReadLine());
        var warehouse = _warehouseService.GetById(warehouseId);

        if (warehouse == null)
        {
            Console.WriteLine($"Склад с ID {warehouseId} не найдено.");
            return;
        }

        var cargos = warehouse.Cargos.ToList();

        if (cargos.Count == 0)
        {
            Console.WriteLine($"На Складе с ID {warehouseId} нет грузов для разгрузки.");
            return;
        }

        Console.WriteLine($"Выберите груз для разгрузки со склада {warehouse.Cargos} (ID: {warehouse.Id}):");

        for (int i = 0; i < cargos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {cargos[i].Code} (ID: {cargos[i].Id})");
        }

        int choice = int.Parse(Console.ReadLine()) - 1;

        if (choice < 0 || choice >= cargos.Count)
        {
            Console.WriteLine($"Неверный выбор груза для разгрузки. Попробуйте еще раз.");
            return;
        }

        var cargo = cargos[choice];
        _warehouseService.UnloadCargo(warehouseId, cargo.Id);
        Console.WriteLine($"Груз '{cargo.Code}' (ID: {cargo.Id}) был успешно разгружен со склада  (ID: {warehouse.Id}).");
    }
    public void UnLoadCargoToVehicle()
    {
        Console.WriteLine("Введите ID транспортного средства для разгрузки груза:");
        var vehicleId = int.Parse(Console.ReadLine());
        var vehicle = _vehicleService.GetById(vehicleId);

        if (vehicle == null)
        {
            Console.WriteLine($"Транспортное средство с ID {vehicleId} не найдено.");
            return;
        }

        var cargos = vehicle.Cargos.ToList();

        if (cargos.Count == 0)
        {
            Console.WriteLine($"На транспортном средстве с ID {vehicleId} нет грузов для разгрузки.");
            return;
        }

        Console.WriteLine($"Выберите груз для разгрузки на транспортном средстве \t\n {vehicle.Cargos} (ID: \t\n{vehicle.Id}):");

        for (int i = 0; i < cargos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {cargos[i].Code} (ID: {cargos[i].Id})");
        }

        int choice = int.Parse(Console.ReadLine()) - 1;

        if (choice < 0 || choice >= cargos.Count)
        {
            Console.WriteLine($"Неверный выбор груза для разгрузки. Попробуйте еще раз.");
            return;
        }

        var cargo = cargos[choice];
        _vehicleService.UnloadCargo(vehicleId, cargo.Id);
        Console.WriteLine($"Груз '{cargo.Code}' (ID: {cargo.Id}) был успешно разгружен с транспортного средства {vehicle.Type} (ID: {vehicle.Id}).");
    }

    public void LoadCargoToVehicle()
    {
        Console.Write("Введите id транспортного средства: ");
        int vehicleId = int.Parse(Console.ReadLine());
        var vehicles = _vehicleService.GetAll();
        var vehicle = vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (vehicle == null)
        {
            Console.WriteLine($"Транспортное средство с таким ID {vehicleId} не найдено.");
            return;
        }
        if (vehicle.Cargos.Count == 100)
        {
            Console.WriteLine($"Vehicle with ID {vehicleId} is already full.");
            return;
        }
        
        Console.Write("Введите код груза: ");
        string code = Console.ReadLine();

        Console.Write("Введите вес груза в kg: ");
        int weight = int.Parse(Console.ReadLine());

        Console.Write("Введите обем груза в м2: ");
        double volume = double.Parse(Console.ReadLine());

        Console.Write("Введите номер получателя: ");
        string recipientPhoneNumber = Console.ReadLine();

        Console.Write("Введите номер отправителя: ");
        string senderPhoneNumber = Console.ReadLine();

        var invoice = new Invoice( recipientPhoneNumber,  senderPhoneNumber);
        var cargo = new Cargo(code, weight, volume, invoice);
        try
        {
            _vehicleService.LoadCargo(cargo, vehicleId);
            Console.WriteLine($"Груз с кодом {code} загружен на транспортное средство с ID {vehicleId}.");
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Ошибка во время загрузки груза: {ex.Message}");
        }
       
    }

    public void GenerateVehicleReportXml()
    {
        var allVehicles = _vehicleService.GetAll();
        _reportService.CreateReport("vehicles", ReportType.Xml, allVehicles);
    }

    public void GenerateVehicleReportJson()
    {
        var allVehicles = _vehicleService.GetAll();
        _reportService.CreateReport("vehicles", ReportType.Json, allVehicles);
    }

    public void LoadReportConsole()
    {
        try
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", "vehicles.json");
            string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", "vehicles.xml");

            var vehiclesJson = _reportService.LoadReport(jsonFilePath, ReportType.Json);
            var vehiclesXml = _reportService.LoadReport(xmlFilePath, ReportType.Xml);

            bool isJsonData = vehiclesJson != null && vehiclesJson.Count > 0;
            bool isXmlData = vehiclesXml != null && vehiclesXml.Count > 0;

            if (isJsonData)
            {
                Console.WriteLine("JSON Data:");
                Console.WriteLine($"" +
                    $"{"Type"} \t " +
                    $"{"Number"} \t  " +
                    $"{"MaxCargoWeightKg"} \t " +
                    $"{"MaxCargoVolume"} \t" +
                    $"{"MaxCargoWeightPnd"}");

                foreach (var vehicle in vehiclesJson)
                {
                    Console.WriteLine($"" +
                        $"{vehicle.Type} \t" +
                        $"{vehicle.Number} \t \t \t " +
                        $"{vehicle.MaxCargoWeightKg} \t\t\t " +
                        $"{vehicle.MaxCargoVolume} \t \t \t " +
                        $"{vehicle.MaxCargoWeightPnd}");
                }
            }
            else if (!isJsonData)
            {
                Console.WriteLine("No data found in JSON file.");
            }

            if (isXmlData)
            {
                Console.WriteLine("XML Data:");
                Console.WriteLine($"" +
                    $"{"Type"} \t " +
                    $"{"Number"} \t  " +
                    $"{"MaxCargoWeightKg"} \t " +
                    $"{"MaxCargoVolume"} \t" +
                    $" {"MaxCargoWeightPnd"}");

                foreach (var vehicle in vehiclesXml)
                {
                    Console.WriteLine($"" +
                        $"{vehicle.Type} \t" +
                        $" {vehicle.Number} \t \t \t" +
                        $" {vehicle.MaxCargoWeightKg} \t\t\t " +
                        $"{vehicle.MaxCargoVolume} \t \t \t " +
                        $"{vehicle.MaxCargoWeightPnd}");
                }
            }
            else if (!isXmlData)
            {
                Console.WriteLine("No data found in XML file.");
            }

            if (!isJsonData && !isXmlData)
            {
                throw new Exception("No valid vehicles data found in both JSON and XML files.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    public void CreateWarehouse()
    {
        Console.WriteLine("что бы создать склад нажми 1:");
        int id = int.Parse(Console.ReadLine());
        var warehouse = new Warehouse
        {
            Cargos = new List<Cargo>()
        };

        _warehouseService.Create(warehouse);
        Console.WriteLine($"Warehouse  has been created.");
    }

    public void GetAllWarehouses()
    {
        var warehouses = _warehouseService.GetAll();
        if (warehouses!= null && warehouses.Any())
        {
            foreach (var warehous in warehouses)
            {
                Console.WriteLine($"Warehouse with ID: {warehous.Id}");
            }
        }
        else
        {
            Console.WriteLine("Нет созданых складов, сначала попробуйте добавить склад");
        }
    }
}