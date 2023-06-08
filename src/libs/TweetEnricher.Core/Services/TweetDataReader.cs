namespace TweetEnricher.Core.Services;

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
        foreach (string file in Directory.GetFiles(_options.DataFolderPath, "*.json"))
        {
            string content = await File.ReadAllTextAsync(file);
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

    public async Task<IEnumerable<TagMeEnrichedTweet>> ReadTagMeEnrichedTweets(string fileName)
    {
        string[] lines = await File.ReadAllLinesAsync(Path.Combine(_options.DataFolderPath,fileName));
        var enrichedTweets = new List<TagMeEnrichedTweet>();

        foreach (string line in lines)
        {
            var enrichedTweet = JsonSerializer.Deserialize<TagMeEnrichedTweet>(line, TweetEnricherSerializerContext.Default.TagMeEnrichedTweet);

            if (enrichedTweet != null)
            {
                enrichedTweets.Add(enrichedTweet);
            }
        }

        return enrichedTweets;
    }

    public async Task<IEnumerable<WikidataEnrichedTweet>> ReadWikidataEnrichedTweets(string fileName)
    {
        string[] lines = await File.ReadAllLinesAsync(Path.Combine(_options.DataFolderPath,fileName));
        var enrichedTweets = new List<WikidataEnrichedTweet>();

        foreach (string line in lines)
        {
            var enrichedTweet = JsonSerializer.Deserialize<WikidataEnrichedTweet>(line, TweetEnricherSerializerContext.Default.WikidataEnrichedTweet);

            if (enrichedTweet != null)
            {
                enrichedTweets.Add(enrichedTweet);
            }
        }

        return enrichedTweets;
    }

    public async Task<IEnumerable<SentimentEnrichedTweet>> SentimentEnrichedTweets(string fileName)
    {
        string[] lines = await File.ReadAllLinesAsync(Path.Combine(_options.DataFolderPath,fileName));
        var enrichedTweets = new List<SentimentEnrichedTweet>();

        foreach (string line in lines)
        {
            var enrichedTweet = JsonSerializer.Deserialize<SentimentEnrichedTweet>(line, TweetEnricherSerializerContext.Default.SentimentEnrichedTweet);

            if (enrichedTweet != null)
            {
                enrichedTweets.Add(enrichedTweet);
            }
        }

        return enrichedTweets;
    }
}