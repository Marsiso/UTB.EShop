using System.Dynamic;
using System.Net.Mail;

namespace UTB.EShop.Application.Hateos;

public sealed class LinkCollectionWrapper<T> : LinkResourceBase where T : class
{
    public List<T> Value { get; set; } = new();

    public LinkCollectionWrapper()
    {
    }
    
    public LinkCollectionWrapper(List<T> value)
    {
        Value = value;
    }
}