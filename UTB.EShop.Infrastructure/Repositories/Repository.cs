﻿using Microsoft.EntityFrameworkCore;
using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.Application.Paging;
using UTB.EShop.Infrastructure.DbContexts;

namespace UTB.EShop.Infrastructure.Repositories;

public sealed class Repository<TEntity, TEntityParameters> : RepositoryBase<TEntity>, IRepository<TEntity, TEntityParameters> 
    where TEntity : class, IDataEntity
    where TEntityParameters : RequestParameters
{
    public Repository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }
    
    public async Task<PagedList<TEntity>?> GetAllEntitiesAsync(TEntityParameters entityParameters, bool trackChanges = false)
    {
        var entities =  await FindAll(trackChanges)
            .OrderBy(entity => entity.Id)
            .ToListAsync();

        return PagedList<TEntity>.ToPagedList(entities, entityParameters.PageNumber, entityParameters.PageSize);
    }

    public async Task<TEntity?> GetEntityAsync(int id, bool trackChanges  = false) => 
        await FindByCondition(expression: entity => entity.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();

    public async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges = false) =>
        await FindByCondition(expression: entity => ids.Contains(entity.Id), trackChanges)
            .OrderBy(entity => entity.Id)
            .ToListAsync();

    public void DeleteEntity(TEntity entity) => Delete(entity);
    public void UpdateEntity(TEntity entity) => Update(entity);
    public void CreateEntity(TEntity entity) => Create(entity);
    public async Task SaveAsync() => await RepositoryContext.SaveChangesAsync();
}