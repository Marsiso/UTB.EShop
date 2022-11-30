using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Application.Paging;

namespace UTB.EShop.Application.Interfaces.Repositories;

public interface IRepository<TEntity> 
    where TEntity : class, IDataEntity
{
    Task<PagedList<TEntity>?> GetAllEntitiesAsync<TEntityParameters>(TEntityParameters? entityParameters, bool trackChanges  = false)
        where TEntityParameters : RequestParameters;

    Task<IEnumerable<TEntity>> GetAllEntitiesAsync(bool trackChanges = false);
    Task<TEntity?> GetEntityAsync(int id, bool trackChanges  = false);
    Task<bool> Exist(int id);

    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges = false);
    void DeleteEntity(TEntity entity);
    void UpdateEntity(TEntity entity);
    void CreateEntity(TEntity entity);
    Task SaveAsync();
}