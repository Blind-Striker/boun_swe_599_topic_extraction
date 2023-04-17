using System.Diagnostics.CodeAnalysis;

namespace TweetEnricher.Models;

public record TagMeResponse
{
    [SetsRequiredMembers]
    public TagMeResponse(string test, ImmutableList<Annotation> annotations, int time, string api, string lang, DateTime timestamp)
    {
        Test = test;
        Annotations = annotations;
        Time = time;
        Api = api;
        Lang = lang;
        Timestamp = timestamp;
    }

    [JsonPropertyName("test")] public required string Test { get; init; }

    [JsonPropertyName("annotations")] public required ImmutableList<Annotation> Annotations { get; init; }

    [JsonPropertyName("time")] public required int Time { get; init; }

    [JsonPropertyName("api")] public required string Api { get; init; }

    [JsonPropertyName("lang")] public required string Lang { get; init; }

    [JsonPropertyName("timestamp")] public required DateTime Timestamp { get; init; }
}

public record Annotation
{
    [SetsRequiredMembers]
    public Annotation(string spot, int start, double linkProbability, double rho, int end, int id, string title)
    {
        Spot = spot;
        Start = start;
        LinkProbability = linkProbability;
        Rho = rho;
        End = end;
        Id = id;
        Title = title;
    }

    [JsonPropertyName("spot")] public required string Spot { get; init; }

    [JsonPropertyName("start")] public required int Start { get; init; }

    [JsonPropertyName("link_probability")] public required double LinkProbability { get; init; }

    [JsonPropertyName("rho")] public required double Rho { get; init; }

    [JsonPropertyName("end")] public required int End { get; init; }

    [JsonPropertyName("id")] public required int Id { get; init; }

    [JsonPropertyName("title")] public required string Title { get; init; }
}