namespace TweetEnricher.Core.Contracts;

public interface ITweetDataReader
{
    Task<IEnumerable<Tweet>> ReadAndFilterTweetsAsync();
    Task<IEnumerable<EnrichedTweet>> ReadEnrichedTweets(string fileName);
}