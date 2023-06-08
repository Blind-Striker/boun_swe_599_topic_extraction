namespace TweetEnricher.Core.Contracts;

public interface ITweetWikidataEnricher
{
    Task<(List<WikidataEnrichedTweet> enrichedWikidataTweets, IList<string> failedTweets)> EnrichTweets(IEnumerable<TagMeEnrichedTweet> enrichedTweets);
}