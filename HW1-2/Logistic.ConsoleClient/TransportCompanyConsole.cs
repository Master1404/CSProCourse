using Logistic.ConsoleClient;
using Logistic.ConsoleClient.Model;
using Logistic.ConsoleClient.Repository;
using Logistic.ConsoleClient.Service;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;

public class TransportCompanyConsole
{
    private VehicleService _vehicleService;
    private WarehouseService _warehouseService;
    private ReportService<Vehicle> _reportService;

    public TransportCompanyConsole(VehicleService vehicleService, WarehouseService warehouseService, ReportService<Vehicle> reportService)
    {
        this._vehicleService = vehicleService;
        this._warehouseService = warehouseService;
        this._reportService = reportService;
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

        Console.WriteLine("Тип (0 - Авто, 1 - Корабель, 2 - Літак, 3 - Потяг):");
        int typeInt = int.Parse(Console.ReadLine());
        VehicleType type = (VehicleType)typeInt;
        Vehicle vehicle = new Vehicle()
        {
            Number = number,
            MaxCargoWeightKg = maxWeightKg,
            MaxCargoWeightPnd = maxWeightPnd,
            MaxCargoVolume = maxVolume,
            Type = type
        };

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
        foreach (var vehicle in vehicles) 
        {
            Console.WriteLine($"Id: {vehicle.Id}, Number: {vehicle.Number}, Type: {vehicle.Type}");
        }
    }

    public void LoadCargoToWarehouse()
    {
        Console.WriteLine("Введіть ID склада ");
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
        var warehouse = _vehicleService.GetById(warehouseId);

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

        Console.WriteLine($"Выберите груз для разгрузки на транспортном средстве {warehouse.Cargos} (ID: {warehouse.Id}):");

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
        Console.WriteLine($"Груз '{cargo.Code}' (ID: {cargo.Id}) был успешно разгружен со склада {warehouse.Type} (ID: {warehouse.Id}).");
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
            Console.WriteLine($"Транспортний засіб з таким {vehicleId} не знайдено.");
            return;
        }
        if (vehicle.Cargos.Count == 100)
        {
            Console.WriteLine($"Vehicle with ID {vehicleId} is already full.");
            return;
        }
        
        Console.Write("Введіть код груза: ");
        string code = Console.ReadLine();

        Console.Write("Введіть вагу груза в kg: ");
        int weight = int.Parse(Console.ReadLine());

        Console.Write("Введіть обем груза в м2: ");
        double volume = double.Parse(Console.ReadLine());

        Console.Write("Введіть номер отримувача: ");
        string recipientPhoneNumber = Console.ReadLine();

        Console.Write("Введіть номер відправника: ");
        string senderPhoneNumber = Console.ReadLine();

        var invoice = new Invoice( recipientPhoneNumber,  senderPhoneNumber);
        var cargo = new Cargo(code, weight, volume, invoice);

        vehicle.Cargos.Add(cargo);
        Console.WriteLine($"Вантаз з кодом {code} завантажений в транспортний засіб з ID {vehicleId}.");
    }

    public void GenerateVehicleReportXml()
    {
        var allVehicles = _vehicleService.GetAll();
        var reportService = new ReportService<Vehicle>(); ;
        reportService.CreateReport("vehicles", ReportType.Xml, allVehicles);
    }

    public void GenerateVehicleReportJson()
    {
        var allVehicles = _vehicleService.GetAll();
        var reportService = new ReportService<Vehicle>();
        reportService.CreateReport("vehicles", ReportType.Json, allVehicles);
    }

    public void LoadReportConsole()
    {
        var vehiclesJson = _reportService.LoadReport(@"D:\C#\C-Pro_\CSProCourse\HW1-2\Logistic.ConsoleClient\bin\Debug\net6.0\vehicles.json");
        var vehiclesXml = _reportService.LoadReport(@"D:\C#\C-Pro_\CSProCourse\HW1-2\Logistic.ConsoleClient\bin\Debug\net6.0\vehicles.xml");
        Console.WriteLine($"{"Type"} \t  {"Number"} \t  {"MaxCargoWeightKg"} \t  {"MaxCargoVolume"} \t  {"MaxCargoWeightPnd"}");
        List<Vehicle> vehicles;
        if (vehiclesJson != null)
        {
            vehicles = vehiclesJson;
        }
        else if (vehiclesXml != null)
        {
            vehicles = vehiclesXml;
        }
        else
        {
            throw new Exception("No valid vehicles data found.");
        }

        foreach (var vehicle in vehicles)
        {
            Console.WriteLine($"{vehicle.Type} \t  {vehicle.Number} \t \t \t{vehicle.MaxCargoWeightKg} \t\t\t {vehicle.CurrentVolume} \t \t \t {vehicle.MaxCargoWeightPnd}");
        }
    }

    public void CreateWarehouse()
    {
        Console.WriteLine("Enter the ID of the warehouse:");
        int id = int.Parse(Console.ReadLine());

        var warehouse = new Warehouse
        {
            Id = id,
            Cargos = new List<Cargo>()
        };

        _warehouseService.Create(warehouse);
        Console.WriteLine($"Warehouse with ID {id} has been created.");
    }

    internal void GetAllWarehouses()
    {
        var warehouses = _warehouseService.GetAll();

        foreach (var warehous in warehouses)
        {
            Console.WriteLine($"Warehouse with ID: {warehous.Id}");
        }
    }
}