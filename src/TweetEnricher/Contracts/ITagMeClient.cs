namespace TweetEnricher.Contracts;

internal interface ITagMeClient
{
    Task<TagMeResponse> GetAnnotationsAsync(string text);
}