using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient.Model
{
    public class Warehouse: IRecord<int>
    {
        private static int _lastId = 0;
        public int Id { get; set; }
        public List<Cargo> Cargos { get; set; }
        public Warehouse() 
        {
            _lastId++;
            Id = _lastId;
        }
    }
}
