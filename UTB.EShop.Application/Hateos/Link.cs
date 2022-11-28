namespace UTB.EShop.Application.Hateos;

public sealed class Link
{
    public string? Href { get; set; } = null!;
    public string Rel { get; set; } = null!;
    public string Method { get; set; } = null!;

    public Link()
    {
    }
    
    public Link(string? href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}