using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Application.Interfaces.Repositories;

namespace UTB.EShop.Infrastructure.Repositories;

public sealed class Repository<TEntity> : 
    RepositoryBase<TEntity>, 
    IRepository<TEntity> 
    where TEntity : class, IDataEntity
{
    public Repository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }
    
    public async Task<IEnumerable<TEntity>> GetAllEntitiesAsync(bool trackChanges = false) => 
        await FindAll(trackChanges)
            .OrderBy(entity => entity.Id)
            .ToListAsync();

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