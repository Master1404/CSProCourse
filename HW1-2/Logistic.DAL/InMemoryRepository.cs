using AutoMapper;
using Logistic.Core;
using Logistic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.DAL
{
    public class InMemoryRepository<TEntity, Tid>: IRepository<TEntity, Tid>
        where TEntity : class, IRecord<Tid>
        where Tid : struct, IEquatable<Tid>
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
            entityCopy.Id = (Tid)Convert.ChangeType(IdCount++, typeof(Tid));
            //entityCopy.Id = IdCount++;
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
        private TEntity DeepCopy(TEntity entity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TEntity, TEntity>();
            });
            var mapper = config.CreateMapper();
            var entityCopy = mapper.Map<TEntity>(entity);
            return entityCopy;
        }
    }
}
