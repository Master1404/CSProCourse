using Logistic.Model;
using Logistic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Logistic.Core
{
    public interface IService<TEntity, TId>
        where TEntity : IRecord<TId>
        where TId : struct, IEquatable<TId>
    {
        void Create(TEntity entity);
        TEntity GetById(int entityId);
        List<TEntity> GetAll();
        void Delete(int id);
        void LoadCargo(Cargo cargo, int entityId);
        void UnloadCargo(int entityId, Guid cargoId);
    }
}
