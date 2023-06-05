namespace TweetEnricher.Core.Services;

public class TweetTagMeEnricher : ITweetTagMeEnricher
{
    private readonly ITweetDataReader _tweetDataReader;
    private readonly ITagMeClient _tagMeClient;
    private readonly TweetDataOptions _tweetDataOptions;

    public TweetTagMeEnricher(ITweetDataReader tweetDataReader, ITagMeClient tagMeClient,
        IOptions<TweetDataOptions> tweetDataOptions)
    {
        _tweetDataReader = tweetDataReader;
        _tagMeClient = tagMeClient;
        _tweetDataOptions = tweetDataOptions.Value;
    }

    public async Task<IEnumerable<EnrichedTweet>> EnrichTweets()
    {
        IEnumerable<Tweet> tweets = await _tweetDataReader.ReadAndFilterTweetsAsync();

        var enrichedTweets = new List<EnrichedTweet>();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        string fileName = Path.Combine(_tweetDataOptions.DataFolderPath, $"{timestamp}.json");

        await using var fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);


        try
        {
            foreach (Tweet tweet in tweets)
            {
                TagMeResponse tagMeResponse = await _tagMeClient.GetAnnotationsAsync(tweet.Content);

                var enrichedTweet = new EnrichedTweet(tweet.Content, tweet.Date, tweet.Favorite, tweet.Handle,
                    tweet.HashTags,
                    tweet.Name, tweet.Replies, tweet.Retweets, tweet.SearchUrl, tweet.UnixTimestamp, tweet.Url,
                    tagMeResponse.Annotations);

                enrichedTweets.Add(enrichedTweet);

                string serializedEnrichedTweet = JsonSerializer.Serialize(enrichedTweet);
                byte[] serializedEnrichedTweetBytes = Encoding.UTF8.GetBytes(serializedEnrichedTweet);

                await fileStream.WriteAsync(serializedEnrichedTweetBytes, 0, serializedEnrichedTweetBytes.Length);
                await fileStream.WriteAsync(Encoding.UTF8.GetBytes(Environment.NewLine), 0, Environment.NewLine.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            fileStream.Close();
        }

        return enrichedTweets;
    }
}