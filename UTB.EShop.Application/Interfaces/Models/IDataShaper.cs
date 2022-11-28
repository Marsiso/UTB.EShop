using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Infrastructure.Models;

namespace UTB.EShop.Application.Interfaces.Models;

public interface IDataShaper<TEntity> where TEntity : IDataEntity
{
    IEnumerable<ShapedEntity> ShapeData(IEnumerable<TEntity> entities, string? fieldsString);
    
    ShapedEntity ShapeData(TEntity entity, string? fieldsString);
}