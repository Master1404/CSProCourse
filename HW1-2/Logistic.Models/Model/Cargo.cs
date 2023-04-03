using Logistic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.Model
{
    public class Cargo: IRecord<Guid>
    {
        //test
        public Guid Id { get; set; }
        public double Volume { get; set; }
        public int Weight { get; set; }
        public string Code { get; set; }
        public Invoice Invoice { get; set; }
        public Cargo() { }
        public Cargo(string code, int weight, double volume, Invoice invoice)
        {
            Id= Guid.NewGuid();
            Code = code;
            Weight = weight;
            Volume = volume;
            Invoice = invoice;
        }

        public string GetInformation()
        {
            return $"Code: {Code}," +
                $" Weight: {Weight} kg," +
                $" Volume: {Volume} cubic meters," +
                $" Invoice{Invoice.RecipientPhoneNumber}";
        }
    }
}
