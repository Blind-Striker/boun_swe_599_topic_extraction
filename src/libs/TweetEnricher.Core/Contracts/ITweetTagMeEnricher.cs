namespace TweetEnricher.Core.Contracts;

public interface ITweetTagMeEnricher
{
    Task<IEnumerable<TagMeEnrichedTweet>> EnrichTweets();
}