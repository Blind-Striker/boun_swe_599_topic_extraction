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
        services.Configure<MongoDbOptions>(hostContext.Configuration.GetSection("MongoDb"));
        services.Configure<TweetDataOptions>(hostContext.Configuration.GetSection("TweetData"));
        services.Configure<AppOptions>(hostContext.Configuration.GetSection("App"));

        services.AddTransient<ITweetDataReader, TweetDataReader>();
        services.AddTransient<ITweetTagMeEnricher, TweetTagMeEnricher>();
        services.AddHttpClient<ITagMeClient, TagMeClient>();
        services.AddSingleton<IRepository<EnrichedTweet>, MongoDbRepository<EnrichedTweet>>();
    })
    .ConfigureLogging(logging => { logging.AddConsole(); })
    .Build();

AppOptions appOptions = host.Services.GetRequiredService<IOptions<AppOptions>>().Value;
TweetDataOptions tweetDataOptions = host.Services.GetRequiredService<IOptions<TweetDataOptions>>().Value;

IEnumerable<EnrichedTweet> enrichedTweets;

switch (appOptions.EnrichedDataSource)
{
    case "TagMe":
        var tweetTagMeEnricher = host.Services.GetRequiredService<ITweetTagMeEnricher>();
        enrichedTweets = await tweetTagMeEnricher.EnrichTweets();
        break;
    case "FileSystem":
        var tweetDataReader = host.Services.GetRequiredService<ITweetDataReader>();
        enrichedTweets = await tweetDataReader.ReadEnrichedTweets(tweetDataOptions.EnrichedDataFile);
        break;
    default:
        throw new InvalidOperationException("Invalid enriched data source");
}


var mongoDbRepository = host.Services.GetRequiredService<IRepository<EnrichedTweet>>();

foreach (EnrichedTweet enrichedTweet in enrichedTweets)
{
    await mongoDbRepository.InsertAsync(enrichedTweet);
}