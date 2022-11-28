using System.Dynamic;
using UTB.EShop.Infrastructure.Models;

namespace UTB.EShop.Application.Hateos;

public class LinkResponse
{
    public bool HasLinks { get; set; }
    public List<ExpandoObject> ShapedEntities { get; set; }
    public LinkCollectionWrapper<ExpandoObject> LinkedEntities { get; set; }
    public LinkResponse()
    {
        LinkedEntities = new LinkCollectionWrapper<ExpandoObject>();
        ShapedEntities = new List<ExpandoObject>();
    }
}