namespace TweetEnricher.Models;

[JsonSerializable(typeof(Tweet))]
[JsonSerializable(typeof(List<Tweet>))]
[JsonSerializable(typeof(TagMeResponse))]
internal partial class TweetEnricherSerializerContext : JsonSerializerContext
{
}