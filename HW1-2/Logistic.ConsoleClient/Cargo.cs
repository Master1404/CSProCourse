using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient
{
    public class Cargo
    {
        public double Volume { get; set; }
        public int Weight { get; set; }
        public string Code { get; set; }
        public Cargo(string code, int weight, double volume) 
        {
            Code = code;
            Weight = weight;
            Volume = volume;
        }

        public string GetInformation()
        {
            return $"Code: {Code}, Weight: {Weight} kg, Volume: {Volume} cubic meters";
        }
    }
}
