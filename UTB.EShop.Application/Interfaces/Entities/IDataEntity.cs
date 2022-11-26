namespace UTB.EShop.Application.Interfaces.Entities;

/// <summary>
/// Generic marker interface for a data model.
/// Used to specifically identify data (persistence-related) models
/// </summary>
/// <typeparam name="TId">The type of Id</typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IDataEntity : IEntity
{
}