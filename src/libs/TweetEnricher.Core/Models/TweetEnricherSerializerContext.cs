namespace TweetEnricher.Core.Models;

[JsonSerializable(typeof(Tweet))]
[JsonSerializable(typeof(EnrichedTweet))]
[JsonSerializable(typeof(List<Tweet>))]
[JsonSerializable(typeof(TagMeResponse))]
internal partial class TweetEnricherSerializerContext : JsonSerializerContext
{
}