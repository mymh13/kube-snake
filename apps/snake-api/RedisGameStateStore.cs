// using StackExchange.Redis;
// using System.Text.Json;

// namespace SnakeApi;

// public class RedisGameStateStore
// {
//     private readonly IDatabase _db;
//     private const string GameStateKey = "snake:gamestate";

//     public RedisGameStateStore(string connectionString)
//     {
//         var redis = ConnectionMultiplexer.Connect(connectionString);
//         _db = redis.GetDatabase();
//     }

//     public GameStateData? GetGameState()
//     {
//         var json = _db.StringGet(GameStateKey);
//         return json.HasValue ? JsonSerializer.Deserialize<GameStateData>(json!) : null;
//     }

//     public void SaveGameState(GameStateData state)
//     {
//         var json = JsonSerializer.Serialize(state);
//         _db.StringSet(GameStateKey, json);
//     }
// }

// // Serializable game state
// public class GameStateData
// {
//     public List<Position> Snake { get; set; } = new();
//     public Position Food { get; set; } = new();
//     public string Direction { get; set; } = "right";
//     public int Score { get; set; }
//     public bool GameStarted { get; set; }
//     public bool GameOver { get; set; }
//     public bool GamePaused { get; set; }
// }

// public class Position
// {
//     public int X { get; set; }
//     public int Y { get; set; }
// }