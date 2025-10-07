using MongoDB.Driver;
using Guestbook.Models;

namespace Guestbook.Services;

public class MongoDbService
{
    private readonly IMongoCollection<Message> _messagesCollection;

    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration["MONGODB_CONNECTION_STRING"]
            ?? throw new InvalidOperationException("MONGODB_CONNECTION_STRING environment variable is not set");
        var databaseName = configuration["MONGODB_DATABASE"]
            ?? throw new InvalidOperationException("MONGODB_DATABASE environment variable is not set");

        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseName);
        _messagesCollection = mongoDatabase.GetCollection<Message>("messages");
    }

    // Get last 10 messages, sorted by newest first
    public async Task<List<Message>> GetRecentMessagesAsync()
    {
        return await _messagesCollection
            .Find(_ => true)
            .SortByDescending(m => m.CreatedAt)
            .Limit(10)
            .ToListAsync();
    }

    // Create a new message
    public async Task CreateMessageAsync(Message message)
    {
        await _messagesCollection.InsertOneAsync(message);
    }

    // Delete a message by ID
    public async Task DeleteMessageAsync(string id)
    {
        await _messagesCollection.DeleteOneAsync(m => m.Id == id);
    }
}