using System.Text;
using static Snake_TheGame.SnakeEngine;

namespace Snake_TheGame
{
    public static class Program
    {
        static int WidhtOffset = 3;
        static int HeightOffst = 2;
        static int Score = 0;

        public static void Main()
        {
            SetupConsole();
            DrawBorders();
            DrawScore(Score);


            var engine = new SnakeEngine(Console.BufferWidth - (WidhtOffset * 2), Console.BufferHeight - (HeightOffst * 2));
            engine.OnDraw += DrawSnakeTheGame;
            engine.OnScoreChange += DrawScore;
            engine.OnGameOver += GameOver;
            engine.Start();

            while (true)
            {
                var input = Console.ReadKey(true);
                MoveDirection dir;
                switch (input.Key.ToString().ToLower())
                {
                    case "w":
                        dir = MoveDirection.Up;
                        break;
                    case "a":
                        dir = MoveDirection.Left;
                        break;
                    case "s":
                        dir = MoveDirection.Down;
                        break;
                    case "d":
                        dir = MoveDirection.Right;
                        break;
                    case "r":
                        engine.Restart();
                        Score = 0;
                        continue;
                    case "spacebar":
                        engine.TogglePause();

                        if (engine.EngineGameState == GameState.Paused)
                            DrawPause();
                        continue;
                    default:
                        continue;
                }
                engine.SheduleMove(dir);
            }
        }

        private static void GameOver()
        {
            Console.Clear();
            Console.WriteLine($"Game over :(\nYour score is {Score}\nPress R to restart.");
        }

        private static void DrawPause()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"GAME IS PAUSED! PRESS SPACE TO RESUME");
        }

        private static void SetupConsole()
        {
            Console.Clear();

            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
            Console.CursorVisible = false;
        }

        private static void DrawSnakeTheGame(Position ApplePosition, Position SnakeHeadPosition, ICollection<Position> SnakeBodyPartsPositions)
        {
            DrawBorders();
            DrawScore(Score);
            DrawControls();
            DrawApple(ApplePosition);
            DrawSnakeHead(SnakeHeadPosition);
            DrawSnakeBody(SnakeBodyPartsPositions);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
        }

        private static void DrawApple(Position pos)
        {
            Console.CursorLeft = pos.X + WidhtOffset;
            Console.CursorTop = pos.Y + HeightOffst;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("A");
        }

        private static void DrawSnakeHead(Position pos)
        {
            Console.CursorLeft = pos.X + WidhtOffset;
            Console.CursorTop = pos.Y + HeightOffst;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write("H");
        }

        private static void DrawSnakeBody(ICollection<Position> pos)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Green;
            foreach (var item in pos)
            {
                Console.CursorLeft = item.X + WidhtOffset;
                Console.CursorTop = item.Y + HeightOffst;
                Console.Write("B");
            }
        }

        private static void DrawBorders()
        {
            Console.Clear();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("  ");
            stringBuilder.Insert(1, "_", Console.BufferWidth - 2);
            Console.Write(stringBuilder.ToString());

            stringBuilder.Clear();
            stringBuilder.Append("|    |");
            stringBuilder.Insert(3, "_", Console.BufferWidth - 6);
            Console.Write(stringBuilder.ToString());

            for (int i = 0; i < Console.BufferHeight - 5; i++)
            {
                stringBuilder.Clear();
                stringBuilder.Append("| || |");
                stringBuilder.Insert(3, " ", Console.BufferWidth - 6);
                Console.Write(stringBuilder.ToString());
            }

            stringBuilder.Clear();
            stringBuilder.Append("| || |");
            stringBuilder.Insert(3, "_", Console.BufferWidth - 6);
            Console.Write(stringBuilder.ToString());

            stringBuilder.Clear();
            stringBuilder.Append("||");
            stringBuilder.Insert(1, "_", Console.BufferWidth - 2);
            Console.Write(stringBuilder.ToString());
        }

        private static void DrawControls()
        {
            Console.CursorTop = Console.BufferHeight - 1;
            Console.CursorLeft = 0;
            Console.Write("WASD - to move, R - to restart, SPACE - to pause/resume");
        }

        private static void DrawScore(int score)
        {
            if (score != Score)
                Console.Beep();

            Score = score;
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
            Console.Write($"Score: {score}");
        }
    }
}