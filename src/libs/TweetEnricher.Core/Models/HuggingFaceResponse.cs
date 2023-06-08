namespace TweetEnricher.Core.Models;

public record HuggingFaceResponse
{
    [SetsRequiredMembers]
    public HuggingFaceResponse(string label, double score)
    {
        Label = label;
        Score = score;
    }

    [JsonPropertyName("label")] public required string Label { get; init; }

    [JsonPropertyName("score")] public required double Score { get; init; }
}

public record HuggingFaceErrorResponse
{
    [SetsRequiredMembers]
    public HuggingFaceErrorResponse(string error, double estimatedTime)
    {
        Error = error;
        EstimatedTime = estimatedTime;
    }

    [JsonPropertyName("error")] public required string Error { get; init; }

    [JsonPropertyName("estimated_time")] public required double EstimatedTime { get; init; }
}