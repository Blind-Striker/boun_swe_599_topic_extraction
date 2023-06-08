namespace TweetEnricher.Core.Contracts;

public interface IWikipediaClient
{
    Task<WikipediaResponse> GetPagePropsAsync(string pageId);
}