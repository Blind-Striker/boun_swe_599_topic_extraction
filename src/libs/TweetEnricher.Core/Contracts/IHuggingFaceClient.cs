namespace TweetEnricher.Core.Contracts;

public interface IHuggingFaceClient
{
    Task<List<HuggingFaceResponse>> AnalyzeSentimentAsync(string text);
}