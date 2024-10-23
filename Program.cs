using System;

// simple Pong game
namespace Pong {
    internal class Program {
        static void Main(string[] args) {
            Console.CursorVisible = false;
            int screenWidth = 60;
            int screenHeight = 20;

            Console.WriteLine("Do you want to play against the computer? (y/n)");
            bool vsComputer = Console.ReadKey(true).Key == ConsoleKey.Y;

            Game game = new Game(screenWidth, screenHeight, vsComputer);
            game.Run();
        }
    }

    public class Ball {
        public int X { get; set; }
        public int Y { get; set; }
        public int prevX { get; set; }
        public int prevY { get; set; }
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }

        public Ball(int initialX, int initialY) {
            X = initialX;
            Y = initialY;
            prevX = X;
            prevY = Y;
            DirectionX = 1;  // Move right initially
            DirectionY = 1;  // Move down initially
        }

        public void Move() {
            X += DirectionX;
            Y += DirectionY;
        }

        public void BounceX() {
            DirectionX = -DirectionX;
        }

        public void BounceY() {
            DirectionY = -DirectionY;
        }
    }

    public class Paddle {
        public int X { get; set; }
        public int Y { get; set; }
        public int prevX { get; set; }
        public int prevY { get; set; }
        public int Height { get; set; }

        public Paddle(int x, int y, int height) {
            X = x;
            Y = y;
            prevX = x;
            prevY = y;
            Height = height;
        }

        public void MoveUp() {
            if (Y > 2) Y--;
        }

        public void MoveDown(int boundary) {
            if (Y < (boundary + 2) - Height) Y++;
        }
    }

    public class Game {
        private Ball ball;
        private Paddle player1;
        private Paddle player2;
        private int screenWidth;
        private int screenHeight;
        private bool vsComputer;

        private int player1Score = 0;  
        private int player2Score = 0;  

        public Game(int width, int height, bool playAgainstComputer) {
            screenWidth = width;
            screenHeight = height;
            ball = new Ball(width / 2, height / 2);
            player1 = new Paddle(2, height / 2 - 2, 4);
            player2 = new Paddle(width - 3, height / 2 - 2, 4);
            vsComputer = playAgainstComputer;

            // Initialize previous ball position
            ball.prevX = ball.X;
            ball.prevY = ball.Y;
            // Init previous players position
            player1.prevX = player1.X;
            player1.prevY = player1.Y;
            player2.prevX = player2.X;
            player2.prevY = player2.Y;
        }

        public void Run() {
            bool gameRunning = true;
            DrawPlayfield();
            while (gameRunning) {
                Update();
                Draw();
                HandleInput();
                System.Threading.Thread.Sleep(100);  // Control game speed
            }
        }

        private void Update() {
            ball.Move();

            // Ball collision with top or bottom
            if (ball.Y <= 2 || ball.Y >= screenHeight + 1) {
                ball.BounceY();
            }

            // Ball collision with paddles
            if ((ball.X == player1.X + 1 && (ball.Y >= player1.Y && ball.Y <= (player1.Y + player1.Height))) ||
                (ball.X == player2.X - 1 && (ball.Y >= player2.Y && ball.Y <= (player2.Y + player2.Height)))) {
                ball.BounceX();
            }

            // Ball out of bounds (left or right)
            if (ball.X <= 1) {
                player2Score++;  // Player 2 scores if the ball goes past Player 1
                ResetBall();
            } else if (ball.X >= screenWidth - 1) {
                player1Score++;  // Player 1 scores if the ball goes past Player 2
                ResetBall();
            }

            if (vsComputer) {
                MoveComputerPaddle();
            }
        }

        private void ResetBall() {
            ball.X = screenWidth / 2;
            ball.Y = screenHeight / 2;
            var rnd = new Random();
            ball.DirectionX = rnd.Next(0, 2) == 0 ? -1 : 1;  // Randomize direction on reset
        }

        private void MoveComputerPaddle() {
            // Simple AI: move paddle towards the ball's Y position
            if (ball.Y < player2.Y) {
                player2.MoveUp();
            } else if (ball.Y > player2.Y + player2.Height - 1) {
                player2.MoveDown(screenHeight);
            }
        }

        private void Draw() {
            // Erase the previous ball position
            Console.SetCursorPosition(ball.prevX, ball.prevY);
            Console.Write(" ");

            // Erase the previous paddle positions
            for (int i = 0; i < player1.Height; i++) {
                Console.SetCursorPosition(player1.prevX, player1.prevY + i);
                Console.Write(" ");
            }
            for (int i = 0; i < player2.Height; i++) {
                Console.SetCursorPosition(player2.prevX, player2.prevY + i);
                Console.Write(" ");
            }

            // Draw the score at the top
            Console.SetCursorPosition(screenWidth / 2 - 6, 0);
            Console.Write($"Player 1: {player1Score} | Player 2: {player2Score}");

            // Draw paddles in their new positions
            for (int i = 0; i < player1.Height; i++) {
                Console.SetCursorPosition(player1.X, player1.Y + i);
                Console.Write("|");
            }

            for (int i = 0; i < player2.Height; i++) {
                Console.SetCursorPosition(player2.X, player2.Y + i);
                Console.Write("|");
            }

            // Draw ball in its new position
            Console.SetCursorPosition(ball.X, ball.Y);
            Console.Write("O");

            // Store the current ball position for next frame
            ball.prevX = ball.X;
            ball.prevY = ball.Y;

            player1.prevX = player1.X;
            player1.prevY = player1.Y;
            player2.prevX = player2.X;
            player2.prevY = player2.Y;
        }

        private void DrawPlayfield() {
            Console.Clear();

            // Draw the score at the top
            Console.SetCursorPosition(screenWidth / 2 - 6, 0);
            Console.Write($"Player 1: {player1Score} | Player 2: {player2Score}");

            // Draw top boundary
            Console.SetCursorPosition(0, 1);
            Console.Write(new string('-', screenWidth));

            // Draw bottom boundary
            Console.SetCursorPosition(0, screenHeight + 2);
            Console.Write(new string('-', screenWidth));

            // Draw left and right boundaries (vertical lines)
            for (int i = 2; i < screenHeight + 2; i++) {
                Console.SetCursorPosition(0, i);
                Console.Write("|");

                Console.SetCursorPosition(screenWidth - 1, i);
                Console.Write("|");
            }
        }

        private void HandleInput() {
            if (Console.KeyAvailable) {
                var key = Console.ReadKey(true).Key;

                switch (key) {
                    case ConsoleKey.W:
                        player1.MoveUp();
                        break;
                    case ConsoleKey.S:
                        player1.MoveDown(screenHeight);
                        break;
                    case ConsoleKey.UpArrow:
                        if (!vsComputer) player2.MoveUp();  // Player2 only if not computer
                        break;
                    case ConsoleKey.DownArrow:
                        if (!vsComputer) player2.MoveDown(screenHeight);  // Player2 only if not computer
                        break;
                }
            }
        }
    }


}
