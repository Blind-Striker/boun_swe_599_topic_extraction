namespace TweetEnricher.Core.Models;

public record Tweet
{
    [SetsRequiredMembers]
    public Tweet(string content, string date, int favorite, string handle, string hashTags, string name, int replies,
        int retweets, string searchUrl, string unixTimestamp, string url)
    {
        Content = content;
        Date = date;
        Favorite = favorite;
        Handle = handle;
        HashTags = hashTags;
        Name = name;
        Replies = replies;
        Retweets = retweets;
        SearchUrl = searchUrl;
        UnixTimestamp = unixTimestamp;
        Url = url;
    }

    [JsonPropertyName("content")] public required string Content { get; init; }

    [JsonPropertyName("date")] public required string Date { get; init; }

    [JsonPropertyName("favorite"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required int Favorite { get; init; }

    [JsonPropertyName("handle")] public required string Handle { get; init; }

    [JsonPropertyName("hashtags")] public required string HashTags { get; init; }

    [JsonPropertyName("name")] public required string Name { get; init; }

    [JsonPropertyName("replies"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required int Replies { get; init; }

    [JsonPropertyName("retweets"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required int Retweets { get; init; }

    [JsonPropertyName("search_url")] public required string SearchUrl { get; init; }

    [JsonPropertyName("unix_timestamp")] public required string UnixTimestamp { get; init; }

    [JsonPropertyName("url")] public required string Url { get; init; }

    public double QualityScore => Replies * 1.0 + Retweets * 0.8 + Favorite * 0.5;
}

public record TagMeEnrichedTweet : Tweet
{
    [SetsRequiredMembers]
    public TagMeEnrichedTweet(string content, string date, int favorite, string handle, string hashTags, string name,
        int replies, int retweets, string searchUrl, string unixTimestamp, string url,
        ImmutableList<Annotation> annotations) : base(content, date, favorite, handle, hashTags, name, replies,
        retweets, searchUrl, unixTimestamp, url)
    {
        Annotations = annotations;
    }

    public required ImmutableList<Annotation> Annotations { get; init; }
}

public record WikidataEnrichedTweet : Tweet
{
    [SetsRequiredMembers]
    public WikidataEnrichedTweet(string content, string date, int favorite, string handle, string hashTags, string name,
        int replies, int retweets, string searchUrl, string unixTimestamp, string url,
        ImmutableList<WikidataEnrichedAnnotation> annotations)
        : base(content, date, favorite, handle, hashTags, name, replies, retweets, searchUrl, unixTimestamp, url)
    {
        Annotations = annotations;
    }

    public required ImmutableList<WikidataEnrichedAnnotation> Annotations { get; init; }
}

public record WikidataEnrichedAnnotation : Annotation
{
    [SetsRequiredMembers]
    public WikidataEnrichedAnnotation(string spot, int start, double linkProbability, double rho, int end, int id,
        string title, string wikidataId, ImmutableList<WikidataTopic> wikidataTopics)
        : base(spot, start, linkProbability, rho, end, id, title)
    {
        WikidataId = wikidataId;
        WikidataTopics = wikidataTopics;
    }

    public required string WikidataId { get; init; }

    public required ImmutableList<WikidataTopic> WikidataTopics { get; init; }
}

public record WikidataTopic(string Id, string Value, string Relation);

public record SentimentEnrichedTweet : WikidataEnrichedTweet
{
    [SetsRequiredMembers]
    public SentimentEnrichedTweet(string content, string date, int favorite, string handle, string hashTags,
        string name, int replies, int retweets, string searchUrl, string unixTimestamp, string url,
        ImmutableList<WikidataEnrichedAnnotation> annotations, ImmutableList<Sentiment> sentiments) : base(content, date, favorite, handle, hashTags, name,
        replies, retweets, searchUrl, unixTimestamp, url, annotations)
    {
        Sentiments = sentiments;
    }

    public required ImmutableList<Sentiment> Sentiments { get; init; }
}

public record Sentiment
{
    [SetsRequiredMembers]
    public Sentiment(string label, double score)
    {
        Label = label;
        Score = score;
    }

    [JsonPropertyName("label")] public required string Label { get; init; }

    [JsonPropertyName("score")] public required double Score { get; init; }
}