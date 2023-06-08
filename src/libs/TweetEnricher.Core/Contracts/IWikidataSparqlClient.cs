namespace TweetEnricher.Core.Contracts;

public interface IWikidataSparqlClient
{
    Task<WikidataSparqlResponse> GetEntityRelationsAsync(string wd);
}