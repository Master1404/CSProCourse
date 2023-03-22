using AutoMapper;
using Logistic.ConsoleClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient.Repository
{
    internal class WarehouseInInMemoryRepository<T, Tid> : InMemoryRepository<Warehouse, int>
    {
        public WarehouseInInMemoryRepository(Func<Warehouse, int> getId) : base(getId)
        {
        }
        protected override Warehouse DeepCopy(Warehouse? warehouse)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Warehouse, Warehouse>();
            });
            var mapper = config.CreateMapper();
            var vehicleCopy = mapper.Map<Warehouse>(warehouse);
            return new Warehouse
            {
                Id = warehouse.Id,
                Cargos = warehouse.Cargos
            };
        }
    }
}
