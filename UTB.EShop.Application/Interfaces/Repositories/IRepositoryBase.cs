using System;
using System.Linq;
using System.Linq.Expressions;
using UTB.EShop.Application.Interfaces.Entities;

namespace UTB.EShop.Application.Interfaces.Repositories;

public interface IRepositoryBase<TEntity> where TEntity : class, IDataEntity
{
    void Dispose();
    IQueryable<TEntity> FindAll(bool trackChanges);
    IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges);
    void Create(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}