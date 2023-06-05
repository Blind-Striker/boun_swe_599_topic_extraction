namespace TweetEnricher.Core.Contracts;

public interface IRepository<TEntity>
{
    Task InsertAsync(TEntity item);
}