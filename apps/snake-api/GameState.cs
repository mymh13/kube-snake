namespace SnakeApi;

public class GameState
{
    private readonly RedisGameStateStore? _redisStore;
    private List<(int X, int Y)> Snake = null!;  // Add = null! to fix warning
    private (int X, int Y) Food;
    private string Direction = null!;  // Add = null! to fix warning
    private int Score;
    private bool GameStarted;
    private bool GameOver;
    private bool GamePaused;
    private const int Width = 20;
    private const int Height = 20;
    private readonly System.Timers.Timer _timer;

    public GameState(System.Timers.Timer timer, RedisGameStateStore? redisStore = null)
    {
        _timer = timer;
        _redisStore = redisStore;

        // Try to load from Redis, otherwise initialize new game
        LoadFromRedis();
    }

    private void LoadFromRedis()
    {
        if (_redisStore != null)
        {
            var data = _redisStore.GetGameState();
            if (data != null)
            {
                Snake = data.Snake.Select(p => (p.X, p.Y)).ToList();
                Food = (data.Food.X, data.Food.Y);
                Direction = data.Direction;
                Score = data.Score;
                GameStarted = data.GameStarted;
                GameOver = data.GameOver;
                GamePaused = data.GamePaused;
                return;  // Only update if we got data from Redis
            }
        }

        // Don't reinitialize if we already have a game in progress!
        // Only initialize if Snake is empty (first load)
        if (Snake == null || Snake.Count == 0)
        {
            Snake = new List<(int X, int Y)> { (10, 10) };
            Food = GenerateFood();
            Direction = "right";
            Score = 0;
            GameStarted = false;
            GameOver = false;
            GamePaused = false;
        }
    }

    private void SaveToRedis()
    {
        if (_redisStore != null)
        {
            var data = new GameStateData
            {
                Snake = Snake.Select(p => new PositionData { X = p.X, Y = p.Y }).ToList(),
                Food = new PositionData { X = Food.X, Y = Food.Y },
                Direction = Direction,
                Score = Score,
                GameStarted = GameStarted,
                GameOver = GameOver,
                GamePaused = GamePaused
            };
            _redisStore.SaveGameState(data);
        }
    }

    // Expose current direction (read-only)
    public string CurrentDirection => Direction;
    public bool IsGameStarted => GameStarted;
    public bool IsGameOver => GameOver;
    public bool IsGamePaused => GamePaused;

    public void Start()
    {
        if (!GameStarted)
        {
            GameStarted = true;
            GameOver = false;
            GamePaused = false;
            SaveToRedis();
        }
    }

    public void Reset()
    {
        Snake = new List<(int X, int Y)> { (10, 10) };
        Food = GenerateFood();
        Direction = "right";
        Score = 0;
        GameStarted = false;
        GameOver = false;
        GamePaused = false;
        SaveToRedis();
    }

    public void TogglePause()
    {
        if (GameStarted && !GameOver)
        {
            GamePaused = !GamePaused;
            SaveToRedis();
        }
    }

    public void ChangeDirection(string newDirection)
    {
        if (GameOver || GamePaused) return;

        bool isOpposite = (Direction == "up" && newDirection == "down") ||
                          (Direction == "down" && newDirection == "up") ||
                          (Direction == "left" && newDirection == "right") ||
                          (Direction == "right" && newDirection == "left");

        if (!isOpposite)
        {
            Direction = newDirection;
            SaveToRedis();
        }
    }

    public void Move(string direction)
    {
        if (GameOver || !GameStarted || GamePaused) return;

        var head = Snake[0];
        var newHead = direction switch
        {
            "up" => (X: head.X, Y: head.Y - 1),
            "down" => (X: head.X, Y: head.Y + 1),
            "left" => (X: head.X - 1, Y: head.Y),
            "right" => (X: head.X + 1, Y: head.Y),
            _ => head
        };

        if (newHead.X < 0 || newHead.X >= Width || newHead.Y < 0 || newHead.Y >= Height || Snake.Contains(newHead))
        {
            GameOver = true;
            SaveToRedis();
            return;
        }

        Snake.Insert(0, newHead);

        if (newHead == Food)
        {
            Score++;
            Food = GenerateFood();
        }
        else
        {
            Snake.RemoveAt(Snake.Count - 1);
        }

        SaveToRedis();
    }

    private (int X, int Y) GenerateFood()
    {
        var random = new Random();
        (int X, int Y) food;
        do
        {
            food = (random.Next(Width), random.Next(Height));
        } while (Snake.Contains(food));
        return food;
    }

    public string RenderHTML()
    {
        // Only reload from Redis if we have Redis AND game is started
        // This prevents overwriting local state when Redis is not available
        if (_redisStore != null && GameStarted)
        {
            LoadFromRedis();
        }

        var grid = new string[Width * Height];
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = "<div style='width: 20px; height: 20px; background: #2d2d2d; border-radius: 2px;'></div>";
        }

        foreach (var segment in Snake)
        {
            int index = segment.Y * Width + segment.X;
            if (index >= 0 && index < grid.Length)
            {
                grid[index] = "<div style='width: 20px; height: 20px; background: #4ec9b0; border-radius: 2px;'></div>";
            }
        }

        int foodIndex = Food.Y * Width + Food.X;
        if (foodIndex >= 0 && foodIndex < grid.Length)
        {
            grid[foodIndex] = "<div style='width: 20px; height: 20px; background: #f5ab3cff; border-radius: 2px;'></div>";
        }

        var html = $@"
        <div style='display: grid; grid-template-columns: repeat({Width}, 20px); gap: 1px; background: #1e1e1e; padding: 10px; border-radius: 8px;'>{string.Join("", grid)}</div>";

        html += $"<p style='color: #4ec9b0; font-size: 1.5em; margin: 10px 0 5px 0;'>Score: {Score}</p>";

        if (!GameStarted)
        {
            html += "<div style='height: 2em; display: flex; align-items: center; justify-content: center; margin: 0;'><span style='color: #4ec9b0; font-size: 1.2em;'>Press START to begin!</span></div>";
        }
        else if (GamePaused)
        {
            html += "<div style='height: 2em; display: flex; align-items: center; justify-content: center; margin: 0;'><span style='color: #ffa500; font-size: 1.5em;'>PAUSED</span></div>";
        }
        else if (GameOver)
        {
            html += "<div style='height: 2em; display: flex; align-items: center; justify-content: center; margin: 0;'><span style='color: #ff6b6b; font-size: 1.5em;'>GAME OVER!</span></div>";
        }
        else
        {
            html += "<div style='height: 2em;'></div>";
        }

        return html;
    }
}