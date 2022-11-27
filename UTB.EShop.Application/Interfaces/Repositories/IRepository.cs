using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Application.Paging;

namespace UTB.EShop.Application.Interfaces.Repositories;

public interface IRepository<TEntity, TEntityParameters> 
    where TEntity : class, IDataEntity
    where TEntityParameters : RequestParameters
{
    Task<PagedList<TEntity>?> GetAllEntitiesAsync(TEntityParameters entityParameters, bool trackChanges  = false);
    Task<TEntity?> GetEntityAsync(int id, bool trackChanges  = false);
    
    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges = false);
    void DeleteEntity(TEntity entity);
    void UpdateEntity(TEntity entity);
    void CreateEntity(TEntity entity);
    Task SaveAsync();
}