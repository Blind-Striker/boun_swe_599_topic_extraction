namespace TweetEnricher.Core.Clients;

public class WikidataSparqlClient : IWikidataSparqlClient
{
    private readonly HttpClient _httpClient;
    private readonly WikidataSparqlClientOptions _options;

    private readonly AsyncPolicyWrap<HttpResponseMessage> _combinedPolicy;

    public WikidataSparqlClient(HttpClient httpClient, IOptions<WikidataSparqlClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("BogaziciUniversity/SWE-599 (term-project; topic-extraction)");

        // Define Polly policies
        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        AsyncRetryPolicy<HttpResponseMessage>? rateLimitPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(1,
                sleepDurationProvider: (_, result, _) => result?.Result?.Headers?.RetryAfter?.Delta.GetValueOrDefault(TimeSpan.FromSeconds(60)) ?? TimeSpan.FromSeconds(60), 
                onRetryAsync: (_, _, _, _) => Task.CompletedTask);

        _combinedPolicy = Policy.WrapAsync(rateLimitPolicy, retryPolicy);
    }

    public async Task<WikidataSparqlResponse> GetEntityRelationsAsync(string wd)
    {
        var sparqlQuery = $@"
        SELECT ?parent ?parentLabel ?relation WHERE {{
            {{ wd:{wd} wdt:P279 ?parent. BIND('subclass of' AS ?relation) }} UNION
            {{ wd:{wd} wdt:P361 ?parent. BIND('part of' AS ?relation) }}
            SERVICE wikibase:label {{ bd:serviceParam wikibase:language '[AUTO_LANGUAGE],en'. }}
        }}";

        var requestUri = $"{_options.ApiBaseUrl}?query={Uri.EscapeDataString(sparqlQuery)}&format=json";
        
        // Use Polly to handle rate limits and retries
        HttpResponseMessage response = await _combinedPolicy.ExecuteAsync(async () => await _httpClient.GetAsync(requestUri));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WikidataSparqlResponse>(TweetEnricherSerializerContext.Default.WikidataSparqlResponse) ?? throw new InvalidOperationException();
    }
}