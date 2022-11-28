using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Application.Interfaces.Models;
using UTB.EShop.Infrastructure.Models;

namespace UTB.EShop.Application.DataShapers;

public sealed class DataShaper<TEntity> : IDataShaper<TEntity>
    where TEntity : class, IDataEntity
{
    public PropertyInfo[] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ShapedEntity> ShapeData(IEnumerable<TEntity> entities, string? fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);
        return FetchData(entities, requiredProperties);
    }
    
    public ShapedEntity ShapeData(TEntity entity, string? fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);
        return FetchDataForEntity(entity, requiredProperties);
    }
    
    private IEnumerable<PropertyInfo> GetRequiredProperties(string? fieldsString)
    {
        var requiredProperties = new List<PropertyInfo>();
        if (!string.IsNullOrWhiteSpace(fieldsString))
        {
            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (var index = 0; index < fields.Length; index++)
            {
                var field = fields[index];
                var property = Properties
                    .FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                if (property == null) continue;
                requiredProperties.Add(property);
            }
        }
        else requiredProperties = Properties.ToList();

        return requiredProperties;
    }

    private IEnumerable<ShapedEntity> FetchData(IEnumerable<TEntity> entities,
        IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedData = new List<ShapedEntity>();
        foreach (var entity in entities)
        {
            var shapedObject = FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }

        return shapedData;
    }
    
    
    /*private IEnumerable<ShapedEntity> FetchData(IEnumerable<TEntity> entities, IEnumerable<PropertyInfo> requiredProperties) =>
        entities.Select(entity => FetchDataForEntity(entity, requiredProperties)).ToList();*/
    
    /*private IEnumerable<ShapedEntity> FetchData(List<TEntity>? entities,
        IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedData = new List<ShapedEntity>();

        Span<TEntity> listAsSpan = CollectionsMarshal.AsSpan(entities);
        ref var searchSpace = ref MemoryMarshal.GetReference(listAsSpan);
        for (var index = 0; index < listAsSpan.Length; index++)
        {
            var entity = Unsafe.Add(ref searchSpace, index);
            var shapedObject = FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }

        return shapedData;
    }*/

    private ShapedEntity FetchDataForEntity(TEntity entity, IEnumerable<PropertyInfo>
        requiredProperties)
    {
        var shapedObject = new ShapedEntity();
        foreach (var property in requiredProperties)
        {
            var objectPropertyValue = property.GetValue(entity);
            shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
        }

        shapedObject.Id = entity.Id;
        
        return shapedObject;
    }
}