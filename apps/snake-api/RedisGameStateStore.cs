using StackExchange.Redis;
using System.Text.Json;

namespace SnakeApi;

public class RedisGameStateStore
{
    private readonly IDatabase _db;

    public RedisGameStateStore(string connectionString)
    {
        var redis = ConnectionMultiplexer.Connect(connectionString);
        _db = redis.GetDatabase();
    }

    public GameStateData? GetGameState(string sessionId)
    {
        var key = $"snake:session:{sessionId}:gamestate";
        var json = _db.StringGet(key);
        return json.HasValue ? JsonSerializer.Deserialize<GameStateData>(json!) : null;
    }

    public void SaveGameState(string sessionId, GameStateData state)
    {
        var key = $"snake:session:{sessionId}:gamestate";
        var json = JsonSerializer.Serialize(state);
        _db.StringSet(key, json, TimeSpan.FromHours(24)); // Auto-expire after 24h
    }
}

// Serializable game state
public class GameStateData
{
    public List<PositionData> Snake { get; set; } = new();
    public PositionData Food { get; set; } = new();
    public string Direction { get; set; } = "right";
    public int Score { get; set; }
    public bool GameStarted { get; set; }
    public bool GameOver { get; set; }
    public bool GamePaused { get; set; }
}

public class PositionData
{
    public int X { get; set; }
    public int Y { get; set; }
}