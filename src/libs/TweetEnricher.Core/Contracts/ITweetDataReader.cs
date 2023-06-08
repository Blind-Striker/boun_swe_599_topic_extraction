namespace TweetEnricher.Core.Contracts;

public interface ITweetDataReader
{
    Task<IEnumerable<Tweet>> ReadAndFilterTweetsAsync();
    Task<IEnumerable<TagMeEnrichedTweet>> ReadTagMeEnrichedTweets(string fileName);
    Task<IEnumerable<WikidataEnrichedTweet>> ReadWikidataEnrichedTweets(string fileName);
    Task<IEnumerable<SentimentEnrichedTweet>> SentimentEnrichedTweets(string fileName);
}