using System.Collections.Generic;
using System.Threading.Tasks;
using UTB.EShop.Application.Interfaces.Entities;

namespace UTB.EShop.Application.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : class, IDataEntity
{
    Task<IEnumerable<TEntity>> GetAllEntitiesAsync(bool trackChanges  = false);
    Task<TEntity?> GetEntityAsync(int id, bool trackChanges  = false);
    
    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges = false);
    void DeleteEntity(TEntity entity);
    void UpdateEntity(TEntity entity);
    void CreateEntity(TEntity entity);
    Task SaveAsync();
}