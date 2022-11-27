using System.Reflection;
using UTB.EShop.Application.Interfaces.Entities;

namespace UTB.EShop.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    public static IQueryable<TEntity> SearchStringBasedProperties<TEntity>(this IQueryable<TEntity> entities, string searchTerm)
        where TEntity : class, IDataEntity
    {
        // Is Search Term Valid?
        if (string.IsNullOrWhiteSpace(searchTerm)) return entities;
        
        var lowerCaseTerm = searchTerm.Trim().ToLower();
        List<int> ids = new()
        {
            Capacity = entities.Count()
        };

        // Search for matches
        foreach (var entity in entities)
            if ((from property in typeof(TEntity).GetProperties() 
                    where property.PropertyType == typeof(string) 
                    select property.GetValue(entity, null) 
                    into val 
                    where val is not null 
                    select (string) val)
                .Any(s => s.ToLower().Contains(lowerCaseTerm)))
            {
                ids.Add(entity.Id);
            }

        var result = entities.Where(e => ids.Contains(e.Id));
        return result;
    }
}