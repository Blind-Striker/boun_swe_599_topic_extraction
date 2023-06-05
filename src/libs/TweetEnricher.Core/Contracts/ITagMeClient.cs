namespace TweetEnricher.Core.Contracts;

public interface ITagMeClient
{
    Task<TagMeResponse> GetAnnotationsAsync(string text);
}