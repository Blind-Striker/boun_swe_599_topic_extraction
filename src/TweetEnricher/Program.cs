IHost host = new HostBuilder()
    .UseEnvironment(GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development")
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<TagMeClientOptions>(hostContext.Configuration.GetSection("TagMe"));
        services.Configure<WikipediaClientOptions>(hostContext.Configuration.GetSection("WikipediaClient"));
        services.Configure<WikidataSparqlClientOptions>(hostContext.Configuration.GetSection("WikidataSparqlClient"));
        services.Configure<HuggingFaceClientOptions>(hostContext.Configuration.GetSection("HuggingFaceClient"));
        services.Configure<MongoDbOptions>(hostContext.Configuration.GetSection("MongoDb"));
        services.Configure<TweetDataOptions>(hostContext.Configuration.GetSection("TweetData"));
        services.Configure<AppOptions>(hostContext.Configuration.GetSection("App"));

        services.AddTransient<ITweetDataReader, TweetDataReader>();
        services.AddTransient<ITweetTagMeEnricher, TweetTagMeEnricher>();
        services.AddTransient<ITweetWikidataEnricher, TweetWikidataEnricher>();
        services.AddTransient<ITweetSentimentEnricher, TweetSentimentEnricher>();

        services.AddHttpClient<ITagMeClient, TagMeClient>();
        services.AddHttpClient<IWikipediaClient, WikipediaClient>();
        services.AddHttpClient<IWikidataSparqlClient, WikidataSparqlClient>();
        services.AddHttpClient<IHuggingFaceClient, HuggingFaceClient>();

        services.AddSingleton<IRepository<TagMeEnrichedTweet>, MongoDbRepository<TagMeEnrichedTweet>>();
    })
    .ConfigureLogging(logging => { logging.AddConsole(); })
    .Build();

AppOptions appOptions = host.Services.GetRequiredService<IOptions<AppOptions>>().Value;
TweetDataOptions tweetDataOptions = host.Services.GetRequiredService<IOptions<TweetDataOptions>>().Value;

IEnumerable<TagMeEnrichedTweet> tagMeEnrichedTweets;

switch (appOptions.TagMeEnrichedDataSource)
{
    case "TagMe":
        var tweetTagMeEnricher = host.Services.GetRequiredService<ITweetTagMeEnricher>();
        tagMeEnrichedTweets = await tweetTagMeEnricher.EnrichTweets();
        break;
    case "FileSystem":
        var tweetDataReader = host.Services.GetRequiredService<ITweetDataReader>();
        tagMeEnrichedTweets = await tweetDataReader.ReadTagMeEnrichedTweets(tweetDataOptions.TagMeEnrichedDataFile);
        break;
    default:
        throw new InvalidOperationException("Invalid enriched data source");
}

IEnumerable<WikidataEnrichedTweet> wikiDataEnrichedTweets;

switch (appOptions.WikidataEnrichedDataSource)
{
    case "Wikidata":
        var wikidataEnricher = host.Services.GetRequiredService<ITweetWikidataEnricher>();
        (List<WikidataEnrichedTweet> enrichedWikidataTweets, IList<string> _) = await wikidataEnricher.EnrichTweets(tagMeEnrichedTweets);
        wikiDataEnrichedTweets = enrichedWikidataTweets;
        break;
    case "FileSystem":
        var tweetDataReader = host.Services.GetRequiredService<ITweetDataReader>();
        wikiDataEnrichedTweets = await tweetDataReader.ReadWikidataEnrichedTweets(tweetDataOptions.WikidataEnrichedDataFile);
        break;
    default:
        throw new InvalidOperationException("Invalid enriched data source");
}

IEnumerable<SentimentEnrichedTweet> sentimentEnrichedTweets;

switch (appOptions.SentimentEnrichedDataSource)
{
    case "Sentiment":
        var tweetSentimentEnricher = host.Services.GetRequiredService<ITweetSentimentEnricher>();
        (List<SentimentEnrichedTweet>? enrichedWikidataTweets, IList<string>? _) = await tweetSentimentEnricher.EnrichTweets(wikiDataEnrichedTweets);
        sentimentEnrichedTweets = enrichedWikidataTweets;
        break;
    case "FileSystem":
        var tweetDataReader = host.Services.GetRequiredService<ITweetDataReader>();
        sentimentEnrichedTweets = await tweetDataReader.SentimentEnrichedTweets(tweetDataOptions.SentimentEnrichedDataFile);
        break;
    default:
        throw new InvalidOperationException("Invalid enriched data source");
}

var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
string fileName = Path.Combine(tweetDataOptions.DataFolderPath, $"sentiment_{timestamp}_in_json.json");

await using var fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);

string serializedEnrichedTweet = JsonSerializer.Serialize(sentimentEnrichedTweets);
byte[] serializedEnrichedTweetBytes = Encoding.UTF8.GetBytes(serializedEnrichedTweet);

await fileStream.WriteAsync(serializedEnrichedTweetBytes, 0, serializedEnrichedTweetBytes.Length);

Console.ReadLine();

//var mongoDbRepository = host.Services.GetRequiredService<IRepository<TagMeEnrichedTweet>>();

//foreach (TagMeEnrichedTweet enrichedTweet in enrichedTweets)
//{
//    await mongoDbRepository.InsertAsync(enrichedTweet);
//}