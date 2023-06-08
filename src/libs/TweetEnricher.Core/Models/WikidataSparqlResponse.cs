namespace TweetEnricher.Core.Models;

public record WikidataSparqlResponse
{
    [SetsRequiredMembers]
    public WikidataSparqlResponse(Head head, Results results)
    {
        Head = head;
        Results = results;
    }

    [JsonPropertyName("head")]
    public required Head Head { get; init; }

    [JsonPropertyName("results")]
    public required Results Results { get; init; }
}

public record Head
{
    [SetsRequiredMembers]
    public Head(List<string> vars)
    {
        Vars = vars;
    }

    [JsonPropertyName("vars")]
    public required List<string> Vars { get; init; }
}

public record Results
{
    [SetsRequiredMembers]
    public Results(List<Binding> bindings)
    {
        Bindings = bindings;
    }

    [JsonPropertyName("bindings")]
    public required List<Binding> Bindings { get; init; }
}

public record Binding
{
    [SetsRequiredMembers]
    public Binding(Item parent, Item relation, Item parentLabel)
    {
        Parent = parent;
        Relation = relation;
        ParentLabel = parentLabel;
    }

    [JsonPropertyName("parent")]
    public required Item Parent { get; init; }

    [JsonPropertyName("relation")]
    public required Item Relation { get; init; }

    [JsonPropertyName("parentLabel")]
    public required Item ParentLabel { get; init; }
}

public record Item
{
    [SetsRequiredMembers]
    public Item(string type, string value)
    {
        Type = type;
        Value = value;
    }

    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("value")]
    public required string Value { get; init; }
}