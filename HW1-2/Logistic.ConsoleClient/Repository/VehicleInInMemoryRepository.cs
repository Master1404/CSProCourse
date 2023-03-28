using AutoMapper;
using Logistic.ConsoleClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient.Repository
{
    public class VehicleInInMemoryRepository<T, Tid> : InMemoryRepository<Vehicle, int>
    {
        public VehicleInInMemoryRepository(Func<Vehicle, int> getId) : base(getId)
        {
        }
        protected override Vehicle DeepCopy(Vehicle? vehicle)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Vehicle, Vehicle>();
            });
            var mapper = config.CreateMapper();
            var vehicleCopy = mapper.Map<Vehicle>(vehicle);
            return vehicleCopy;
        }
    }
}
