namespace TweetEnricher.Contracts;

public interface ITweetDataReader
{
    Task<IEnumerable<Tweet>> ReadAndFilterTweetsAsync();
}