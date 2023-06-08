namespace TweetEnricher.Core.Services;

public class TweetWikidataEnricher : ITweetWikidataEnricher
{
    private readonly IWikipediaClient _wikipediaClient;
    private readonly IWikidataSparqlClient _wikidataSparqlClient;
    private readonly ILogger<TweetWikidataEnricher> _logger;
    private readonly TweetDataOptions _tweetDataOptions;

    public TweetWikidataEnricher(IWikipediaClient wikipediaClient,
        IWikidataSparqlClient wikidataSparqlClient,
        IOptions<TweetDataOptions> tweetDataOptions,
        ILogger<TweetWikidataEnricher> logger)
    {
        _wikipediaClient = wikipediaClient;
        _wikidataSparqlClient = wikidataSparqlClient;
        _logger = logger;
        _tweetDataOptions = tweetDataOptions.Value;
    }

    public async Task<(List<WikidataEnrichedTweet> enrichedWikidataTweets, IList<string> failedTweets)> EnrichTweets(
        IEnumerable<TagMeEnrichedTweet> enrichedTweets)
    {
        ArgumentNullException.ThrowIfNull(enrichedTweets, nameof(enrichedTweets));

        var enrichedWikidataTweets = new List<WikidataEnrichedTweet>();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        string fileName = Path.Combine(_tweetDataOptions.DataFolderPath, $"wikidata_{timestamp}.json");

        await using var fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);

        IList<string> failedTweets = new List<string>();

        try
        {
            int count = 0;
            foreach (TagMeEnrichedTweet tagMeEnrichedTweet in enrichedTweets)
            {
                var wikidataEnrichedAnnotations = new List<WikidataEnrichedAnnotation>();

                try
                {
                    foreach (Annotation annotation in tagMeEnrichedTweet.Annotations)
                    {
                        int wikiPageId = annotation.Id;
                        string wikidataId;
                        string title;
                        double linkProbability;
                        double rho;


                        if (!annotation.Spot.Equals("GPT", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var strWikiPageId = wikiPageId.ToString();
                            WikipediaResponse pagePropsAsync = await _wikipediaClient.GetPagePropsAsync(strWikiPageId);

                            wikidataId = pagePropsAsync.Query.Pages[strWikiPageId]?.PageProps?.WikibaseItem;

                            if (wikidataId == null)
                            {
                                continue;
                            }

                            title = annotation.Title;
                            linkProbability = annotation.LinkProbability;
                            rho = annotation.Rho;
                        }
                        else
                        {
                            wikidataId = "Q116777014";
                            title = "generative pre-trained transformer";
                            linkProbability = 1;
                            rho = 1;
                        }

                        WikidataSparqlResponse wikidataSparqlResponse =
                            await _wikidataSparqlClient.GetEntityRelationsAsync(wikidataId);

                        List<WikidataTopic> wikidataTopics = wikidataSparqlResponse.Results.Bindings.Select(binding =>
                        {
                            string parentWikidataId = new Uri(binding.Parent.Value).Segments.Last();
                            string relation = binding.Relation.Value;
                            string topic = binding.ParentLabel.Value;

                            return new WikidataTopic(parentWikidataId, topic, relation);
                        }).ToList();

                        wikidataTopics.Add(new WikidataTopic(wikidataId, title?.ToLowerInvariant() ?? annotation.Spot,
                            "self"));

                        var wikidataEnrichedAnnotation = new WikidataEnrichedAnnotation(annotation.Spot,
                            annotation.Start,
                            linkProbability, rho, annotation.End, annotation.Id, title ?? annotation.Spot, wikidataId,
                            wikidataTopics.ToImmutableList());

                        wikidataEnrichedAnnotations.Add(wikidataEnrichedAnnotation);
                    }

                    var wikidataEnrichedTweet = new WikidataEnrichedTweet(tagMeEnrichedTweet.Content,
                        tagMeEnrichedTweet.Date,
                        tagMeEnrichedTweet.Favorite,
                        tagMeEnrichedTweet.Handle, tagMeEnrichedTweet.HashTags, tagMeEnrichedTweet.Name,
                        tagMeEnrichedTweet.Replies, tagMeEnrichedTweet.Retweets, tagMeEnrichedTweet.SearchUrl,
                        tagMeEnrichedTweet.UnixTimestamp, tagMeEnrichedTweet.Url,
                        wikidataEnrichedAnnotations.ToImmutableList());

                    enrichedWikidataTweets.Add(wikidataEnrichedTweet);

                    string serializedEnrichedTweet = JsonSerializer.Serialize(wikidataEnrichedTweet);
                    byte[] serializedEnrichedTweetBytes = Encoding.UTF8.GetBytes(serializedEnrichedTweet);

                    await fileStream.WriteAsync(serializedEnrichedTweetBytes, 0, serializedEnrichedTweetBytes.Length);
                    await fileStream.WriteAsync(Encoding.UTF8.GetBytes(Environment.NewLine), 0, Environment.NewLine.Length);

                    count++;
                    _logger.LogInformation($"Item {count} finished");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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

        return (enrichedWikidataTweets, failedTweets);
    }
}