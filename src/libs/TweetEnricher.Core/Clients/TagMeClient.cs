namespace TweetEnricher.Core.Clients;

public class TagMeClient : ITagMeClient
{
    private readonly HttpClient _httpClient;
    private readonly TagMeClientOptions _options;

    private readonly AsyncPolicyWrap<HttpResponseMessage> _combinedPolicy;

    public TagMeClient(HttpClient httpClient, IOptions<TagMeClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        // Define Polly policies
        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        AsyncRetryPolicy<HttpResponseMessage> rateLimitPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(1, _ => TimeSpan.FromSeconds(5));

        _combinedPolicy = Policy.WrapAsync(retryPolicy, rateLimitPolicy);
    }

    public async Task<TagMeResponse> GetAnnotationsAsync(string text)
    {
        var requestUri = $"{_options.ApiBaseUrl}tag?lang=en&tweet=true&gcube-token={_options.GCubeToken}&text={Uri.EscapeDataString(text)}";

        // Use Polly to handle rate limits and retries
        HttpResponseMessage response = await _combinedPolicy.ExecuteAsync(async () => await _httpClient.GetAsync(requestUri));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(TweetEnricherSerializerContext.Default.TagMeResponse) ?? throw new InvalidOperationException();
    }
}