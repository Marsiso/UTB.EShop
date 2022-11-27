using System.Dynamic;
using System.Reflection;
using UTB.EShop.Application.Interfaces.Entities;
using UTB.EShop.Application.Interfaces.Models;

namespace UTB.EShop.Infrastructure.Models;

public sealed class DataShaper<TEntity> : IDataShaper<TEntity>
{
    public PropertyInfo[] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ExpandoObject> ShapeData(IEnumerable<TEntity> entities, string fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);
        return FetchData(entities, requiredProperties);
    }
    
    public ExpandoObject ShapeData(TEntity entity, string fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);
        return FetchDataForEntity(entity, requiredProperties);
    }
    
    private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
    {
        var requiredProperties = new List<PropertyInfo>();
        if (!string.IsNullOrWhiteSpace(fieldsString))
        {
            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var field in fields)
            {
                var property = Properties
                    .FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                
                if (property == null) continue;
                requiredProperties.Add(property);
            }
        }
        else requiredProperties = Properties.ToList();

        return requiredProperties;
    }

    private IEnumerable<ExpandoObject> FetchData(IEnumerable<TEntity> entities,
        IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedData = new List<ExpandoObject>();
        foreach (var entity in entities)
        {
            var shapedObject = FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }

        return shapedData;
    }

    private ExpandoObject FetchDataForEntity(TEntity entity, IEnumerable<PropertyInfo>
        requiredProperties)
    {
        var shapedObject = new ExpandoObject();
        foreach (var property in requiredProperties)
        {
            var objectPropertyValue = property.GetValue(entity);
            shapedObject.TryAdd(property.Name, objectPropertyValue);
        }
        
        return shapedObject;
    }
}