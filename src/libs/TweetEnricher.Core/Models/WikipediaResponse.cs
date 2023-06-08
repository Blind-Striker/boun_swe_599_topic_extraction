namespace TweetEnricher.Core.Models;

public record WikipediaResponse
{
    [SetsRequiredMembers]
    public WikipediaResponse(string batchComplete, Query query)
    {
        BatchComplete = batchComplete;
        Query = query;
    }

    [JsonPropertyName("batchcomplete")]
    public required string BatchComplete { get; init; }

    [JsonPropertyName("query")]
    public required Query Query { get; init; }
}

public record Query
{
    [SetsRequiredMembers]
    public Query(Dictionary<string, Page> pages)
    {
        Pages = pages;
    }

    [JsonPropertyName("pages")]
    public required Dictionary<string, Page> Pages { get; init; }
}

public record Page
{
    [SetsRequiredMembers]
    public Page(int pageId, int ns, string title, PageProps pageProps)
    {
        PageId = pageId;
        Ns = ns;
        Title = title;
        PageProps = pageProps;
    }

    [JsonPropertyName("pageid")]
    public required int PageId { get; init; }

    [JsonPropertyName("ns")]
    public required int Ns { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("pageprops")]
    public required PageProps PageProps { get; init; }
}

public record PageProps
{
    [SetsRequiredMembers]
    public PageProps(string pageImageFree, string wikibaseShortdesc, string wikibaseItem)
    {
        PageImageFree = pageImageFree;
        WikibaseShortdesc = wikibaseShortdesc;
        WikibaseItem = wikibaseItem;
    }

    [JsonPropertyName("page_image_free")]
    public required string PageImageFree { get; init; }

    [JsonPropertyName("wikibase-shortdesc")]
    public required string WikibaseShortdesc { get; init; }

    [JsonPropertyName("wikibase_item")]
    public required string WikibaseItem { get; init; }
}