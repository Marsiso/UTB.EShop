using System.Dynamic;
using UTB.EShop.Application.Interfaces.Entities;

namespace UTB.EShop.Infrastructure.Models;

public sealed class ShapedEntity
{
    public int Id { get; set; }
    public ExpandoObject Entity { get; set; }
    public ShapedEntity()
    {
        Entity = new ExpandoObject();
    }
}