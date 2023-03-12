using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient.Repository
{
    public interface IRepository<TEntity, TId>
        where TEntity : IRecord<TId>
    where TId : struct, IEquatable<TId>
    {
        void SaveRecords(IEnumerable<IRecord<TId>> records);

        TEntity GetRecordById(TId id);
    }
}
