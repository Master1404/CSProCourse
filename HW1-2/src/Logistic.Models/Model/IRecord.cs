using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.Models
{
    public interface IRecord<TId>
       where TId : struct, IEquatable<TId>
    {
        public TId Id { get; set; }
    }
}
