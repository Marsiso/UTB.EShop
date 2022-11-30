using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.Application.Paging;
using UTB.EShop.Infrastructure.DbContexts;
using UTB.EShop.Infrastructure.Extensions;

namespace UTB.EShop.Infrastructure.Repositories;

public sealed class Repository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity> 
    where TEntity : class, IDataEntity
{
    public Repository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }
    
    public async Task<PagedList<TEntity>?> GetAllEntitiesAsync<TEntityParameters>(TEntityParameters? entityParameters, bool trackChanges = false)
        where TEntityParameters : RequestParameters
    {
        var entities =  await FindAll(trackChanges)
            .Filter(entityParameters)
            .SearchStringBasedProperties(entityParameters?.SearchTerm)
            .OrderBy(entity => entity.Id)
            .ToListAsync();

        return PagedList<TEntity>.ToPagedList(entities, entityParameters!.PageNumber, entityParameters.PageSize);
    }
    
    public async Task<IEnumerable<TEntity>> GetAllEntitiesAsync(bool trackChanges = false) =>
        await FindAll(trackChanges)
            .OrderBy(entity => entity.Id)
            .ToListAsync();

    public async Task<TEntity?> GetEntityAsync(int id, bool trackChanges  = false) => 
        await FindByCondition(expression: entity => entity.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();

    public async Task<bool> Exist(int id) => 
        await FindByCondition(entity => entity.Id == id, false)
            .AnyAsync();

    public async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges = false) =>
        await FindByCondition(expression: entity => ids.Contains(entity.Id), trackChanges)
            .OrderBy(entity => entity.Id)
            .ToListAsync();

    public void DeleteEntity(TEntity entity) => Delete(entity);
    public void UpdateEntity(TEntity entity) => Update(entity);
    public void CreateEntity(TEntity entity) => Create(entity);
    public async Task SaveAsync() => await RepositoryContext.SaveChangesAsync();
}