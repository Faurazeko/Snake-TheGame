namespace Snake_TheGame
{
    public class SnakeEngine
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsRunning { get; private set; }
        public int Speed { get; private set; } = 100;
        public int Score { get; private set; } = 0;
        public GameState EngineGameState { get; private set; }

        public Position ApplePosition { get; private set; }
        public Position SnakeHeadPosition { get; private set; }
        public List<Position> SnakeBodyPositions { get; private set; } = new List<Position>();

        private MoveDirection SnakeFacingDirection;
        private MoveDirection SheduledDirection;

        private int SheduledBodyPartsCount = 0;

        public delegate void DrawHandler(Position applePosition, Position snakeHeadPosition, ICollection<Position> snakeBodyPartsPositions);
        public event DrawHandler OnDraw;

        public delegate void ScoreHandler(int newScore);
        public event ScoreHandler OnScoreChange;

        public delegate void GameOverHandler();
        public event GameOverHandler OnGameOver;

        private Thread GameThread;
        private CancellationToken ThreadToken;
        private CancellationTokenSource ThreadTokenSource;

        private Random Random;
        public SnakeEngine(int width, int height)
        {
            Random = new Random();

            Width = width;
            Height = height;

            Init();
        }

        public void Start()
        {
            GameThread.Start();
            EngineGameState = GameState.Running;
        }

        private void Init()
        {
            Score = 0;
            SnakeBodyPositions = new();
            EngineGameState = GameState.NotStarted;

            SnakeFacingDirection = MoveDirection.Up;
            SheduledDirection = SnakeFacingDirection;
            SnakeHeadPosition = GeneratePosition();

            SpawnApple();

            ThreadTokenSource = new CancellationTokenSource();
            ThreadToken = ThreadTokenSource.Token;

            GameThread = new Thread(() => UpdateGameField(ThreadToken));
        }

        public void Restart()
        {
            ThreadTokenSource.Cancel();
            Init();
            Start();
        }

        public void Pause() => EngineGameState = GameState.Paused;

        public void Resume() => EngineGameState = GameState.Running;

        public void TogglePause()
        {
            if (EngineGameState == GameState.Paused)
                EngineGameState = GameState.Running;
            else if (EngineGameState == GameState.Running)
                EngineGameState = GameState.Paused;
        }

        private void GameOver()
        {
            EngineGameState = GameState.GameOver;
            ThreadTokenSource.Cancel();
            OnGameOver?.Invoke();
        }

        private void UpdateGameField(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if(EngineGameState == GameState.Running)
                {
                    MoveSnake();
                    EatApple();

                    if(EngineGameState != GameState.GameOver)
                        OnDraw?.Invoke(ApplePosition, SnakeHeadPosition, SnakeBodyPositions);
                }
                Thread.Sleep(Speed);
            }
        }

        private void MoveSnake()
        {
            var moveDir = SnakeFacingDirection;

            if (Convert.ToInt32(SnakeFacingDirection) != -Convert.ToInt32(SheduledDirection))
            {
                moveDir = SheduledDirection;
                SnakeFacingDirection = moveDir;
            }

            var storedHeadPostion = SnakeHeadPosition;

            MoveSnakeHead(moveDir);
            MoveSnakeBody(storedHeadPostion);
            CheckForGameOver(storedHeadPostion);
        }

        private void MoveSnakeBody(Position storedHeadPostion)
        {
            if (SnakeBodyPositions.Count() > 0)
            {
                if (SheduledBodyPartsCount > 0)
                    --SheduledBodyPartsCount;
                else
                    SnakeBodyPositions.RemoveAt(0);

                SnakeBodyPositions.Add(storedHeadPostion);
            }
            else if (SheduledBodyPartsCount > 0 && SnakeBodyPositions.Count <= 0)
            {
                SnakeBodyPositions.Add(storedHeadPostion);
                --SheduledBodyPartsCount;
            }
        }

        private void CheckForGameOver(Position storedHeadPostion)
        {
            if ((SnakeHeadPosition.X > Width || SnakeHeadPosition.Y > Height) || SnakeHeadPosition.X < 0 || SnakeHeadPosition.Y < 0)
            {
                SnakeHeadPosition = storedHeadPostion;
                GameOver();
            }

            foreach (var item in SnakeBodyPositions)
            {
                if (item == SnakeHeadPosition)
                {
                    GameOver();
                    break;
                }
            }
        }

        private void MoveSnakeHead(MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case MoveDirection.Up:
                    SnakeHeadPosition = new Position() { X = SnakeHeadPosition.X, Y = SnakeHeadPosition.Y - 1 };
                    break;
                case MoveDirection.Down:
                    SnakeHeadPosition = new Position() { X = SnakeHeadPosition.X, Y = SnakeHeadPosition.Y + 1 };
                    break;
                case MoveDirection.Right:
                    SnakeHeadPosition = new Position() { X = SnakeHeadPosition.X + 1, Y = SnakeHeadPosition.Y };
                    break;
                case MoveDirection.Left:
                    SnakeHeadPosition = new Position() { X = SnakeHeadPosition.X - 1, Y = SnakeHeadPosition.Y };
                    break;
                default:
                    break;
            }
        }

        public void SheduleMove(MoveDirection direction) => SheduledDirection = direction;

        private void SpawnApple() => ApplePosition = GeneratePosition();

        private Position GeneratePosition()
        {
            while (true)
            {
                var result = new Position() { X = Random.Next(0, Width), Y = Random.Next(0, Height) };
                var isUnique = true;

                if (result == SnakeHeadPosition)
                    continue;

                foreach (var item in SnakeBodyPositions)
                {
                    if (item == result)
                    {
                        isUnique = false;
                        break;
                    }
                }

                if (isUnique)
                    return result;
            }
        }

        private void EatApple()
        {
            if (SnakeHeadPosition == ApplePosition)
            {
                SheduledBodyPartsCount++;
                Score++;

                OnScoreChange?.Invoke(Score);

                SpawnApple();
            }
        }

        public enum MoveDirection
        {
            Up = 1,
            Down = -1,
            Right = 2, 
            Left = -2
        }

        public enum GameState
        {
            NotStarted = 0,
            Running,
            Paused,
            GameOver
        }
        
    }
}
