using TweetEnricher.Core.Models;

namespace TweetEnricher.Core.Services;

public class TweetSentimentEnricher : ITweetSentimentEnricher
{
    private readonly IHuggingFaceClient _huggingFaceClient;
    private readonly TweetDataOptions _tweetDataOptions;
    private readonly ILogger<TweetSentimentEnricher> _logger;

    public TweetSentimentEnricher(IHuggingFaceClient huggingFaceClient, IOptions<TweetDataOptions> tweetDataOptions, ILogger<TweetSentimentEnricher> logger)
    {
        _huggingFaceClient = huggingFaceClient;
        _tweetDataOptions = tweetDataOptions.Value;
        _logger = logger;
    }

    public async Task<(List<SentimentEnrichedTweet> enrichedWikidataTweets, IList<string> failedTweets)> EnrichTweets(IEnumerable<WikidataEnrichedTweet> enrichedTweets)
    {
        ArgumentNullException.ThrowIfNull(enrichedTweets, nameof(enrichedTweets));

        var sentimentEnrichedTweets = new List<SentimentEnrichedTweet>();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        string fileName = Path.Combine(_tweetDataOptions.DataFolderPath, $"sentiment_{timestamp}.json");

        await using var fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);

        IList<string> failedTweets = new List<string>();

        try
        {
            int count = 0;
            foreach (WikidataEnrichedTweet tagMeEnrichedTweet in enrichedTweets)
            {
                try
                {
                    List<HuggingFaceResponse> analyzeSentimentAsync = await _huggingFaceClient.AnalyzeSentimentAsync(tagMeEnrichedTweet.Content);

                    var sentimentEnrichedTweet = new SentimentEnrichedTweet(tagMeEnrichedTweet.Content,
                        tagMeEnrichedTweet.Date,
                        tagMeEnrichedTweet.Favorite, tagMeEnrichedTweet.Handle, tagMeEnrichedTweet.HashTags,
                        tagMeEnrichedTweet.Name, tagMeEnrichedTweet.Replies, tagMeEnrichedTweet.Retweets,
                        tagMeEnrichedTweet.SearchUrl, tagMeEnrichedTweet.UnixTimestamp, tagMeEnrichedTweet.Url,
                        tagMeEnrichedTweet.Annotations, analyzeSentimentAsync.Select(response => new Sentiment(response.Label, response.Score)).ToImmutableList());

                    sentimentEnrichedTweets.Add(sentimentEnrichedTweet);

                    
                    string serializedEnrichedTweet = JsonSerializer.Serialize(sentimentEnrichedTweet);
                    byte[] serializedEnrichedTweetBytes = Encoding.UTF8.GetBytes(serializedEnrichedTweet);

                    await fileStream.WriteAsync(serializedEnrichedTweetBytes, 0, serializedEnrichedTweetBytes.Length);
                    await fileStream.WriteAsync(Encoding.UTF8.GetBytes(Environment.NewLine), 0, Environment.NewLine.Length);
                }
                catch (Exception e)
                {
                    failedTweets.Add(tagMeEnrichedTweet.Url);
                    Console.WriteLine(e);
                }

                count++;
                _logger.LogInformation($"Item {count} finished");
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

        return (sentimentEnrichedTweets, failedTweets);
    }
}