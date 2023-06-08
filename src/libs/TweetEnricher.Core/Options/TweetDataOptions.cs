namespace TweetEnricher.Core.Options;

public class TweetDataOptions
{
    public string DataFolderPath { get; set; }

    public string TagMeEnrichedDataFile { get; set; }

    public string WikidataEnrichedDataFile { get; set; }

    public string SentimentEnrichedDataFile { get; set; }
}