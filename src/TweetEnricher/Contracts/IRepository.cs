namespace TweetEnricher.Contracts;

public interface IRepository<TEntity>
{
    Task InsertAsync(TEntity item);
}