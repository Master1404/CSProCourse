using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.ConsoleClient.Repository
{
    public class InMemoryRepository<TEntity, Tid>
    {
        private List<TEntity> _entities;
        private readonly Func<TEntity, Tid> _getId;

        public InMemoryRepository(Func<TEntity, Tid> getId)
        {
            _entities = new List<TEntity>();
            _getId = getId;
        }

        public void Create(TEntity entity)
        {
            _entities.Add(entity);
        }

        public IEnumerable<TEntity> ReadAll()
        {
            return _entities;
        }

        public TEntity GetById(Tid id)
        {
            return _entities.FirstOrDefault(e => _getId(e).Equals(id));
        }

        public bool Update(TEntity entity)
        {
            var index = _entities.FindIndex(e => _getId(e).Equals(_getId(entity)));
            if (index >= 0)
            {
                _entities[index] = entity;
                return true;
            }
            return false;
        }

        public bool DeleteById(Tid id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _entities.Remove(entity);
                return true;
            }
            return false;
        }
    }
}
