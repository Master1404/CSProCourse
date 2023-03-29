using Logistic.ConsoleClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient.Repository
{
    public abstract class InMemoryRepository<TEntity, Tid> where TEntity : IRecord<int>
    {
        protected List<TEntity> _records = new List<TEntity>();
        protected int IdCount = 1;
        private readonly Func<TEntity, Tid> _getId;

        public InMemoryRepository(Func<TEntity, Tid> getId)
        {

            _getId = getId;
        }

        public void Create(TEntity entity)
        {
            
            var entityCopy = DeepCopy(entity);
            entityCopy.Id = IdCount++;
            _records.Add(entityCopy);
        }
        
        public IEnumerable<TEntity> ReadAll()
        {
            return _records.Select(entity => DeepCopy(entity));
        }

        public TEntity GetById(Tid id)
        {
            var entity = _records.FirstOrDefault(e => _getId(e).Equals(id));
            return entity != null ? DeepCopy(entity) : default(TEntity);
        }

        public bool Update(TEntity entity)
        {
            var index = _records.FindIndex(e => _getId(e).Equals(_getId(entity)));
            if (index >= 0)
            {
                _records[index] = DeepCopy(entity);
                return true;
            }
            return false;
        }

        public bool DeleteById(Tid id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _records.Remove(entity);
                return true;
            }
            return false;
        }

        protected abstract TEntity DeepCopy(TEntity? entities);
    }
}
