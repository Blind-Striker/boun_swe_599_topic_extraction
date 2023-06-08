namespace TweetEnricher.Core.Contracts;

public interface ITweetSentimentEnricher
{
    Task<(List<SentimentEnrichedTweet> enrichedWikidataTweets, IList<string> failedTweets)> EnrichTweets(IEnumerable<WikidataEnrichedTweet> enrichedTweets);
}