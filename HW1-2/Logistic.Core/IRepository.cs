using Logistic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.Core
{
    public interface IRepository<TEntity, Tid> 
    {
        void Create(TEntity entity);
        IEnumerable<TEntity> ReadAll();
        TEntity GetById(Tid id);
        bool Update(TEntity entity);
        bool DeleteById(Tid id);
    }
}
