using System.Dynamic;
using UTB.EShop.Application.Interfaces.Entities;

namespace UTB.EShop.Application.Interfaces.Models;

public interface IDataShaper<TEntity>
{
    IEnumerable<ExpandoObject> ShapeData(IEnumerable<TEntity> entities, string fieldsString);
    
    ExpandoObject ShapeData(TEntity entity, string fieldsString);
}