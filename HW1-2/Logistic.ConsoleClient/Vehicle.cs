using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient
{
    public class Vehicle
    {
        public string Number { get; set; }  
        public int MaxCargoWeightKg { get; set; }  
        public double MaxCargoWeightPnd { get; set; } 
        public double MaxCargoVolume { get; set; }
        public  VehicleType Type { get; set; }   
        public List<Cargo> Cargos { get; set; } = new List<Cargo>(100); 
        
        public Vehicle(VehicleType type, int maxCargoWeightKg, double maxCargoVolume, string number="0000", double maxCarcoWeightPnd = 0.0)
        {
            Type = type;
            Number = number;
            MaxCargoWeightKg = maxCargoWeightKg;
            MaxCargoVolume = maxCargoVolume;
            MaxCargoWeightPnd = maxCarcoWeightPnd;
        }

        public string GetCargoVolumeLeft()  
        {
            return $"Remaining cargo volume: {MaxCargoVolume - GetCurrentVolume()} cubic meters";
        }

        public string GetCargoWeightLeft(WeightUnit weightUnit)
        {
            double remainingWeight = MaxCargoWeightKg - GetCurrentWeigth();
            string result = string.Empty;
            switch (weightUnit)
            {
                case WeightUnit.Kilograms:
                    result = $"Remaining cargo weight: {remainingWeight} kilograms";
                    break;
                case WeightUnit.Pounds:
                    remainingWeight *= 2.20462;
                    result = $"Remaining cargo weight: {remainingWeight} pounds";
                    break;
                default:
                    break;
            }
            return result ;
        }

        public string GetInformation() 
        {
            return $"Number = {Number}, " +
                $"Max Cargo WeightKg = {MaxCargoWeightKg}, " +
                $"Max Cargo WeightPnd = {MaxCargoWeightPnd}, " +
                $"Max Cargo Volume = {MaxCargoVolume}, " +
                $"Type = {Type}, " +
                $"Currenr Weight = {GetCurrentWeigth()}, " +
                $"Current Volume = {GetCurrentVolume()}\n";
        }

        public void LoadCargo(Cargo cargo) 
        {
            if (Cargos == null)
            {
                Cargos= new List<Cargo>();
            }

            if (cargo != null)
            {
                int weight = 0;
                double volume = 0;
                foreach (var item in Cargos)
                {
                    volume += item.Volume;
                    weight += item.Weight;
                }

                weight += cargo.Weight;
                if (weight > MaxCargoWeightKg)
                {
                    throw new Exception($"Current Weight = {GetCurrentWeigth()} kg, imposible to add this cargo({cargo.Weight} kg), because maxWeigth {MaxCargoWeightKg} kg \n");
                }

                volume += cargo.Volume;
                if (volume > MaxCargoVolume)
                {
                    throw new Exception($"Current Volume {GetCurrentVolume()} cubic meters, imposible to add this cargo({cargo.Volume} cubic meters),because maxWeigth {MaxCargoVolume} cubic meters \n");
                }

                if (weight < MaxCargoWeightKg && volume < MaxCargoVolume)
                {
                    Cargos.Add(cargo);
                }
            }
        }

        private int GetCurrentWeigth()
        {
            int weigth = 0;
            if (Cargos != null && Cargos.Any())
            {
                foreach (var cargo in Cargos)
                {
                    weigth += cargo.Weight;
                }
            }

            return weigth;
        }

        private double GetCurrentVolume()
        {
            double volume = 0;
            if (Cargos != null && Cargos.Any())
            {
                foreach (var cargo in Cargos)
                {
                    volume += cargo.Volume;
                }
            }

            return volume;
        }
    }
}
