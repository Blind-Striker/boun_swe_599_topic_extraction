namespace TweetEnricher.Core.Services;

public class MongoDbRepository<T> : IRepository<T>
{
    private readonly IMongoCollection<T> _collection;

    public MongoDbRepository(IOptions<MongoDbOptions> options)
    {
        MongoDbOptions mongoDbOptions = options.Value;
        var client = new MongoClient(mongoDbOptions.ConnectionString);
        IMongoDatabase database = client.GetDatabase(mongoDbOptions.DatabaseName);
        _collection = database.GetCollection<T>(mongoDbOptions.CollectionName);
    }

    public async Task InsertAsync(T item)
    {
        await _collection.InsertOneAsync(item);
    }
}