namespace TweetEnricher.Services;

public class TweetDataReader : ITweetDataReader
{
    private readonly TweetDataOptions _options;

    public TweetDataReader(IOptions<TweetDataOptions> options)
    {
        _options = options.Value;
    }

    public async Task<IEnumerable<Tweet>> ReadAndFilterTweetsAsync()
    {
        // Read JSON files and deserialize tweet data
        var tweets = new List<Tweet>();
        foreach (var file in Directory.GetFiles(_options.DataFolderPath, "*.json"))
        {
            var content = await File.ReadAllTextAsync(file);
            List<Tweet>? deserializedTweets = JsonSerializer.Deserialize(content, TweetEnricherSerializerContext.Default.ListTweet);

            if (deserializedTweets != null)
            {
                tweets.AddRange(deserializedTweets);
            }
        }

        // Filter out duplicates and low-quality tweets
        List<Tweet> filteredTweets = tweets
            .GroupBy(t => t.Url)
            .Select(g => g.First())
            .Where(t => t.QualityScore >= 20)
            .ToList();

        return filteredTweets;
    }
}