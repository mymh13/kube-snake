namespace SnakeApi;

public class GameState
{
    private const int GridSize = 20;
    private const int MinInterval = 100; // Minimum speed (fastest)
    private const int IntervalDecrement = 10; // Speed increase per food

    private List<Position> Snake { get; set; } = new() { new Position(10, 10) };
    private Position Food { get; set; } = new Position(15, 15);
    private string Direction { get; set; } = "right";
    private bool GameOver { get; set; } = false;
    private bool GameStarted { get; set; } = false;
    private int Score { get; set; } = 0;
    private readonly System.Timers.Timer _timer;

    // Expose current direction (read-only)
    public string CurrentDirection => Direction;
    public bool IsGameStarted => GameStarted;

    public GameState(System.Timers.Timer timer)
    {
        _timer = timer;
    }

    // Start the game
    public void Start()
    {
        if (!GameStarted && !GameOver)
        {
            GameStarted = true;
        }
    }

    // Reset the game
    public void Reset()
    {
        Snake = new() { new Position(10, 10) };
        Food = new Position(15, 15);
        Direction = "right";
        GameOver = false;
        GameStarted = false;
        Score = 0;
        _timer.Interval = 300; // Reset speed
    }

    // Change direction (called by button press)
    public void ChangeDirection(string direction)
    {
        if (!GameStarted || GameOver) return;

        // Prevent reversing
        if (direction == "up" && Direction != "down") Direction = direction;
        if (direction == "down" && Direction != "up") Direction = direction;
        if (direction == "left" && Direction != "right") Direction = direction;
        if (direction == "right" && Direction != "left") Direction = direction;
    }

    // Move snake (called by timer)
    public void Move(string currentDirection)
    {
        if (!GameStarted || GameOver) return;

        // Calculate new head position
        var head = Snake[0];
        var newHead = currentDirection switch
        {
            "up" => new Position(head.X, head.Y - 1),
            "down" => new Position(head.X, head.Y + 1),
            "left" => new Position(head.X - 1, head.Y),
            "right" => new Position(head.X + 1, head.Y),
            _ => head
        };

        // Check collision with walls
        if (newHead.X < 0 || newHead.X >= GridSize || newHead.Y < 0 || newHead.Y >= GridSize)
        {
            GameOver = true;
            return;
        }

        // Check collision with self
        if (Snake.Any(segment => segment.X == newHead.X && segment.Y == newHead.Y))
        {
            GameOver = true;
            return;
        }

        // Move snake
        Snake.Insert(0, newHead);

        // Check if food eaten
        if (newHead.X == Food.X && newHead.Y == Food.Y)
        {
            Score++;
            GenerateFood();

            // Increase speed (decrease interval)
            var newInterval = _timer.Interval - IntervalDecrement;
            if (newInterval >= MinInterval)
            {
                _timer.Interval = newInterval;
            }
        }
        else
        {
            Snake.RemoveAt(Snake.Count - 1); // Remove tail
        }
    }

    private void GenerateFood()
    {
        var random = new Random();
        Position newFood;
        do
        {
            newFood = new Position(random.Next(0, GridSize), random.Next(0, GridSize));
        } while (Snake.Any(segment => segment.X == newFood.X && segment.Y == newFood.Y));

        Food = newFood;
    }

    public string RenderHTML()
    {
        var html = $@"
        <div style='display: grid; grid-template-columns: repeat({GridSize}, 20px); gap: 1px; background: #1e1e1e; padding: 10px; border-radius: 8px;'>";

        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                var isSnakeHead = Snake[0].X == x && Snake[0].Y == y;
                var isSnakeBody = Snake.Skip(1).Any(s => s.X == x && s.Y == y);
                var isFood = Food.X == x && Food.Y == y;

                var color = isSnakeHead ? "#4ec9b0" :
                           isSnakeBody ? "#3aa18c" :
                           isFood ? "#f5ab3cff" :
                           "#2d2d2d";

                html += $"<div style='width: 20px; height: 20px; background: {color}; border-radius: 2px;'></div>";
            }
        }

        html += "</div>";
        html += $"<p style='color: #4ec9b0; font-size: 1.5em; margin-top: 10px;'>Score: {Score}</p>";

        if (GameOver)
        {
            html += "<p style='color: #ff6b6b; font-size: 1.5em;'>GAME OVER!</p>";
        }
        else if (!GameStarted)
        {
            html += "<p style='color: #4ec9b0; font-size: 1.2em;'>Press START to begin!</p>";
        }

        return html;
    }
}

public record Position(int X, int Y);