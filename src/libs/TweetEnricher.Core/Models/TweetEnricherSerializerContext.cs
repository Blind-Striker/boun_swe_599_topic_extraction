namespace TweetEnricher.Core.Models;

[JsonSerializable(typeof(Tweet))]
[JsonSerializable(typeof(TagMeEnrichedTweet))]
[JsonSerializable(typeof(WikidataEnrichedTweet))]
[JsonSerializable(typeof(SentimentEnrichedTweet))]
[JsonSerializable(typeof(List<Tweet>))]
[JsonSerializable(typeof(TagMeResponse))]
[JsonSerializable(typeof(WikipediaResponse))]
[JsonSerializable(typeof(WikidataSparqlResponse))]
internal partial class TweetEnricherSerializerContext : JsonSerializerContext
{
}