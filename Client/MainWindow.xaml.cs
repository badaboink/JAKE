using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using JAKE.classlibrary;
using JAKE.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace JAKE.client
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private Player currentPlayer;
        private List<Player> playerInfoList = new List<Player>();
        private List<PlayerVisual> playerVisuals = new List<PlayerVisual>();
        //private List<Rectangle> enemyVisuals = new List<Rectangle>();
        private Dictionary<Enemy, Rectangle> enemyVisuals = new Dictionary<Enemy, Rectangle>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private Microsoft.AspNetCore.SignalR.Client.HubConnection connection;
        private DateTime lastGameTime = DateTime.MinValue;

        public MainWindow()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7039/gamehub")
                .Build();
            InitializeComponent();
            WindowState = WindowState.Maximized;

            // Start the SignalR connection when the window loads
            Loaded += MainWindow_Loaded;
            this.KeyDown += MainWindow_KeyDown;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await StartSignalRConnection();
            Task.Run(() => ListenForGameUpdates());
        }

        private async Task StartSignalRConnection()
        {
            while (connection.State != HubConnectionState.Connected)
            {
                await connection.StartAsync();
                await Task.Delay(1000);
            }

            await GameStart();
        }

        private async Task GameStart()
        {
            // Create and show the ColorChoiceForm as a pop-up
            ColorChoiceForm colorChoiceForm = new ColorChoiceForm();
            colorChoiceForm.ShowDialog();
            string selectedColor = colorChoiceForm.SelectedColor;
            string name = colorChoiceForm.Name;

            await connection.SendAsync("SendColor", selectedColor, name);
            connection.On<int, string, string>("YourPlayerInfo", (id, name, color)=>
            {
                currentPlayer = new Player(id, name, color);
            });
            connection.On<string>("ObstacleInfo", (obstacleData) =>
            {
                string[] obstaclemessages = obstacleData.Split(',');
                foreach (string obs in obstaclemessages)
                {
                    string[] parts = obs.Split(':');
                    if (parts.Length == 4)
                    {
                        double width = double.Parse(parts[0]);
                        double height = double.Parse(parts[1]);
                        double posX = double.Parse(parts[2]);
                        double posY = double.Parse(parts[3]);

                        Obstacle obstacle = new Obstacle(width, height, posX, posY);
                        obstacles.Add(obstacle);
                        LoadGameMap();
                    }
                }
            });
            connection.On<List<string>>("PlayerList", (userData) =>
            {
                foreach (string playerEntry in userData)
                {
                    // Split: ID, Name, Color, X, Y
                    string[] parts = playerEntry.Split(':');

                    if (parts.Length == 5)
                    {
                        int playerId = int.Parse(parts[0]);
                        string playerName = parts[1];
                        string playerColor = parts[2];
                        int x = int.Parse(parts[3]);
                        int y = int.Parse(parts[4]);

                        //Create a PlayerInfo object to store player information
                        if (playerId-1>=0 && playerId-1 < playerInfoList.Count)
                        {
                            playerInfoList[playerId-1].SetCurrentPosition(x, y);
                        }
                        else
                        {
                            Player playerInfo = new Player(playerId, playerName, playerColor);
                            playerInfo.SetCurrentPosition(x, y);
                            playerInfoList.Add(playerInfo);
                        }
                    }
                }
                UpdateClientView(playerInfoList);
            });
            connection.On<DateTime>("GameTime", (GameTime) =>
            {
                lastGameTime = GameTime;
            });
            // get already existing enemy list and update view
        }
        private Timer timer;
        private async Task ListenForGameUpdates()
        {
            connection.On<string>("UpdateUsers", (player) =>
            {
                string[] parts = player.Split(':');

                if (parts.Length == 5)
                {
                    int playerId = int.Parse(parts[0]);
                    string playerName = parts[1];
                    string playerColor = parts[2];
                    int x = int.Parse(parts[3]);
                    int y = int.Parse(parts[4]);

                    playerInfoList[playerId - 1].SetCurrentPosition(x, y);

                    Dispatcher.Invoke(() =>
                    {
                        playerInfoList[playerId - 1].SetCurrentPosition(x, y);

                        Player playerInfo = playerInfoList[playerId - 1];
                        PlayerVisual playerVisual = playerVisuals[playerId - 1];
                        Canvas.SetLeft(playerVisual, playerInfo.GetCurrentX());
                        Canvas.SetTop(playerVisual, playerInfo.GetCurrentY());
                    });
                }
            });
            timer = new Timer(CheckElapsedTimeMove, null, 0, 1000);

            connection.On<List<string>>("SendingEnemies", (enemydata) =>
            {
                foreach(string enemystring in enemydata)
                {
                    string[] parts = enemystring.Split(':');
                    if (parts.Length == 5)
                    {
                        int enemyId = int.Parse(parts[0]);
                        string enemyColor = parts[1];
                        double enemyX = double.Parse(parts[2]);
                        double enemyY = double.Parse(parts[3]);
                        int health = int.Parse(parts[4]);
                        Enemy enemy = new Enemy(enemyId, enemyColor);
                        enemy.SetHealth(health);
                        if (!enemies.Contains(enemy))
                        {
                            enemy.SetCurrentPosition(enemyX, enemyY);
                            enemies.Add(enemy);
                            Dispatcher.Invoke(() =>
                            {
                                Rectangle enemyRect = new Rectangle
                                {
                                    Width = 20,
                                    Height = 20,
                                    Fill = Brushes.Red, // Set the enemy's color
                                };
                                enemyVisuals[enemy] = enemyRect;
                                Canvas.SetLeft(enemyRect, enemyX);
                                Canvas.SetTop(enemyRect, enemyY);
                                EnemyContainer.Children.Add(enemyRect);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Rectangle enemyRect = enemyVisuals[enemy];
                                enemy.SetHealth(health);
                                enemy.SetCurrentPosition(enemyX, enemyY);
                                Canvas.SetLeft(enemyRect, enemyX);
                                Canvas.SetTop(enemyRect, enemyY);
                            });
                        }
                    }
                }
            });
            connection.On<int, string>("UpdateDeadEnemy", (enemyid, enemycolor) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Enemy enemy = new Enemy(enemyid, enemycolor);
                    Rectangle enemyRect = enemyVisuals[enemy];
                    enemies.Remove(enemy);
                    enemyVisuals.Remove(enemy);
                    EnemyContainer.Children.Remove(enemyRect);
                });
            });
            connection.On<int, string, int>("UpdateEnemyHealth", (enemyid, enemycolor, enemyhealth) =>
            {
                Enemy enemyToUpdate = enemies.FirstOrDefault(enemy => enemy.MatchesId(enemyid));
                enemyToUpdate?.SetHealth(enemyhealth);
            });
        }
        private async void CheckElapsedTimeMove(object state)
        {
            await connection.SendAsync("SendEnemies");
        }
        private void LoadGameMap()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Canvas gameMapCanvas = new Canvas();
                gameMapCanvas.Name = "GameMap";
                gameMapCanvas.Background = Brushes.Gray;

                // Create a Rectangle (obstacle)
                foreach (Obstacle obs in obstacles)
                {
                    Rectangle obstacleRect = new Rectangle();

                    obstacleRect.Width = obs.Width;
                    obstacleRect.Height = obs.Height;
                    obstacleRect.Fill = Brushes.LightGray;
                    Canvas.SetLeft(obstacleRect, obs.PositionX);
                    Canvas.SetTop(obstacleRect, obs.PositionY);

                    // Add the Rectangle to the Canvas
                    gameMapCanvas.Children.Add(obstacleRect);
                }
                // Replace the existing Canvas with the new one
                playersContainer.Items.Add(gameMapCanvas);
            });
        }
        public string getFirstInstanceUsingSubString(string input, string something)
        {
            int index = input.Contains(something) ? input.IndexOf(something)+something.Length-1 : 0;
            return input.Substring(0, index);
        }
        public string GetStringAfterFirstSpace(string input, string something)
        {
            int index = input.Contains(something) ? input.IndexOf(something) + something.Length : 0;
            return input.Substring(index);
        }
        private void UpdateClientView(List<Player> playerInfoList)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (Player playerInfo in playerInfoList)
                {
                    if (playerInfo.GetId()-1>=0 && playerInfo.GetId() - 1 < playerVisuals.Count)
                    {
                        PlayerVisual playerVisual = playerVisuals[playerInfo.GetId()-1];
                        double x = Canvas.GetLeft(playerVisual);
                        Canvas.SetLeft(playerVisual, playerInfo.GetCurrentX());
                        Canvas.SetTop(playerVisual, playerInfo.GetCurrentY());
                    }
                    else
                    {
                        PlayerVisual playerVisual = new PlayerVisual();
                        ColorConverter converter = new ColorConverter();
                        Color playerColor = (Color)ColorConverter.ConvertFromString(playerInfo.GetColor());
                        SolidColorBrush solidColorBrush = new SolidColorBrush(playerColor);
                        playerVisual.PlayerName = playerInfo.GetName();
                        playerVisual.PlayerColor = solidColorBrush;
                        playerVisual.UpdateColor(solidColorBrush);
                        Canvas.SetLeft(playerVisual, playerInfo.GetCurrentX());
                        Canvas.SetTop(playerVisual, playerInfo.GetCurrentY());
                        playerVisuals.Add(playerVisual);
                        playersContainer.Items.Add(playerVisual);
                    }
                }
                foreach (Enemy enemy in enemies)
                {
                    Rectangle enemyRect = enemyVisuals[enemy];
                    Canvas.SetLeft(enemyRect, enemy.GetCurrentX());
                    Canvas.SetTop(enemyRect, enemy.GetCurrentY());
                }
            });
        }

        private int playerDirectionX = 0;
        private int playerDirectionY = 0;
        private int score = 0;
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            int deltaX = 0;
            int deltaY = 0;

            // Handle arrow key presses here and update the player's position
            // based on the arrow key input.
            if (e.Key == Key.Left)
            {
                deltaX = -1;
                playerDirectionX = deltaX; // Store the player's direction
                playerDirectionY = 0;
            }
            else if (e.Key == Key.Right)
            {
                deltaX = 1;
                playerDirectionX = deltaX; // Store the player's direction
                playerDirectionY = 0;
            }
            else if (e.Key == Key.Up)
            {
                deltaY = -1;
                playerDirectionY = deltaY; // Store the player's direction
                playerDirectionX = 0;
            }
            else if (e.Key == Key.Down)
            {
                deltaY = 1;
                playerDirectionY = deltaY; // Store the player's direction
                playerDirectionX = 0;
            }

            if (e.Key == Key.Space)
            {
                // Use the stored direction for shooting
                Shoot(playerDirectionX, playerDirectionY);

            }
            double playerCurrentX = currentPlayer.GetCurrentX();
            double playerCurrentY = currentPlayer.GetCurrentY();
            int stepSize = 10;
            double newX = playerCurrentX + deltaX * stepSize;
            double newY = playerCurrentY + deltaY * stepSize;

            bool overlap = false;
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle.WouldOverlap(newX, newY, 50, 50))
                {
                    overlap = true;
                    double distance = obstacle.DistanceFromObstacle(playerDirectionX, playerDirectionY, playerCurrentX, playerCurrentY, 50, 50);
                    if (distance!=0)
                    {
                        newX = playerDirectionX == 0 ? playerCurrentX : playerCurrentX + distance;
                        newY = playerDirectionY == 0 ? playerCurrentY : playerCurrentY + distance;

                        Move(newX, newY);
                    }
                    break;
                }
            }
            if (!overlap)
            {
                Move(newX, newY);
            }
        }
        private async void Move(double newX, double newY)
        {
            UpdatePlayer(playerVisuals[currentPlayer.GetId() - 1], newX, newY);
            await connection.SendAsync("SendMove", currentPlayer.GetId(), newX, newY);
        }

        private void Shoot(int deltaX, int deltaY)
        {
            CreateShot(playerVisuals[currentPlayer.GetId()-1], deltaX, deltaY);
        }

        private async void CreateShot(PlayerVisual playerVisual, double directionX, double directionY)
        {
            // Create a new shot visual element (e.g., a bullet or projectile)
            Ellipse shotVisual = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red // You can customize the shot appearance
            };

            // Get the player's position
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);

            // Calculate the center of the player
            double playerCenterX = playerX + playerVisual.Width / 2;
            double playerCenterY = playerY + playerVisual.Height / 2;

            // Set the initial position of the shot at the center of the player
            Canvas.SetLeft(shotVisual, playerCenterX - shotVisual.Width / 2);
            Canvas.SetTop(shotVisual, playerCenterY - shotVisual.Height / 2);

            // Add the shot to the ShotContainer (Canvas)
            ShotContainer.Children.Add(shotVisual);

            // Define the speed of the shot (you can adjust this value)
            double shotSpeed = 5;

            // Update the shot's position based on the direction and speed
            bool shouldRender = true;
            CompositionTarget.Rendering += async (sender, e) =>
            {
                if (!shouldRender) return;
                double currentX = Canvas.GetLeft(shotVisual);
                double currentY = Canvas.GetTop(shotVisual);

                double newX = currentX + directionX * shotSpeed;
                double newY = currentY + directionY * shotSpeed;

                // Check for collisions with obstacles
                foreach (Obstacle obstacle in obstacles)
                {
                    if (obstacle.WouldOverlap(newX, newY, shotVisual.Width, shotVisual.Height))
                    {
                        // Remove the shot and break out of the loop
                        ShotContainer.Children.Remove(shotVisual);
                        shouldRender = false;
                        return;
                    }
                }

                List<Enemy> enemiesToRemove = new List<Enemy>(); // Create a list to store enemies to be removed

    
                bool shotHitEnemy = false;
                foreach (Enemy enemy in enemies)
                {

                    if (enemyVisuals.ContainsKey(enemy))
                    {
                        Rectangle enemyRect = enemyVisuals[enemy];
                        double enemyX = Canvas.GetLeft(enemyRect);
                        double enemyY = Canvas.GetTop(enemyRect);

                        if (newX + shotVisual.Width >= enemyX &&
                            newX <= enemyX + enemyRect.Width &&
                            newY + shotVisual.Height >= enemyY &&
                            newY <= enemyY + enemyRect.Height)
                        {
                            enemy.SetHealth(enemy.GetHealth() - 5); // Reduce the enemy's health
                            score += 5;
                            Debug.WriteLine("score: " + score);
                            scoreLabel.Text = $"Score: {score}";
                            Debug.WriteLine("pataike i enemy");
                            ShotContainer.Children.Remove(shotVisual);
                            shotHitEnemy = true;
                            shouldRender = false;
                            await connection.SendAsync("SendEnemyUpdate", enemy.ToString());
                            if (enemy.GetHealth() <= 0)
                            {
                                enemiesToRemove.Add(enemy); // Add the enemy to the removal list
                                enemyVisuals.Remove(enemy);
                                EnemyContainer.Children.Remove(enemyRect);
                                Debug.WriteLine("mire enemy");
                                await connection.SendAsync("SendDeadEnemy", enemy.ToString());
                            }
                            break;
                        }
                    }
                    
                }
                //Debug.WriteLine("po break"); //lygiai du praeina tarp shots
                // Remove the enemies that need to be removed
                foreach (Enemy enemyToRemove in enemiesToRemove)
                {
                    enemies.Remove(enemyToRemove);
                    Debug.WriteLine("removed enemy");
                }


                // Update the shot's position
                if (!shotHitEnemy)
                {
                    //Debug.WriteLine("paskutinis if");
                    Canvas.SetLeft(shotVisual, newX);
                    Canvas.SetTop(shotVisual, newY);

                    // Remove the shot if it goes out of bounds
                    if (newX < 0 || newX >= ShotContainer.ActualWidth || newY < 0 || newY >= ShotContainer.ActualHeight)
                    {
                        ShotContainer.Children.Remove(shotVisual);
                    }
                }
            };
        }

        private void UpdatePlayer(PlayerVisual playerVisual, double moveX, double moveY)
        {
            Canvas.SetLeft(playerVisual, moveX);
            Canvas.SetTop(playerVisual, moveY);

            currentPlayer.SetCurrentPosition(moveX, moveY);
        }
    }
}
