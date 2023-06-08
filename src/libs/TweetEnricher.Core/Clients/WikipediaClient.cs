namespace TweetEnricher.Core.Clients;

public class WikipediaClient : IWikipediaClient
{
    private readonly HttpClient _httpClient;
    private readonly WikipediaClientOptions _options;
    private readonly AsyncPolicyWrap<HttpResponseMessage> _combinedPolicy;

    public WikipediaClient(HttpClient httpClient, IOptions<WikipediaClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

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

    public async Task<WikipediaResponse> GetPagePropsAsync(string pageId)
    {
        var requestUri = $"{_options.ApiBaseUrl}?action=query&prop=pageprops&format=json&pageids={pageId}";

        // Use Polly to handle rate limits and retries
        HttpResponseMessage response = await _combinedPolicy.ExecuteAsync(async () => await _httpClient.GetAsync(requestUri));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(TweetEnricherSerializerContext.Default.WikipediaResponse) ?? throw new InvalidOperationException();
    }
}