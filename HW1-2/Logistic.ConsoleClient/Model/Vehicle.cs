using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logistic.ConsoleClient.Enum;

namespace Logistic.ConsoleClient.Model
{
    public class Vehicle: IRecord<int>
    {
        private static int _lastId = 0;
        public int Id { get; set; }
        public string Number { get; set; }
        public int MaxCargoWeightKg { get; set; }
        public double MaxCargoWeightPnd { get; set; }
        public double MaxCargoVolume { get; set; }
        public int CurrentCargoWeight { get; set; } 
        public double CurrentCargoVolume { get; set; }   
        public VehicleType Type { get; set; }
        public List<Cargo> Cargos { get; set; } = new List<Cargo>(100);

        public Vehicle(VehicleType type, int maxCargoWeightKg, double maxCargoVolume, string number = "0000", double maxCarcoWeightPnd = 0.0)
        {
            _lastId++;
            Id = _lastId;
            Type = type;
            Number = number;
            MaxCargoWeightKg = maxCargoWeightKg;
            MaxCargoVolume = maxCargoVolume;
            MaxCargoWeightPnd = maxCarcoWeightPnd;
        }

        public Vehicle()
        {
        }

        public string GetInformation()
        {
            return $"Id = {Id}," +
                $"Number = {Number}," +
                $"Type = {Type},";
        }
    }
}
