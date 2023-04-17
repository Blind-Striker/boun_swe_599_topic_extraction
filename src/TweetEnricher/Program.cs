using System.Globalization;
using System.Text;

IHost host = new HostBuilder()
    .UseEnvironment(GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                    GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development")
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<TagMeClientOptions>(hostContext.Configuration.GetSection("TagMe"));
        services.Configure<MongoDbOptions>(hostContext.Configuration.GetSection("MongoDb"));
        services.Configure<TweetDataOptions>(hostContext.Configuration.GetSection("TweetData"));

        services.AddTransient<ITweetDataReader, TweetDataReader>();
        services.AddHttpClient<ITagMeClient, TagMeClient>();
        services.AddSingleton<IRepository<EnrichedTweet>, MongoDbRepository<EnrichedTweet>>();
    })
    .ConfigureLogging(logging => { logging.AddConsole(); })
    .Build();

var tweetDataReader = host.Services.GetRequiredService<ITweetDataReader>();
var tagMeClient = host.Services.GetRequiredService<ITagMeClient>();
var mongoDbRepository = host.Services.GetRequiredService<IRepository<EnrichedTweet>>();

TweetDataOptions tweetDataOptions = host.Services.GetRequiredService<IOptions<TweetDataOptions>>().Value;

IEnumerable<Tweet> tweets = await tweetDataReader.ReadAndFilterTweetsAsync();

var enrichedTweets = new List<EnrichedTweet>();

var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
var fileName = Path.Combine(tweetDataOptions.DataFolderPath, $"{timestamp}.json");

await using var fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);


try
{
    foreach (Tweet tweet in tweets)
    {
        TagMeResponse tagMeResponse = await tagMeClient.GetAnnotationsAsync(tweet.Content);

        var enrichedTweet = new EnrichedTweet(tweet.Content, tweet.Date, tweet.Favorite, tweet.Handle, tweet.HashTags,
            tweet.Name, tweet.Replies, tweet.Retweets, tweet.SearchUrl, tweet.UnixTimestamp, tweet.Url,
            tagMeResponse.Annotations);

        enrichedTweets.Add(enrichedTweet);

        var serializedEnrichedTweet = JsonSerializer.Serialize(enrichedTweet);
        var serializedEnrichedTweetBytes = Encoding.UTF8.GetBytes(serializedEnrichedTweet);

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


foreach (EnrichedTweet enrichedTweet in enrichedTweets)
{
    await mongoDbRepository.InsertAsync(enrichedTweet);
}