﻿using System;
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
using JAKE.classlibrary.Patterns;
using JAKE.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using JAKE.classlibrary.Enemies;
using JAKE.classlibrary.Shots;
using JAKE.classlibrary.Colors;
using JAKE.classlibrary.Collectibles;
namespace JAKE.client
{
    public partial class MainWindow : Window
    {
        private const string UrlToGameHub = "https://localhost:7039/gamehub";
        //---------------------------------------------
        //Chat
        private Chat chatWindow;
        public event EventHandler<string> NameEntered;
        public event EventHandler<string> MessageGot;
        //---------------------------------------------
        private Player currentPlayer;
        private bool gamestarted = false;
        private List<Player> playerInfoList = new();
        private Dictionary<Player, PlayerVisual> playerVisuals = new Dictionary<Player, PlayerVisual>();
        private Dictionary<Enemy, EnemyVisual> enemyVisuals = new Dictionary<Enemy, EnemyVisual>();
        private List<Obstacle> obstacles = new();
        private List<Enemy> enemies = new();
        private Microsoft.AspNetCore.SignalR.Client.HubConnection connection;
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable S4487 // Unread "private" fields should be removed
        private DateTime lastGameTime = DateTime.MinValue;
#pragma warning restore S4487 // Unread "private" fields should be removed
#pragma warning restore IDE0052 // Remove unread private members
        private Dictionary<Enemy, bool> collisionCheckedEnemies = new();
        private List<Coin> coins = new();
        private Dictionary<Coin, CoinVisual> coinVisuals = new();
        private List<HealthBoost> healthBoosts = new();
        private Dictionary<HealthBoost, HealthBoostVisual> healthBoostsVisuals = new();
        private List<Shield> shields = new(); 
        private Dictionary<Shield, ShieldVisual> shieldVisuals = new();
        private List<SpeedBoost> speedBoosts = new List<SpeedBoost>();
        private Dictionary<SpeedBoost, SpeedBoostVisual> speedBoostsVisuals = new();
        private readonly object enemyListLock = new();
        Controller controller = new();


        private bool isCollidingWithHealthBoost = false;


        public MainWindow()
        {
            currentPlayer = new();
            chatWindow = new Chat(this);
            connection = new HubConnectionBuilder()
                .WithUrl(UrlToGameHub)
                .Build();
            InitializeComponent();
            WindowState = WindowState.Maximized;

            // Start the SignalR connection when the window loads
            chatWindow.MessageSent += ChatWindow_SendMessage;
            Loaded += MainWindow_Loaded;
            this.KeyDown += MainWindow_KeyDown;

            foreach (Enemy enemy in enemies)
            {
                collisionCheckedEnemies[enemy] = false;
            }
        }

        

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await StartSignalRConnection();
            chatWindow.Show();
            await Task.Run(() => ListenForGameUpdates());
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
        IBuilderVisual<PlayerVisual> playerVisualBuilder = new PlayerVisualBuilder();
        IBuilderVisual<EnemyVisual> enemyVisualBuilder = new EnemyVisualBuilder();
        IBuilderVisual<ShotVisual> shotVisualBuilder = new ShotVisualBuilder();
        public async Task GameStart()
        {            
            // Create and show the ColorChoiceForm as a pop-up
            ColorChoiceForm colorChoiceForm = new ColorChoiceForm();
            colorChoiceForm.ShowDialog();
            string selectedColor = colorChoiceForm.SelectedColor;
            string name = colorChoiceForm.Name;
            string shotColor = colorChoiceForm.ShotColor;
            string shotShape = colorChoiceForm.ShotShape;
            GameStats gameStat = GameStats.Instance;
            gameStat.WindowWidth = this.ActualWidth;
            gameStat.WindowHeight = this.ActualHeight;

            await connection.SendAsync("SendColor", selectedColor, name, shotColor, shotShape);
            connection.On<int, string, string, string>("GameStart", (id, name, color, obstacleData) =>
            {
                SetCurrentPlayer(id, name, color, shotColor, shotShape);
                NameEntered?.Invoke(this, name);
                gamestarted = true;
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
            connection.On<List<string>, DateTime>("GameUpdate", (userData, gametime) =>
            {
                foreach (string playerEntry in userData)
                {
                    string[] parts = playerEntry.Split(':');

                    if (parts.Length == 7)
                    {
                        int playerId = int.Parse(parts[0]);
                        string playerName = parts[1];
                        string playerColor = parts[2];
                        int x = int.Parse(parts[3]);
                        int y = int.Parse(parts[4]);
                        string shotColor = parts[5];
                        string shotShape = parts[6];

                        Player playerInfo = new Player(playerId, playerName, playerColor, shotColor, shotShape);
                        playerInfo.SetCurrentPosition(x, y);

                        if (!playerInfoList.Contains(playerInfo))
                        {
                            playerInfoList.Add(playerInfo);
                            Dispatcher.Invoke(() =>
                            {
                                PlayerVisual playerVisual = playerVisualBuilder.New()
                                .SetName(playerInfo.GetName())
                                .SetColor(playerInfo.GetColor())
                                .SetPosition(x, y)
                                .Build();
                                playerVisuals[playerInfo] = playerVisual;

                                playersContainer.Items.Add(playerVisual);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                PlayerVisual playerVisual = playerVisuals[playerInfo];
                                Canvas.SetLeft(playerVisual, x);
                                Canvas.SetTop(playerVisual, y);
                            });
                        }
                    }
                }
                GameStats gameStat = GameStats.Instance;
                gameStat.PlayersCount = playerInfoList.Count;  //singleton
                lastGameTime = gametime;
            });
        }
        public void SetCurrentPlayer(int id, string name, string color, string shotColor, string shotShape)
        {
            currentPlayer = new Player(id, name, color, shotColor, shotShape);
            playerInfoList.Add(currentPlayer);
            Dispatcher.Invoke(() =>
            {
                PlayerVisual playerVisual = playerVisualBuilder.New()
                .SetName(currentPlayer.GetName())
                .SetColor(currentPlayer.GetColor())
                .SetPosition(0, 0)
                .Build();
                playerVisuals[currentPlayer] = playerVisual;

                playersContainer.Items.Add(playerVisual);
            });
        }
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable IDE0052 // Remove unread private members
        private Timer? timer;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore S4487 // Unread "private" fields should be removed
        private async Task ListenForGameUpdates()
        {
            UpdateUsers();

            timer = new Timer(CheckElapsedTimeMove, null, 0, 1000);
            GetMessage();
            SendingEnemies();
            UpdateShotsFired();
            UpdateDeadEnemy();
            UpdateEnemyHealth();
            DisconnectedPlayer();
            SendingCoins();
            SendingPickedCoin();
            SendingShield();
            SendingPickedShield();
            SendingHealthBoosts();
            SendingPickedHealthBoost();
            SendingSpeedBoosts();
            SendingPickedSpeedBoost();
        }

        private void SendingPickedSpeedBoost()
        {
            connection.On<int>("SendingPickedSpeedBoost", (speedid) =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var pair in speedBoostsVisuals)
                    {
                        SpeedBoost speedBoost = pair.Key;
                        SpeedBoostVisual speedBoostVisual = pair.Value;
                        if (speedBoost.id == speedid)
                        {
                            speedBoosts.Remove(speedBoost);
                            speedBoostsVisuals.Remove(speedBoost);
                            SpeedBoostContainer.Children.Remove(speedBoostVisual);
                        }
                    }
                });
            });
        }

        private void SendingSpeedBoosts()
        {
            connection.On<List<string>>("SendingSpeedBoosts", (speeddata) =>
            {

                foreach (string speedstring in speeddata)
                {
                    string image = "speedboost.png";
                    string[] parts = speedstring.Split(':');
                    if (parts.Length == 7)
                    {
                        int speedId = int.Parse(parts[0]);
                        double speedX = double.Parse(parts[1]);
                        double speedY = double.Parse(parts[2]);
                        int speedVal = int.Parse(parts[5]);
                        SpeedBoost speed = new SpeedBoost(speedId, speedX, speedY, speedVal, image);
                        if (!speedBoosts.Contains(speed))
                        {
                            speedBoosts.Add(speed);
                            Dispatcher.Invoke(() =>
                            {
                                SpeedBoostVisual speedVisual = new SpeedBoostVisual();
                                speedBoostsVisuals[speed] = speedVisual;
                                Canvas.SetLeft(speedVisual, speedX);
                                Canvas.SetTop(speedVisual, speedY);
                                SpeedBoostContainer.Children.Add(speedVisual);
                                HandleSpeedBoostsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                    }
                }

            });
        }

        private void SendingPickedHealthBoost()
        {
            connection.On<int>("SendingPickedHealthBoost", (healthid) =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var pair in healthBoostsVisuals)
                    {
                        HealthBoost healthBoost = pair.Key;
                        HealthBoostVisual healthBoostVisual = pair.Value;
                        if (healthBoost.id == healthid)
                        {
                            healthBoosts.Remove(healthBoost);
                            healthBoostsVisuals.Remove(healthBoost);
                            HealthBoostContainer.Children.Remove(healthBoostVisual);
                        }
                    }
                });
            });
        }

        private void SendingHealthBoosts()
        {
            connection.On<List<string>>("SendingHealthBoosts", (healthdata) =>
            {
                string image = "healthBoost.png";
                foreach (string healthstring in healthdata)
                {
                    string[] parts = healthstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int healthId = int.Parse(parts[0]);
                        double healthX = double.Parse(parts[1]);
                        double healthY = double.Parse(parts[2]);
                        int healthVal = int.Parse(parts[5]);
                        HealthBoost health = new HealthBoost(healthId, healthX, healthY, healthVal, image);
                        if (!healthBoosts.Contains(health))
                        {
                            healthBoosts.Add(health);
                            Dispatcher.Invoke(() =>
                            {
                                HealthBoostVisual healthVisual = new HealthBoostVisual();
                                healthBoostsVisuals[health] = healthVisual;
                                Canvas.SetLeft(healthVisual, healthX);
                                Canvas.SetTop(healthVisual, healthY);
                                HealthBoostContainer.Children.Add(healthVisual);
                                HandleHealthBoostsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                    }
                }

            });
        }

        private void SendingPickedShield()
        {
            connection.On<int>("SendingPickedShield", (shieldid) =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var pair in shieldVisuals)
                    {
                        Shield shield = pair.Key;
                        ShieldVisual shieldVisual = pair.Value;

                        if (shield.id == shieldid)
                        {
                            shields.Remove(shield);
                            shieldVisuals.Remove(shield);
                            ShieldContainer.Children.Remove(shieldVisual);
                            break;
                        }
                    }

                });
            });
        }

        private void SendingShield()
        {
            connection.On<List<string>>("SendingShields", (shieldsdata) =>
            {
                string image = "shield.png";
                foreach (string shieldstring in shieldsdata)
                {
                    string[] parts = shieldstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int shieldId = int.Parse(parts[0]);
                        double shieldX = double.Parse(parts[1]);
                        double shieldY = double.Parse(parts[2]);
                        int time = int.Parse(parts[5]);
                        Shield shield = new Shield(shieldId, shieldX, shieldY, time, image);

                        if (!shields.Contains(shield))
                        {
                            shields.Add(shield);
                            Dispatcher.Invoke(() =>
                            {

                                ShieldVisual shieldVisual = new ShieldVisual();
                                Canvas.SetLeft(shieldVisual, shieldX);
                                Canvas.SetTop(shieldVisual, shieldY);
                                shieldVisuals[shield] = shieldVisual;
                                ShieldContainer.Children.Add(shieldVisual);
                                HandleShieldsCollisions(playerVisuals[currentPlayer]);
                            });
                        }

                    }
                }

            });
        }

        private void SendingPickedCoin()
        {
            connection.On<string>("SendingPickedCoin", (coinObj) =>
            {
                string coinString = new ServerString(coinObj).ConvertedString;
                string[] parts = coinString.Split(':');
                int id = int.Parse(parts[1]);
                Dispatcher.Invoke(() =>
                {
                    foreach (var pair in coinVisuals)
                    {
                        Coin coin = pair.Key;
                        CoinVisual coinVisual = pair.Value;

                        if (coin.id == id)
                        {
                            coins.Remove(coin);
                            coinVisuals.Remove(coin);
                            CoinContainer.Children.Remove(coinVisual);
                            break;
                        }
                    }

                });
            });
        }

        private void SendingCoins()
        {
            connection.On<List<string>>("SendingCoins", (coinsdata) =>
            {
                string image = "coin.png";
                foreach (string coinstring in coinsdata)
                {
                    string[] parts = coinstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int coinId = int.Parse(parts[0]);
                        double coinX = double.Parse(parts[1]);
                        double coinY = double.Parse(parts[2]);
                        int points = int.Parse(parts[5]);
                        Coin coin = new Coin(coinId, coinX, coinY, points, image);
                        if (!coins.Contains(coin))
                        {
                            coins.Add(coin);
                            Dispatcher.Invoke(() =>
                            {

                                CoinVisual coinVisual = new CoinVisual();
                                Canvas.SetLeft(coinVisual, coinX);
                                Canvas.SetTop(coinVisual, coinY);
                                coinVisuals[coin] = coinVisual;
                                CoinContainer.Children.Add(coinVisual);

                                HandleCoinsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                    }
                }

            });
        }

        private void DisconnectedPlayer()
        {
            connection.On<string>("DisconnectedPlayer", (player) =>
            {
                string[] parts = player.Split(':');
                if (parts.Length == 7)
                {
                    int playerId = int.Parse(parts[0]);
                    string playerName = parts[1];
                    string playerColor = parts[2];
                    string shotColor = parts[5];
                    string shotShape = parts[6];
                    Player playerToDelete = new Player(playerId, playerName, playerColor, shotColor, shotShape);
                    Dispatcher.Invoke(() =>
                    {
                        PlayerVisual playerVisual = playerVisuals[playerToDelete];
                        playerVisuals.Remove(playerToDelete);
                        playerInfoList.Remove(playerToDelete);
                        playersContainer.Items.Remove(playerVisual);
                    });
                }
            });
        }

        private void UpdateEnemyHealth()
        {
            connection.On<int, string, int>("UpdateEnemyHealth", (enemyid, enemycolor, enemyhealth) =>
            {
                Enemy? enemyToUpdate = enemies.Find(enemy => enemy.MatchesId(enemyid));
                if (enemyToUpdate != null)
                {
                    enemyToUpdate.SetHealth(enemyhealth);
                }
            });
        }

        private void UpdateDeadEnemy()
        {
            connection.On<int, string>("UpdateDeadEnemy", (enemyid, enemycolor) =>
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    Enemy enemy = new Enemy(enemyid, enemycolor);
                    EnemyVisual enemyRect = enemyVisuals[enemy];
                    enemies.Remove(enemy);
                    enemyVisuals.Remove(enemy);
                    EnemyContainer.Children.Remove(enemyRect);
                }));
            });
        }

        private void UpdateShotsFired()
        {
            connection.On<int, double, double>("UpdateShotsFired", (playerid, X, Y) =>
            {
                Player? playerToUpdate = playerInfoList.Find(player => player.MatchesId(playerid));
                if (playerToUpdate != null)
                {
                    CreateShot(playerVisuals[playerToUpdate], X, Y, playerToUpdate.GetShotColor(), playerToUpdate.GetShotShape());
                }
            });
        }

        private void SendingEnemies()
        {
            connection.On<List<string>>("SendingEnemies", (enemydata) =>
            {
                foreach (string enemystring in enemydata)
                {
                    string[] parts = enemystring.Split(':');
                    if (parts.Length == 6)
                    {
                        int enemyId = int.Parse(parts[0]);
                        string enemyColor = parts[1];
                        double enemyX = double.Parse(parts[2]);
                        double enemyY = double.Parse(parts[3]);
                        int health = int.Parse(parts[4]);
                        int size = int.Parse(parts[5]);
                        Enemy enemy = new Enemy(enemyId, enemyColor);
                        enemy.SetHealth(health); enemy.SetSize(size);
                        if (!enemies.Contains(enemy))
                        {
                            enemy.SetCurrentPosition(enemyX, enemyY);
                            enemies.Add(enemy);
                            lock (enemyListLock)
                            {
                                Dispatcher.Invoke(() =>
                                {

                                    EnemyVisual enemyVisual = enemyVisualBuilder.New()
                                    .SetColor(enemyColor)
                                    .SetSize(size)
                                    .SetPosition(enemyX, enemyY)
                                    .Build();

                                    enemyVisuals[enemy] = enemyVisual;
                                    EnemyContainer.Children.Add(enemyVisual);
                                    HandleEnemyCollisions(playerVisuals[currentPlayer]);

                                });
                            }
                        }
                        else
                        {
                            lock (enemyListLock)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    EnemyVisual enemyVisual = enemyVisuals[enemy];
                                    enemy.SetHealth(health);
                                    enemy.SetCurrentPosition(enemyX, enemyY);
                                    Canvas.SetLeft(enemyVisual, enemyX);
                                    Canvas.SetTop(enemyVisual, enemyY);

                                    HandleEnemyCollisions(playerVisuals[currentPlayer]);
                                });
                            }
                        }
                    }
                }
                foreach (Enemy enemy in enemies)
                {
                    collisionCheckedEnemies[enemy] = false;
                }
            });
        }

        private void UpdateUsers()
        {
            connection.On("UpdateUsers", (Action<string>)((player) =>
            {
                string[] parts = player.Split(':');

                if (parts.Length == 7)
                {
                    int playerId = int.Parse(parts[0]);
                    string playerName = parts[1];
                    int x = int.Parse(parts[3]);
                    int y = int.Parse(parts[4]);
                    Player? playerInfo = playerInfoList.Find(p => p.GetId() == playerId);
                    if (playerInfo == null)
                    {
                        Exception exception = new("PlayerInfo is null");
                        throw exception;
                    }
                    playerInfo.SetCurrentPosition(x, y);

                    Dispatcher.Invoke(() =>
                    {
                        PlayerVisual playerVisual = playerVisuals[playerInfo];

                        Canvas.SetLeft(playerVisual, playerInfo.GetCurrentX());
                        Canvas.SetTop(playerVisual, playerInfo.GetCurrentY());

                        if (playerName == "DEAD")
                        {
                            HandlePlayerDeath(playerInfo);
                        }
                    });
                }
            }));
        }

        private void GetMessage()
        {
            connection.On<string, string>("MessageSent", ((name, message) =>
            {
                MessageGot?.Invoke(this, $"{name}: {message}");
            }));
        }

        private async void ChatWindow_SendMessage(object? sender, string message)
        {
            await connection.SendAsync("SendPlayerMessage", currentPlayer.GetId(), message);
        }

        private async void CheckElapsedTimeMove(object? state)
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

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (gamestarted && currentPlayer != null)
            {
                // Handle arrow key presses here and update the player's position
                // based on the arrow key input.
                bool execute = true;
                switch (e.Key)
                {
                    case Key.Left:
                        controller.SetCommand(new MoveLeft(currentPlayer, obstacles));
                        break;
                    case Key.Right:
                        controller.SetCommand(new MoveRight(currentPlayer, obstacles));
                        break;
                    case Key.Up:
                        controller.SetCommand(new MoveUp(currentPlayer, obstacles));
                        break;
                    case Key.Down:
                        controller.SetCommand(new MoveDown(currentPlayer, obstacles));
                        break;
                    case Key.Z:
                        controller.Undo();
                        execute = false;
                        break;
                    case Key.Space:
                        controller.SetCommand(new ShootCommand(currentPlayer, this));
                        break;
                    default:
                        execute = false;
                        break;
                }
                
                if (execute)
                {
                    controller.Execute();
                }

                Move(currentPlayer.GetCurrentX(), currentPlayer.GetCurrentY());
                UpdateTextLabelPosition();
                HandleEnemyCollisions(playerVisuals[currentPlayer]);
                HandleCoinsCollisions(playerVisuals[currentPlayer]);
                HandleShieldsCollisions(playerVisuals[currentPlayer]);
                HandleSpeedBoostsCollisions(playerVisuals[currentPlayer]);
                HandleHealthBoostsCollisions(playerVisuals[currentPlayer]);
            }
        }

        private static void UpdatePlayer(PlayerVisual playerVisual, double moveX, double moveY)
        {
            Canvas.SetLeft(playerVisual, moveX);
            Canvas.SetTop(playerVisual, moveY);
        }

        private async void Move(double newX, double newY)
        {
            UpdatePlayer(playerVisuals[currentPlayer], newX, newY);
            await connection.SendAsync("SendMove", currentPlayer.GetId(), newX, newY);
        }


        sealed class ShootCommand : Command
        {

            private readonly MainWindow window;
            public ShootCommand(Player player, MainWindow mainWindow) : base(player)
            {
                this.window = mainWindow;
            }

            public override bool Execute()
            {
                window.Shoot();
                return false;
            }

            public override void Undo()
            {
                
            }
        }

        protected void Shoot()
        {
            if (!currentPlayer.IsShooting)
            {
                currentPlayer.SetShooting(true);
                CreateShot(playerVisuals[currentPlayer], currentPlayer.GetDirectionX(), currentPlayer.GetDirectionY(), currentPlayer.GetShotColor(), currentPlayer.GetShotShape());
                Task.Delay(TimeSpan.FromSeconds(1 / currentPlayer.GetAttackSpeed)).ContinueWith(t => currentPlayer.SetShooting(false));
            }
        }

        private void UpdateTextLabelPosition()
        {
            // pastoviai updatinama, kad tekstas sekiotu zaideja
            double playerX = currentPlayer.GetCurrentX();
            double playerY = currentPlayer.GetCurrentY();

            // object paemimo text
            Canvas.SetLeft(testLabel, playerX);
            Canvas.SetTop(testLabel, playerY - 30);
            // sirdele -10 i desine +10 i kaire
            Canvas.SetLeft(heart2Label, playerX - 20);
            Canvas.SetTop(heart2Label, playerY + 45);
            // shield
            Canvas.SetLeft(shieldBorder, playerX);
            Canvas.SetTop(shieldBorder, playerY);
        }



        private void HideDisplay()
        {
            // tekstas dingsta po puse sekundes
            var timerT = new DispatcherTimer();
            timerT.Interval = TimeSpan.FromSeconds(0.5);
            timerT.Tick += (sender, args) =>
            {
                testLabel.Text = "";
                timerT.Stop();
            };

            timerT.Start();
        }
        private static void StopSpeed()
        {
            // tekstas dingsta po puse sekundes
            var timerT = new DispatcherTimer();
            timerT.Interval = TimeSpan.FromSeconds(10);
            timerT.Tick += (sender, args) =>
            {
                GameStats gameStat = GameStats.Instance;
                gameStat.PlayerSpeed = 10;
                timerT.Stop();
            };

            timerT.Start();
        }
        private void HideShieldDisplay()
        {
            // tekstas dingsta po 10 sekundziu
            var timerT = new DispatcherTimer();
            timerT.Interval = TimeSpan.FromSeconds(10);
            timerT.Tick += (sender, args) =>
            {
                if (true)
                {
                    GameStats gameStat = GameStats.Instance;
                    shieldBorder.Visibility = Visibility.Hidden;
                    gameStat.ShieldOn = false;
                    timerT.Stop();
                }
            };

            timerT.Start();
        }

        public static bool CheckCollision(Coordinates coords1, double width1, double height1, Coordinates coords2, double width2, double height2)
        {
            return coords1.x + width1 >= coords2.x && coords1.x <= coords2.x + width2 && coords1.y + height1 >= coords2.y && coords1.y <= coords2.y + height2;
        }

#pragma warning disable S3168 // "async" methods should not return "void"
        private async void HandleEnemyCollisions(PlayerVisual playerVisual)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);
            foreach (var enemy in from Enemy enemy in enemies
                                  where enemyVisuals.ContainsKey(enemy)
                                  let enemyRect = enemyVisuals[enemy]
                                  let enemyX = Canvas.GetLeft(enemyRect)
                                  let enemyY = Canvas.GetTop(enemyRect)
                                  where CheckCollision(new Coordinates(playerX, playerY), playerVisual.Width, playerVisual.Height,
                                                                   new Coordinates(enemyX, enemyY), enemyRect.Width, enemyRect.Height)
                                  select enemy)
            {
                await HandleCollision(playerVisual, enemy);
            }
        }
        public async Task HandleCollision(PlayerVisual playerVisual, Enemy enemy)
        {
            GameStats gameStat = GameStats.Instance;
            if (!gameStat.ShieldOn)
            {
                gameStat.PlayerHealth -= 5;
                healthLabel.Text = $"Health: {gameStat.PlayerHealth}";

                HealthDecorator healthObj = new HealthDecorator(currentPlayer);
                healthBar.Width = gameStat.PlayerHealth <= 0
                                    ? healthObj.Display(0, gameStat.ShieldOn).health
                                    : healthObj.Display(gameStat.PlayerHealth, gameStat.ShieldOn).health;

                if (gameStat.PlayerHealth <= 0)
                {
                    HandlePlayerDeath(currentPlayer);
                    await connection.SendAsync("UpdateDeadPlayer", currentPlayer.GetId());
                }
            }

            collisionCheckedEnemies[enemy] = true;
        }

        public void HandlePlayerDeath(Player player)
        {
            if(player.Equals(currentPlayer))
            {
                gamestarted = false;
                deadLabel.Text = "DEAD!";
                healthLabel.Text = $"Health: {0}";
            }
            
            playerVisuals[player].PlayerName = "DEAD";
            Color shotColor = (Color)ColorConverter.ConvertFromString("black");
            SolidColorBrush solidColorBrush = new SolidColorBrush(shotColor);
            playerVisuals[player].PlayerColor = solidColorBrush;
            playerVisuals[player].UpdateColor(solidColorBrush);
        }


#pragma warning disable S3168 // "async" methods should not return "void"
        private async void HandleCoinsCollisions(PlayerVisual playerVisual)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);
            List<Coin> coinsCopy = new List<Coin>(coins);
            foreach (var (coin, gameStat) in from Coin coin in coinsCopy
                                             where coinVisuals.ContainsKey(coin)
                                             let coinRect = coinVisuals[coin]
                                             let coinX = Canvas.GetLeft(coinRect)
                                             let coinY = Canvas.GetTop(coinRect)
                                             where PlayerTouchesMapObject(playerX, playerY, playerVisual.Height, coinX, coinY, coinRect.Height)
                                             let gameStat = GameStats.Instance
                                             select (coin, gameStat))
            {
                Debug.WriteLine("singleton " + gameStat.GetHashCode());
                coin.Interact(gameStat);
                scoreLabel.Text = $"Score: {gameStat.PlayerScore}";
                Player text = new CoinDecorator(currentPlayer);
                testLabel.Text = text.Display(gameStat.PlayerHealth, gameStat.ShieldOn).text;
                HideDisplay();
                // Convert coin to a JSON string
                string json = JsonConvert.SerializeObject(coin);
                //-----------
                await connection.SendAsync("SendPickedCoin", json);
            }
        }
        
#pragma warning disable S3168 // "async" methods should not return "void"
        private async void HandleShieldsCollisions(PlayerVisual playerVisual)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);
            List<Shield> shieldsCopy = new List<Shield>(shields);
            foreach (var (shield, gameStat, text) in from Shield shield in shieldsCopy
                                                     where shieldVisuals.ContainsKey(shield)
                                                     let shieldRect = shieldVisuals[shield]
                                                     let shieldX = Canvas.GetLeft(shieldRect)
                                                     let shieldY = Canvas.GetTop(shieldRect)
                                                     where PlayerTouchesMapObject(playerX, playerY, playerVisual.Height, shieldX, shieldY, shieldRect.Height)
                                                     let gameStat = GameStats.Instance
                                                     let text = new ShieldItemDecorator(currentPlayer)
                                                     select (shield, gameStat, text))
            {
                testLabel.Text = text.Display(gameStat.PlayerHealth, gameStat.ShieldOn).text;
                ShieldDecorator shieldObj = new ShieldDecorator(currentPlayer);
                bool shieldVisible = shieldObj.Display(gameStat.PlayerHealth, gameStat.ShieldOn).shieldOn;
                if (shieldVisible)
                {
                    shieldBorder.Visibility = Visibility.Visible;
                    gameStat.ShieldOn = true;
                    shield.Interact(gameStat);
                }

                HideDisplay();
                HideShieldDisplay();
                await connection.SendAsync("SendPickedShield", shield.ToString());
            }
        }
#pragma warning disable S3168 // "async" methods should not return "void"
        private async void HandleSpeedBoostsCollisions(PlayerVisual playerVisual)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);
            List<SpeedBoost> speedBoostCopy = new List<SpeedBoost>(speedBoosts);
            foreach (var (speedBoost, gameStat) in from SpeedBoost speedBoost in speedBoostCopy
                                                   where speedBoostsVisuals.ContainsKey(speedBoost)
                                                   let speedBoostRect = speedBoostsVisuals[speedBoost]
                                                   let speedBoostX = Canvas.GetLeft(speedBoostRect)
                                                   let speedBoostY = Canvas.GetTop(speedBoostRect)
                                                   where PlayerTouchesMapObject(playerX, playerY, playerVisual.Height, speedBoostX, speedBoostY, speedBoostRect.Height)
                                                   let gameStat = GameStats.Instance
                                                   where gameStat.PlayerSpeed < 50
                                                   select (speedBoost, gameStat))
            {
                speedBoost.Interact(gameStat);
                Player text = new SpeedDecorator(player: currentPlayer);
                testLabel.Text = text.Display(gameStat.PlayerHealth, gameStat.ShieldOn).text;
                HideDisplay();
                StopSpeed();
                await connection.SendAsync("SendPickedSpeedBoost", speedBoost.ToString());
            }
        }
#pragma warning disable S3168 // "async" methods should not return "void"
        private async void HandleHealthBoostsCollisions(PlayerVisual playerVisual)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            if (isCollidingWithHealthBoost)
            {
                return; // Skip collision handling if already colliding
            }
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);
            List<HealthBoost> healthBoostCopy = new List<HealthBoost>(healthBoosts);
            foreach (var (healthBoost, healthBoostRect) in from HealthBoost healthBoost in healthBoostCopy
                                                           where healthBoostsVisuals.ContainsKey(healthBoost)
                                                           let healthBoostRect = healthBoostsVisuals[healthBoost]
                                                           let healthBoostX = Canvas.GetLeft(healthBoostRect)
                                                           let healthBoostY = Canvas.GetTop(healthBoostRect)
                                                           where PlayerTouchesMapObject(playerX, playerY, playerVisual.Height, healthBoostX, healthBoostY, healthBoostRect.Height)
                                                           select (healthBoost, healthBoostRect))
            {
                isCollidingWithHealthBoost = true;
                GameStats gameStat = GameStats.Instance;
                Debug.WriteLine("singleton " + gameStat.GetHashCode());
                healthBoost.Interact(gameStat);
                if (gameStat.PlayerHealth > 100)
                {
                    gameStat.PlayerHealth = 100;
                }

                healthLabel.Text = $"Health: {gameStat.PlayerHealth}";
                healthBoosts.Remove(healthBoost);
                healthBoostsVisuals.Remove(healthBoost);
                HealthBoostContainer.Children.Remove(healthBoostRect);
                Player health = new HealthBoostDecorator(currentPlayer);
                testLabel.Text = health.Display(gameStat.PlayerHealth, gameStat.ShieldOn).text;
                HideDisplay();
                health = new HealthDecorator(currentPlayer);
                healthBar.Width = health.Display(gameStat.PlayerHealth, gameStat.ShieldOn).health;
                await connection.SendAsync("SendPickedHealthBoost", healthBoost.ToString());
                isCollidingWithHealthBoost = false;
            }
        }
 
#pragma warning disable S3168 // "async" methods should not return "void"
        public async void CreateShot(PlayerVisual playerVisual, double directionX, double directionY, string color, string shape)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            bool CountKills = false;
            if (playerVisual == playerVisuals[currentPlayer])
            {
                await connection.SendAsync("ShotFired", currentPlayer.GetId(), directionX, directionY);
                CountKills = true;
            }
            lock (enemyListLock)
            {
                Dispatcher.Invoke(() =>
                {
                    Shot shot;
                    double playerX = Canvas.GetLeft(playerVisual);
                    double playerY = Canvas.GetTop(playerVisual);
                    double playerWidth = playerVisual.Width;
                    double playerHeight = playerVisual.Height;
                    SingleShot(playerX, playerY, playerWidth, playerHeight, color, shape, out shot);
                    ShotVisual shotVisual = shotVisualBuilder.New()
                                .SetColor($"{color},{shape}")
                                .SetSize(shot.GetSize())
                                .SetPosition(shot.GetX(), shot.GetY())
                                .Build();

                    // Add the shot to the ShotContainer (Canvas)
                    ShotContainer.Children.Add(shotVisual);

                    // Update the shot's position based on the direction and speed
                    bool shouldRender = true;

                    CompositionTarget.Rendering += async (sender, e) =>
                    {
                        if (!shouldRender)
                        {
                            return;
                        }
                        double currentX = Canvas.GetLeft(shotVisual);
                        double currentY = Canvas.GetTop(shotVisual);
                        double delta = shot.DeltaTime;
                        double newX = currentX + directionX * shot.GetSpeed() * delta;
                        double newY = currentY + directionY * shot.GetSpeed() * delta;
                        // Check for collisions with obstacles
                        // TO-DO: shotvisual does not set width and height for some reason... will fix in future maybe
                        foreach (Obstacle obstacle in obstacles)
                        {
                            double elipse = shotVisual.EllipseSize;
                            shot = RemoveShot(shot, newX, newY, obstacle, elipse);
                            if (shot == null) //obstacle.WouldOverlap(newX, newY, shotVisual.EllipseSize, shotVisual.EllipseSize)
                            {
                                // Remove the shot and break out of the loop
                                ShotContainer.Children.Remove(shotVisual);
                                shouldRender = false;
                                return;
                            }
                        }


                        List<Enemy> enemiesToRemove = new List<Enemy>(); // Create a list to store enemies to be removed

                        bool shotHitEnemy = false;
                        foreach (var (enemy, enemyRect) in from Enemy enemy in enemies
                                                           where enemyVisuals.ContainsKey(enemy)
                                                           let enemyRect = enemyVisuals[enemy]
                                                           let enemyX = Canvas.GetLeft(enemyRect)
                                                           let enemyY = Canvas.GetTop(enemyRect)
                                                           where newX + shotVisual.EllipseSize >= enemyX &&
                                                            newX <= enemyX + enemyRect.Width &&
                                                            newY + shotVisual.EllipseSize >= enemyY &&
                                                            newY <= enemyY + enemyRect.Height
                                                           select (enemy, enemyRect))
                        {
                            await HandleCountKill(CountKills, shot, enemiesToRemove, enemy, enemyRect);

                            ShotContainer.Children.Remove(shotVisual);
                            shotHitEnemy = true;
                            shouldRender = false;
                            break;
                        }

                        shouldRender = HandleEnemyHits(shotVisual, shouldRender, newX, newY, enemiesToRemove, shotHitEnemy);
                    };

                });
            }
        }

        private bool HandleEnemyHits(ShotVisual shotVisual, bool shouldRender, double newX, double newY, List<Enemy> enemiesToRemove, bool shotHitEnemy)
        {
            foreach (Enemy enemyToRemove in enemiesToRemove)
            {
                enemies.Remove(enemyToRemove);
            }

            // Update the shot's position
            if (!shotHitEnemy)
            {
                shouldRender = UpdateShotPosition(shotVisual, shouldRender, newX, newY);
            }

            return shouldRender;
        }

        private async Task HandleCountKill(bool CountKills, Shot shot, List<Enemy> enemiesToRemove, Enemy enemy, EnemyVisual? enemyRect)
        {
            if (CountKills)
            {
                GameStats gameStat = GameStats.Instance;
                enemy.SetHealth((int)(enemy.GetHealth() - shot.GetPoints()));  // Reduce the enemy's health
                gameStat.PlayerScore += 5;
                scoreLabel.Text = $"Score: {gameStat.PlayerScore}";
                await connection.SendAsync("SendEnemyUpdate", enemy.ToString());
                if (enemy.GetHealth() <= 0)
                {
                    enemiesToRemove.Add(enemy); // Add the enemy to the removal list
                    enemyVisuals.Remove(enemy);
                    EnemyContainer.Children.Remove(enemyRect);
                    await connection.SendAsync("SendDeadEnemy", enemy.ToString());
                }
            }
        }

        private bool UpdateShotPosition(ShotVisual shotVisual, bool shouldRender, double newX, double newY)
        {
            Canvas.SetLeft(shotVisual, newX);
            Canvas.SetTop(shotVisual, newY);

            // Remove the shot if it goes out of bounds
            if (newX < 0 || newX >= ShotContainer.ActualWidth || newY < 0 || newY >= ShotContainer.ActualHeight)
            {
                ShotContainer.Children.Remove(shotVisual);
                shouldRender = false;
            }

            return shouldRender;
        }

        public static Shot RemoveShot(Shot shot, double newX, double newY, Obstacle obstacle, double elipse)
        {
            if (obstacle.WouldOverlap(newX, newY, elipse, elipse))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                shot = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
#pragma warning disable CS8603 // Possible null reference return.
            return shot;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public static void SingleShot(double playerX, double playerY, double playerWidth, double playerHeight, string color, string shape, out Shot shot)
        {
            // choose shot color
            IColor shotColor;
            if (color == "red")
            {
                shotColor = new RedColor();
            }
            else
            {
                shotColor = new BlueColor();
            }

            Shot localShot = new Shot(shotColor, 5, 10, 5);

            if (shape == "triangle")
            {
                localShot = new TriangleShot(localShot);
            }
            else
            {
                localShot = new RoundShot(localShot);
            }     

            double playerCenterX = playerX + playerWidth / 2;
            double playerCenterY = playerY + playerHeight / 2;

            localShot.SetPosition(playerCenterX - localShot.GetSize() / 2, playerCenterY - localShot.GetSize() / 2);
            shot = localShot;
        }

        public static bool PlayerTouchesMapObject(double playerX, double playerY, double playerSize, double objectX, double objectY, double objectSize)
        {           
            if (playerX + playerSize >= objectX &&  playerX <= objectX + objectSize &&  playerY + playerSize >= objectY && playerY <= objectY + objectSize) return true;
            else return false;
        }
    }
}
