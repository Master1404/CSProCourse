using AutoMapper;
using Logistic.Core;
using Logistic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.DAL
{
    public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class, IRecord<int>
    {
        protected List<TEntity> _records = new List<TEntity>();
        //protected ObservableCollection<TEntity> _records = new ObservableCollection<TEntity>();
        protected int IdCount = 1;
        private readonly Func<TEntity, int> _getId;

        public InMemoryRepository(Func<TEntity, int> getId)
        {
            _getId = getId;
            _records = new List<TEntity>();
        }

        public void Create(TEntity entity)
        {
            var entityCopy = DeepCopy(entity);
            entityCopy.Id = IdCount;
            _records.Add(entityCopy);
            IdCount++;
        }
       
        public IEnumerable<TEntity> ReadAll()
        {
            return _records.Select(entity => DeepCopy(entity)).ToList();
        }

        public TEntity GetById(int id)
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

        public bool DeleteById(int id)//переробить
        {
            /*var entity = GetById(id);
            if (entity != null)
            {
                _records.Remove(entity);
                return true;
            }
            return false;*/
            var entity = _records.FirstOrDefault(x => x.Id == id);

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
