﻿using AutoMapper;
using Logistic.ConsoleClient;
using Logistic.ConsoleClient.Model;
using Logistic.ConsoleClient.Repository;
using Logistic.ConsoleClient.Service;
using System.Net.WebSockets;
var vehicleInMemoryRepository = new VehicleInInMemoryRepository<Vehicle, int>(v => v.Id);
var warehouseInMemoryRepository = new WarehouseInInMemoryRepository<Warehouse, int>(w => w.Id);
var vehicleService = new VehicleService(vehicleInMemoryRepository, warehouseInMemoryRepository);
var warehouseService = new WarehouseService(vehicleInMemoryRepository, warehouseInMemoryRepository);
var reportService = new ReportService<Vehicle>();
var consoleApp = new TransportCompanyConsole(vehicleService, warehouseService, reportService);

while (true)
{
    Console.WriteLine("Выберите действие:");
    Console.WriteLine("1. Работа с транспортными средствами");
    Console.WriteLine("2. Работа с грузами и складами");
    Console.WriteLine("3. Генерация отчетов");
    Console.WriteLine("4. Выход");

    string choice = Console.ReadLine();
    switch (choice)
    {
        case "1":
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Добавить новое транспортное средство");
            Console.WriteLine("2. Получить список всех транспортных средств");
            Console.WriteLine("3. Загрузить груз на транспортное средство");
            Console.WriteLine("4. Загрузить груз на склад");
            Console.WriteLine("5. Розгрузить груз с транспортного средства");
            Console.WriteLine("6. Розгрузить груз со склада");
            Console.WriteLine("7. Вернуться в главное меню");

            string vehicleChoice = Console.ReadLine();
            switch (vehicleChoice)
            {
                case "1":
                    consoleApp.AddVehicle();
                    break;
                case "2":
                    consoleApp.GetAllVehicles();
                    break;
                case "3":
                      consoleApp.LoadCargoToVehicle();
                    break;
                case "4":
                     consoleApp.LoadCargoToWarehouse();
                    break;
                case "5":
                        consoleApp.UnLoadCargoToVehicle();
                    break;
                case "6":
                    consoleApp.UnLoadCargoToWarehouse();
                    break;
                case "7":
                    break;
                default:
                    Console.WriteLine("Неверный выбор, попробуйте еще раз.");
                    break;
            }
            break;
              case "2":
                 Console.WriteLine("Выберите действие:");
                 Console.WriteLine("1. Создать новый склад");
                 Console.WriteLine("2. Получить список всех складов");
                 Console.WriteLine("3. Загрузить груз на склад");
                 Console.WriteLine("4. Розгрузить груз со склада");
                 Console.WriteLine("5. Вернуться в главное меню");

                 string cargoChoice = Console.ReadLine();
                 switch (cargoChoice)
                 {
                     case "1":
                         consoleApp.CreateWarehouse();
                         break;
                     
                     case "2":
                         consoleApp.GetAllWarehouses();
                         break;
                     case "3":
                            consoleApp.LoadCargoToWarehouse();
                         break;
                    case "4":
                         consoleApp.UnLoadCargoToWarehouse();
                     break;
                    case "5":
                         break;
                     default:
                         Console.WriteLine("Неверный выбор, попробуйте еще раз.");
                         break;
                 }
            break; 
        case "3":
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Создает Xml Отчет");
            Console.WriteLine("2. Создает Json Отчет");
            Console.WriteLine("3. Создает Отчет в консоле");// нужно переделать, отдельно жсон и хмл
            Console.WriteLine("4. Вернуться в главное меню");

            string reportChoice = Console.ReadLine();
            switch (reportChoice)
            {
                case "1":
                     consoleApp.GenerateVehicleReportXml();
                    break;
                case "2":
                     consoleApp.GenerateVehicleReportJson();
                    break;
                case "3":
                    consoleApp.LoadReportConsole();
                    break;
                case "4":
                    break;
                default:
                    Console.WriteLine("Неверный выбор, попробуйте еще раз.");
                    break;
            }
            break;
        case "4":
            return;
        default:
            Console.WriteLine("Неверный выбор, попробуйте еще раз.");
            break;
    }
}