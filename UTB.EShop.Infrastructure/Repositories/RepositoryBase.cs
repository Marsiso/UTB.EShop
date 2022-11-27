using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.Infrastructure.DbContexts;

namespace UTB.EShop.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> : 
    IRepositoryBase<TEntity>, 
    IDisposable
    where TEntity : class, IDataEntity
{
    protected readonly RepositoryContext RepositoryContext;

    private bool _disposed;

    protected RepositoryBase(RepositoryContext repositoryContext)
    {
        RepositoryContext = repositoryContext;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
            if (disposing)
                RepositoryContext.Dispose();

        this._disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IQueryable<TEntity> FindAll(bool trackChanges) =>
        !trackChanges
            ? RepositoryContext.Set<TEntity>()
                .AsNoTracking()
            : RepositoryContext.Set<TEntity>();
    
    public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges) => 
        !trackChanges 
            ? RepositoryContext.Set<TEntity>()
                .Where(expression)
                .AsNoTracking() 
            : RepositoryContext.Set<TEntity>()
                .Where(expression);

    public void Create(TEntity entity) => RepositoryContext.Set<TEntity>().Add(entity);

    public void Update(TEntity entity) => RepositoryContext.Set<TEntity>().Update(entity);

    public void Delete(TEntity entity) => RepositoryContext.Set<TEntity>().Remove(entity);
}