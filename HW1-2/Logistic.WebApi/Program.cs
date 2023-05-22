using AutoMapper;
using Logistic.Core;
using Logistic.DAL;
using Logistic.Models;
using Logistic.WebApi.Model;
using System.Runtime.CompilerServices;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IService<Vehicle, int>, VehicleService>();
builder.Services.AddSingleton<IRepository<Vehicle>, InMemoryRepository<Vehicle>>(x => new InMemoryRepository<Vehicle>(y=>y.Id ));

builder.Services.AddScoped<IService<Warehouse, int>, WarehouseService>();
builder.Services.AddSingleton<IRepository<Warehouse>, InMemoryRepository<Warehouse>>(x => new InMemoryRepository<Warehouse>(y => y.Id));

builder.Services.AddScoped<IReportService<Vehicle>, ReportService<Vehicle>>();
builder.Services.AddScoped<IReportRepository<Vehicle>, JsonRepository<Vehicle>>(x => new JsonRepository<Vehicle>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports")));
builder.Services.AddScoped<IReportRepository<Vehicle>, XmlRepository<Vehicle>>(x=> new XmlRepository<Vehicle>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports")));

builder.Services.AddScoped<IReportService<Warehouse>, ReportService<Warehouse>>();
builder.Services.AddScoped<IReportRepository<Warehouse>, JsonRepository<Warehouse>>(x => new JsonRepository<Warehouse>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports")));
builder.Services.AddScoped<IReportRepository<Warehouse>, XmlRepository<Warehouse>>(x => new XmlRepository<Warehouse>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.CreateMap<VehicleModel, Vehicle>();
    mc.CreateMap<WarehouseModel, Warehouse>();
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
