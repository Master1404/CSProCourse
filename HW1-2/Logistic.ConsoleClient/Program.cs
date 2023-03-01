using Logistic.ConsoleClient;

SuccessScenario();

ExceptionScenario();

void SuccessScenario()
{
    var vehicle = new Vehicle(VehicleType.Car, 24000, 82);
    Console.WriteLine(vehicle.GetInformation());
    var cargos = new List<Cargo>
       {
            new Cargo("Laptop", 50, 20.3),
            new Cargo("Smartphone",10, 1.5),
            new Cargo("TV", 20, 2.9),
            new Cargo("Chair", 20, 1.2),
            new Cargo("Table", 50, 2.7)
       };

    foreach (Cargo cargo in cargos)
    {
        vehicle.LoadCargo(cargo);
    }

    Console.WriteLine(vehicle.GetInformation());
}

void ExceptionScenario()
{
    try
    {
        var vehicle = new Vehicle(VehicleType.Train, 120000, 120);
        Console.WriteLine(vehicle.GetInformation());
        var cargos = new Cargo[]
           {
            new Cargo("rolled metal", 50000, 70),
            new Cargo("sand",40000, 30),
            new Cargo("cement", 50000, 55),
           };

        foreach (Cargo cargo in cargos)
        {
            vehicle.LoadCargo(cargo);
        }

        Console.WriteLine(vehicle.GetInformation());
    }

    catch(Exception ex)
    {
        Console.WriteLine($"Error loading cargo: {ex.Message} ");
    }
}




