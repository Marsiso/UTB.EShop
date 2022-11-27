namespace UTB.EShop.Application.Paging;

public sealed class PagedList<TEntity> : List<TEntity>
{
    public MetaData MetaData {get; set; }
    
    public PagedList(List<TEntity> entities, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
        
        // Pass entities to the parent container
        AddRange(entities);
    }

    public static PagedList<TEntity> ToPagedList(IEnumerable<TEntity> source, int pageNumber, int pageSize)
    {
        var enumerable = source as TEntity[] ?? source.ToArray();
        var count = enumerable.Count();
        var items = enumerable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<TEntity>(items, count, pageNumber, pageSize);
    }
}