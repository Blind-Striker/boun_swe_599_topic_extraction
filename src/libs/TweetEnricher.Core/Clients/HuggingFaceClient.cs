namespace TweetEnricher.Core.Clients;

public class HuggingFaceClient : IHuggingFaceClient
{
    private readonly HttpClient _httpClient;
    private readonly HuggingFaceClientOptions _options;
    private readonly AsyncPolicyWrap<HttpResponseMessage> _combinedPolicy;

    public HuggingFaceClient(HttpClient httpClient, IOptions<HuggingFaceClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        // Define Polly policies
        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        AsyncRetryPolicy<HttpResponseMessage> serviceUnavailable = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(
                retryCount: 2,
                sleepDurationProvider: (_, response, _) =>
                {
                    HuggingFaceErrorResponse? errorResponse = response.Result.Content.ReadFromJsonAsync<HuggingFaceErrorResponse>()?.Result;
                    return errorResponse is not null && !string.IsNullOrEmpty(errorResponse.Error)
                        ? TimeSpan.FromSeconds(errorResponse.EstimatedTime)
                        : TimeSpan.FromSeconds(60);
                },
                onRetryAsync: (_, _, _, _) => Task.CompletedTask);

        AsyncRetryPolicy<HttpResponseMessage>? rateLimitPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(1,
                sleepDurationProvider: (_, result, _) => result?.Result?.Headers?.RetryAfter?.Delta.GetValueOrDefault(TimeSpan.FromSeconds(60)) ?? TimeSpan.FromSeconds(60),
                onRetryAsync: (_, _, _, _) => Task.CompletedTask);

        _combinedPolicy = Policy.WrapAsync(serviceUnavailable, rateLimitPolicy, retryPolicy);
    }

    public async Task<List<HuggingFaceResponse>> AnalyzeSentimentAsync(string text)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.BearerToken);

        var requestUri = $"{_options.ApiBaseUrl}/yiyanghkust/finbert-tone";

        var requestBody = new { inputs = text };
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // Use Polly to handle rate limits, server errors and retries
        HttpResponseMessage response = await _combinedPolicy.ExecuteAsync(async () => await _httpClient.PostAsync(requestUri, content));

        response.EnsureSuccessStatusCode();

        List<List<HuggingFaceResponse>> result = await response.Content.ReadFromJsonAsync<List<List<HuggingFaceResponse>>>() 
                                                 ?? throw new InvalidOperationException();

        return result.First();
    }
}