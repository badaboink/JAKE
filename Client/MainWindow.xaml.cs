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
using JAKE.classlibrary.Patterns;
using JAKE.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace JAKE.client
{
    public partial class MainWindow : Window
    {
        private Player currentPlayer;
        private bool gamestarted = false;
        private List<Player> playerInfoList = new List<Player>();
        private Dictionary<Player, PlayerVisual> playerVisuals = new Dictionary<Player, PlayerVisual>();
        private Dictionary<Enemy, EnemyVisual> enemyVisuals = new Dictionary<Enemy, EnemyVisual>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private List<Shot> shots = new List<Shot>();
        private Microsoft.AspNetCore.SignalR.Client.HubConnection connection;
        private DateTime lastGameTime = DateTime.MinValue;
        private GameStats gameStat = GameStats.Instance;
        private Dictionary<Enemy, bool> collisionCheckedEnemies = new Dictionary<Enemy, bool>();
        private List<Coin> coins = new List<Coin>();
        private Dictionary<Coin, CoinVisual> coinVisuals = new Dictionary<Coin, CoinVisual>();
        private List<HealthBoost> healthBoosts = new List<HealthBoost>();
        private Dictionary<HealthBoost, HealthBoostVisual> healthBoostsVisuals = new Dictionary<HealthBoost, HealthBoostVisual>();
        private List<Shield> shields = new List<Shield>(); 
        private Dictionary<Shield, ShieldVisual> shieldVisuals = new Dictionary<Shield, ShieldVisual>();
        private List<SpeedBoost> speedBoosts = new List<SpeedBoost>();
        private Dictionary<SpeedBoost, SpeedBoostVisual> speedBoostsVisuals = new Dictionary<SpeedBoost, SpeedBoostVisual>();
        private List<Weapon> weapons = new List<Weapon>();
        private readonly object enemyListLock = new object();
        //private Dictionary<Weapon, WeaponVisual> weaponVisuals = new Dictionary<Weapon, WeaponVisual>();


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

            foreach (Enemy enemy in enemies)
            {
                collisionCheckedEnemies[enemy] = false;
            }
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
            connection.On<int, string, string, string>("GameStart", (id, name, color, obstacleData) =>
            {
                currentPlayer = new Player(id, name, color);
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

                    if (parts.Length == 5)
                    {
                        int playerId = int.Parse(parts[0]);
                        string playerName = parts[1];
                        string playerColor = parts[2];
                        int x = int.Parse(parts[3]);
                        int y = int.Parse(parts[4]);
                        Player playerInfo = new Player(playerId, playerName, playerColor);
                        playerInfo.SetCurrentPosition(x, y);

                        if (!playerInfoList.Contains(playerInfo))
                        {
                            playerInfo.SetCurrentPosition(x, y);
                            playerInfoList.Add(playerInfo);
                            Dispatcher.Invoke(() =>
                            {
                                PlayerVisual playerVisual = new PlayerVisual();
                                ColorConverter converter = new ColorConverter();
                                Color playerColor = (Color)ColorConverter.ConvertFromString(playerInfo.GetColor());
                                SolidColorBrush solidColorBrush = new SolidColorBrush(playerColor);
                                playerVisual.PlayerName = playerInfo.GetName();
                                playerVisual.PlayerColor = solidColorBrush;
                                playerVisual.UpdateColor(solidColorBrush);

                                playerVisuals[playerInfo] = playerVisual;
                                Canvas.SetLeft(playerVisual, x);
                                Canvas.SetTop(playerVisual, y);
                                playersContainer.Items.Add(playerVisual);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                PlayerVisual playerVisual = playerVisuals[playerInfo];
                                playerInfo.SetCurrentPosition(x, y);
                                Canvas.SetLeft(playerVisual, x);
                                Canvas.SetTop(playerVisual, y);
                            });
                        }
                    }
                }
                gameStat.PlayersCount = playerInfoList.Count;  //singleton
                lastGameTime = gametime;
            });
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
                        PlayerVisual playerVisual = playerVisuals[playerInfo];
                        Canvas.SetLeft(playerVisual, playerInfo.GetCurrentX());
                        Canvas.SetTop(playerVisual, playerInfo.GetCurrentY());

                        playerVisual.PlayerName = playerName;
                        Color shotColor = (Color)ColorConverter.ConvertFromString(playerColor);
                        SolidColorBrush solidColorBrush = new SolidColorBrush(shotColor);
                        playerVisual.PlayerColor = solidColorBrush;
                        playerVisual.UpdateColor(solidColorBrush);

                    });
                }
            });
            timer = new Timer(CheckElapsedTimeMove, null, 0, 1000);

            connection.On<List<string>>("SendingEnemies", (enemydata) =>
            {
                foreach(string enemystring in enemydata)
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
                            lock(enemyListLock)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    Debug.WriteLine("enemy spalva: " + enemy.GetColor());
                                    EnemyVisual enemyVisual = new EnemyVisual();
                                    ColorConverter converter = new ColorConverter();
                                    Color enemyColor = (Color)ColorConverter.ConvertFromString(enemy.GetColor());

                                    SolidColorBrush solidColorBrush = new SolidColorBrush(enemyColor);
                                    enemyVisual.FillColor = solidColorBrush;
                                    enemyVisual.EllipseSize = enemy.GetSize();
                                    enemyVisual.UpdateEnemy(solidColorBrush);

                                    enemyVisuals[enemy] = enemyVisual;
                                    Canvas.SetLeft(enemyVisual, enemyX);
                                    Canvas.SetTop(enemyVisual, enemyY);
                                    EnemyContainer.Children.Add(enemyVisual);

                                    HandleEnemyCollisions(playerVisuals[currentPlayer]);
                                });
                            }
                        }
                        else
                        {
                            lock(enemyListLock)
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
            connection.On<int, double, double>("UpdateShotsFired", (playerid, X, Y) =>
            {
                Player playerToUpdate = playerInfoList.FirstOrDefault(player => player.MatchesId(playerid));
                if (playerToUpdate != null)
                {
                    CreateShot(playerVisuals[playerToUpdate], X, Y);
                }
            });
            connection.On<int, string>("UpdateDeadEnemy", (enemyid, enemycolor) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Enemy enemy = new Enemy(enemyid, enemycolor);
                    EnemyVisual enemyRect = enemyVisuals[enemy];
                    enemies.Remove(enemy);
                    enemyVisuals.Remove(enemy);
                    EnemyContainer.Children.Remove(enemyRect);
                });
            });
            connection.On<int, string, int>("UpdateEnemyHealth", (enemyid, enemycolor, enemyhealth) =>
            {
                Enemy enemyToUpdate = enemies.FirstOrDefault(enemy => enemy.MatchesId(enemyid));
                if (enemyToUpdate != null)
                {
                    enemyToUpdate.SetHealth(enemyhealth);
                }
            });
            connection.On<string>("DisconnectedPlayer", (player) =>
            {
                string[] parts = player.Split(':');
                if (parts.Length == 5)
                {
                    int playerId = int.Parse(parts[0]);
                    string playerName = parts[1];
                    string playerColor = parts[2];
                    int x = int.Parse(parts[3]);
                    int y = int.Parse(parts[4]);
                    Player playerToDelete = new Player(playerId, playerName, playerColor);
                    Dispatcher.Invoke(() =>
                    {
                        PlayerVisual playerVisual = playerVisuals[playerToDelete];
                        playerVisuals.Remove(playerToDelete);
                        playerInfoList.Remove(playerToDelete);
                        playersContainer.Items.Remove(playerVisual);
                    });
                }
            });
            connection.On<List<string>>("SendingCoins", (coinsdata) =>
            {
                //Debug.WriteLine("sendingcoins jakeclient");
                //Debug.WriteLine("CoinContainer Width: " + CoinContainer.ActualWidth);
                //Debug.WriteLine("CoinContainer Height: " + CoinContainer.ActualHeight);
                //Debug.WriteLine("CoinContainer Visibility: " + CoinContainer.Visibility);
                foreach (string coinstring in coinsdata)
                {
                    Debug.WriteLine("coin: " + coinstring);
                    string[] parts = coinstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int coinId = int.Parse(parts[0]);
                        double coinX = double.Parse(parts[1]);
                        double coinY = double.Parse(parts[2]);
                        int coinWidth = int.Parse(parts[3]);
                        int coinHeight = int.Parse(parts[4]);
                        int points = int.Parse(parts[5]);
                        Coin coin = new Coin(coinId, coinWidth, coinHeight);
                        coin.Image = "./coin.png";
                        if (!coins.Contains(coin))
                        {
                            coin.SetPosition(coinX, coinY);
                            coins.Add(coin);
                            Dispatcher.Invoke(() =>
                            {
                                Debug.WriteLine("kuria nauja grazu coin");
                                CoinVisual coinVisual = new CoinVisual();


                                coinVisual.EllipseSizeC = 20;
                                Canvas.SetLeft(coinVisual, coinX);
                                Canvas.SetTop(coinVisual, coinY);
                                coinVisuals[coin] = coinVisual;
                                CoinContainer.Children.Add(coinVisual);
                                Debug.WriteLine("First Coin X: " + Canvas.GetLeft(coinVisual));
                                Debug.WriteLine("First Coin Y: " + Canvas.GetTop(coinVisual));

                                //Color coinColor = (Color)ColorConverter.ConvertFromString("red");
                                //SolidColorBrush solidColorBrush = new SolidColorBrush(coinColor);

                                //CoinVisual coinVisual = new CoinVisual();
                                //coinVisual.EllipseSizeC = 20;
                                //coinVisual.FillColorC = solidColorBrush;
                                //Canvas.SetLeft(coinVisual, coinX);
                                //Canvas.SetTop(coinVisual, coinY);

                                //// Add the shot to the ShotContainer (Canvas)
                                //CoinContainer.Children.Add(coinVisual);
                                //if (coinVisual.IsVisible) Debug.WriteLine("visual is visible");
                                //else Debug.WriteLine("NESIMATO");

                                HandleCoinsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                        //else
                        //{
                        //    Dispatcher.Invoke(() =>
                        //    {
                        //        Debug.WriteLine("atvaizduoja jau esama coin");
                        //        CoinVisual coinVisual = coinVisuals[coin];
                        //        coin.SetPoints(points);
                        //        coin.SetPosition(coinX, coinY);
                        //        Canvas.SetLeft(coinVisual, coinX);
                        //        Canvas.SetTop(coinVisual, coinY);

                        //        HandleCoinsCollisions(playerVisuals[currentPlayer]);
                        //    });
                        //}
                    }
                }

            });
            connection.On<int>("SendingPickedCoin", (coinid) =>
            {
                Dispatcher.Invoke(() =>
                {
             
                    Coin coin = new Coin(coinid);
                    CoinVisual coinVisual = coinVisuals[coin];
                    coins.Remove(coin);
                    coinVisuals.Remove(coin);    
                    CoinContainer.Children.Remove(coinVisual);
                    //coins.RemoveAll(coin => coin.id == coinid);
                });
            });
            connection.On<List<string>>("SendingShields", (shieldsdata) =>
            {
                Debug.WriteLine("sendingSHIELDS jakeclient");
                foreach (string shieldstring in shieldsdata)
                {
                    Debug.WriteLine("shield: " + shieldstring);
                    string[] parts = shieldstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int shieldId = int.Parse(parts[0]);
                        double shieldX = double.Parse(parts[1]);
                        double shieldY = double.Parse(parts[2]);
                        int shieldWidth = int.Parse(parts[3]);
                        int shieldHeight = int.Parse(parts[4]);
                        int time = int.Parse(parts[5]);
                        Shield shield = new Shield(shieldId, shieldWidth, shieldHeight);
                        shield.Image = "shield.png";
                        if (!shields.Contains(shield))
                        {
                            shield.SetPosition(shieldX, shieldY);
                            shields.Add(shield);
                            Dispatcher.Invoke(() =>
                            {
                                Debug.WriteLine("kuria nauja grazu shield");
                                ShieldVisual shieldVisual = new ShieldVisual(shield.Image, shield.Width, shield.Height);
                                shieldVisuals[shield] = shieldVisual;
                                Canvas.SetLeft(shieldVisual, shieldX);
                                Canvas.SetTop(shieldVisual, shieldY);
                                ShieldContainer.Children.Add(shieldVisual);
                                Debug.WriteLine("First Shield X: " + Canvas.GetLeft(shieldVisual));
                                Debug.WriteLine("First Shield Y: " + Canvas.GetTop(shieldVisual));
                                HandleShieldsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Debug.WriteLine("atvaizduoja jau esama shield");
                                ShieldVisual shieldVisual = shieldVisuals[shield];
                                shield.SetTime(time);
                                shield.SetPosition(shieldX, shieldY);
                                Canvas.SetLeft(shieldVisual, shieldX);
                                Canvas.SetTop(shieldVisual, shieldY);

                                HandleShieldsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                    }
                }

            });
            connection.On<int>("SendingPickedShield", (shieldid) =>
            {
                Dispatcher.Invoke(() =>
                {

                    Shield shield = new Shield(shieldid);
                    ShieldVisual shieldVisual = shieldVisuals[shield];
                    shields.Remove(shield);
                    shieldVisuals.Remove(shield);
                    ShieldContainer.Children.Remove(shieldVisual);
                });
            });
            connection.On<List<string>>("SendingHealthBoosts", (healthdata) =>
            {
                Debug.WriteLine("sendingHEALTHBOOSTS jakeclient");
                foreach (string healthstring in healthdata)
                {
                    Debug.WriteLine("healthBoost: " + healthstring);
                    string[] parts = healthstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int healthId = int.Parse(parts[0]);
                        double healthX = double.Parse(parts[1]);
                        double healthY = double.Parse(parts[2]);
                        int healthWidth = int.Parse(parts[3]);
                        int healthHeight = int.Parse(parts[4]);
                        int healthVal = int.Parse(parts[5]);
                        HealthBoost health = new HealthBoost(healthId, healthWidth, healthHeight);
                        health.Image = "healthboost.png";
                        if (!healthBoosts.Contains(health))
                        {
                            health.SetPosition(healthX, healthY);
                            healthBoosts.Add(health);
                            Dispatcher.Invoke(() =>
                            {
                                Debug.WriteLine("kuria nauja grazu health");
                                HealthBoostVisual healthVisual = new HealthBoostVisual(health.Image, health.Width, health.Height);
                                healthBoostsVisuals[health] = healthVisual;
                                Canvas.SetLeft(healthVisual, healthX);
                                Canvas.SetTop(healthVisual, healthY);
                                HealthBoostContainer.Children.Add(healthVisual);
                                Debug.WriteLine("First HealthBoost X: " + Canvas.GetLeft(healthVisual));
                                Debug.WriteLine("First HealthBoost Y: " + Canvas.GetTop(healthVisual));
                                HandleHealthBoostsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Debug.WriteLine("atvaizduoja jau esama health");
                                HealthBoostVisual healthVisual = healthBoostsVisuals[health];
                                health.SetHealth(healthVal);
                                health.SetPosition(healthX, healthY);
                                Canvas.SetLeft(healthVisual, healthX);
                                Canvas.SetTop(healthVisual, healthY);

                                HandleHealthBoostsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                    }
                }

            });
            connection.On<int>("SendingPickedHealthBoost", (healthid) =>
            {
                Dispatcher.Invoke(() =>
                {

                    HealthBoost health = new HealthBoost(healthid);
                    HealthBoostVisual healthVisual = healthBoostsVisuals[health];
                    healthBoosts.Remove(health);
                    healthBoostsVisuals.Remove(health);
                    HealthBoostContainer.Children.Remove(healthVisual);
                });
            });
            connection.On<List<string>>("SendingSpeedBoosts", (speeddata) =>
            {
                Debug.WriteLine("sendingSPEEDBOOSTS jakeclient");
                foreach (string speedstring in speeddata)
                {
                    Debug.WriteLine("speedBoost: " + speedstring);
                    string[] parts = speedstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int speedId = int.Parse(parts[0]);
                        double speedX = double.Parse(parts[1]);
                        double speedY = double.Parse(parts[2]);
                        int speedWidth = int.Parse(parts[3]);
                        int speedHeight = int.Parse(parts[4]);
                        int speedVal = int.Parse(parts[5]);
                        int time = int.Parse(parts[6]);
                        SpeedBoost speed = new SpeedBoost(speedId, speedWidth, speedHeight);
                        speed.Image = "speedboost.png";
                        if (!speedBoosts.Contains(speed))
                        {
                            speed.SetPosition(speedX, speedY);
                            speedBoosts.Add(speed);
                            Dispatcher.Invoke(() =>
                            {
                                Debug.WriteLine("kuria nauja grazu speed");
                                SpeedBoostVisual speedVisual = new SpeedBoostVisual(speed.Image, speed.Width, speed.Height);
                                speedBoostsVisuals[speed] = speedVisual;
                                Canvas.SetLeft(speedVisual, speedX);
                                Canvas.SetTop(speedVisual, speedY);
                                SpeedBoostContainer.Children.Add(speedVisual);
                                Debug.WriteLine("First SpeedBoost X: " + Canvas.GetLeft(speedVisual));
                                Debug.WriteLine("First SpeedBoost Y: " + Canvas.GetTop(speedVisual));
                                HandleSpeedBoostsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Debug.WriteLine("atvaizduoja jau esama speed");
                                SpeedBoostVisual speedVisual = speedBoostsVisuals[speed];
                                speed.SetSpeedTime(speedVal, time); 
                                speed.SetPosition(speedX, speedY);
                                Canvas.SetLeft(speedVisual, speedX);
                                Canvas.SetTop(speedVisual, speedY);

                                HandleSpeedBoostsCollisions(playerVisuals[currentPlayer]);
                            });
                        }
                    }
                }

            });
            connection.On<int>("SendingPickedSpeedBoost", (speedid) =>
            {
                Dispatcher.Invoke(() =>
                {

                    SpeedBoost speed = new SpeedBoost(speedid);
                    SpeedBoostVisual speedVisual = speedBoostsVisuals[speed];
                    speedBoosts.Remove(speed);
                    speedBoostsVisuals.Remove(speed);
                    SpeedBoostContainer.Children.Remove(speedVisual);
                });
            });
        }
        private async void CheckElapsedTimeMove(object state)
        {
            await connection.SendAsync("SendEnemies");
           // await connection.SendAsync("SendCoins");
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

        private int playerDirectionX = 0;
        private int playerDirectionY = 0;
        //private int score = 0;
        //private int health = 100;
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (gamestarted)
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
                        if (distance != 0)
                        {
                            newX = playerDirectionX == 0 ? playerCurrentX : playerCurrentX + distance;
                            newY = playerDirectionY == 0 ? playerCurrentY : playerCurrentY + distance;

                            Move(newX, newY);
                        }
                        break;
                    }
                }
                double minX = 0; // Minimum X-coordinate
                double minY = 0; // Minimum Y-coordinate
                double windowWidth = this.ActualWidth;
                double windowHeight = this.ActualHeight;
                double maxX = windowWidth - 60; // Maximum X-coordinate
                double maxY = windowHeight - 80; // Maximum Y-coordinate

                if (newX >= minX && newX <= maxX && newY >= minY && newY <= maxY && !overlap)
                {
                    Move(newX, newY);
                }

                HandleEnemyCollisions(playerVisuals[currentPlayer]);
            }
        }

        private async void HandleEnemyCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);

            foreach (Enemy enemy in enemies)
            {
                if (enemyVisuals.ContainsKey(enemy))
                {
                    EnemyVisual enemyRect = enemyVisuals[enemy];
                    double enemyX = Canvas.GetLeft(enemyRect);
                    double enemyY = Canvas.GetTop(enemyRect);

                    if (playerX + playerVisual.Width >= enemyX &&
                        playerX <= enemyX + enemyRect.Width &&
                        playerY + playerVisual.Height >= enemyY &&
                        playerY <= enemyY + enemyRect.Height)
                    {
                        gameStat.PlayerHealth -= 5;
                        //health -= 5;
                        healthLabel.Text = $"Health: {gameStat.PlayerHealth}";

                        collisionCheckedEnemies[enemy] = true;

                        if(gameStat.PlayerHealth <= 0)
                        {
                            gamestarted = false;
                            deadLabel.Text = "DEAD!";
                            healthLabel.Text = $"Health: {0}";
                            //currentPlayer.SetName("DEAD B****");
                            playerVisuals[currentPlayer].PlayerName = "DEAD";
                            Color shotColor = (Color)ColorConverter.ConvertFromString("black");
                            SolidColorBrush solidColorBrush = new SolidColorBrush(shotColor);
                            playerVisuals[currentPlayer].PlayerColor = solidColorBrush;
                            playerVisuals[currentPlayer].UpdateColor(solidColorBrush);
                            await connection.SendAsync("UpdateDeadPlayer", currentPlayer.GetId());
                        }
                    }
                }
            }
        }

        private async void HandleCoinsCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);

            foreach (Coin coin in coins)
            {
                if (coinVisuals.ContainsKey(coin))
                {
                    CoinVisual coinRect = coinVisuals[coin];
                    double coinX = Canvas.GetLeft(coinRect);
                    double coinY = Canvas.GetTop(coinRect);

                    if (playerX + playerVisual.Width >= coinX &&
                        playerX <= coinX + coinRect.Width &&
                        playerY + playerVisual.Height >= coinY &&
                        playerY <= coinY + coinRect.Height)
                    {
                   
                        // coin.Interact(playerVisual); //TODO: paduot player atrinkta pagal player visual ir tada viduj interact is gamestats pasiimt score ir pridet
                        //arba - tieisog padidint score??? arba saugot ir prie klases points ir gamestats? 
                        gameStat.PlayerScore += coin.Points;

                        //collisionCheckedCoins[coin] = true;

                        await connection.SendAsync("SendPickedCoin", coin.ToString());
                        
                    }
                }
            }
        }
        private async void HandleShieldsCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);

            foreach (Shield shield in shields)
            {
                if (shieldVisuals.ContainsKey(shield))
                {
                    ShieldVisual shieldRect = shieldVisuals[shield];
                    double shieldX = Canvas.GetLeft(shieldRect);
                    double shieldY = Canvas.GetTop(shieldRect);

                    if (playerX + playerVisual.Width >= shieldX &&
                        playerX <= shieldX + shieldRect.Width &&
                        playerY + playerVisual.Height >= shieldY &&
                        playerY <= shieldY + shieldRect.Height)
                    {

                        //TODO: jei paliecia priesas nenusiima gyvybe - veliavele kazkokia??? ir timeri uzdet
                        //pakeist player visual kazkaip - borderi koki uzdet

                        await connection.SendAsync("SendPickedShield", shield.ToString());

                    }
                }
            }
        }
        private async void HandleSpeedBoostsCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);

            foreach (SpeedBoost speedBoost in speedBoosts)
            {
                if (speedBoostsVisuals.ContainsKey(speedBoost))
                {
                    SpeedBoostVisual speedBoostRect = speedBoostsVisuals[speedBoost];
                    double speedBoostX = Canvas.GetLeft(speedBoostRect);
                    double speedBoostY = Canvas.GetTop(speedBoostRect);

                    if (playerX + playerVisual.Width >= speedBoostX &&
                        playerX <= speedBoostX + speedBoostRect.Width &&
                        playerY + playerVisual.Height >= speedBoostY &&
                        playerY <= speedBoostY + speedBoostRect.Height)
                    {

                      //TODO: kur speed saugosi?
                        //speedBoost.Interact(player); // padidint speed pagal reiksme

                        await connection.SendAsync("SendPickedSpeedBoost", speedBoost.ToString());

                    }
                }
            }
        }
        private async void HandleHealthBoostsCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);

            foreach (HealthBoost healthBoost in healthBoosts)
            {
                if (healthBoostsVisuals.ContainsKey(healthBoost))
                {
                    HealthBoostVisual healthBoostRect = healthBoostsVisuals[healthBoost];
                    double healthBoostX = Canvas.GetLeft(healthBoostRect);
                    double healthBoostY = Canvas.GetTop(healthBoostRect);

                    if (playerX + playerVisual.Width >= healthBoostX &&
                        playerX <= healthBoostX + healthBoostRect.Width &&
                        playerY + playerVisual.Height >= healthBoostY &&
                        playerY <= healthBoostY + healthBoostRect.Height)
                    {

                        gameStat.PlayerHealth += healthBoost.Health;

                        await connection.SendAsync("SendPickedhealthBoost", healthBoost.ToString());

                    }
                }
            }
        }

        private async void Move(double newX, double newY)
        {
            UpdatePlayer(playerVisuals[currentPlayer], newX, newY);
            await connection.SendAsync("SendMove", currentPlayer.GetId(), newX, newY);
        }

        private void Shoot(int deltaX, int deltaY)
        {
            CreateShot(playerVisuals[currentPlayer], deltaX, deltaY);
        }

        public async void CreateShot(PlayerVisual playerVisual, double directionX, double directionY)
        {
            bool CountKills = false;
            if (playerVisual == playerVisuals[currentPlayer])
            {
                await connection.SendAsync("ShotFired", currentPlayer.GetId(), directionX, directionY);
                CountKills = true;
            }
            lock(enemyListLock)
            {
                Dispatcher.Invoke(() =>
                {
                    // Create a new shot visual element (e.g., a bullet or projectile)
                    ShotVisual shotVisual;
                    Shot shot;
                    SolidColorBrush solidColorBrush;

                    double playerX = Canvas.GetLeft(playerVisual);
                    double playerY = Canvas.GetTop(playerVisual);
                    double playerWidth = playerVisual.Width;
                    double playerHeight = playerVisual.Height;
                    string playerColor = playerVisual.PlayerColor.ToString();
                    

                    SingleShot(playerX, playerY, playerWidth, playerHeight, playerColor, out shot);


                    Color shotColor = (Color)ColorConverter.ConvertFromString(shot.getColor());
                    solidColorBrush = new SolidColorBrush(shotColor);

                    shotVisual = new ShotVisual();
                    shotVisual.EllipseSize = shot.getSize();
                    shotVisual.FillColor = solidColorBrush;
                    shotVisual.UpdateShot(solidColorBrush);

                    // Set the initial position of the shot at the center of the player
                    Canvas.SetLeft(shotVisual, shot.getX());
                    Canvas.SetTop(shotVisual, shot.getY());

                    // Add the shot to the ShotContainer (Canvas)
                    ShotContainer.Children.Add(shotVisual);

                    // Update the shot's position based on the direction and speed
                    bool shouldRender = true;


                    CompositionTarget.Rendering += async (sender, e) =>
                    {
                        if (!shouldRender) return;
                        double currentX = Canvas.GetLeft(shotVisual);
                        double currentY = Canvas.GetTop(shotVisual);

                        double newX = currentX + directionX * shot.getSpeed();
                        double newY = currentY + directionY * shot.getSpeed();

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
                        bool overlapWithEnemy = false;

                        foreach (Enemy enemy in enemies)
                        {
                            if (enemyVisuals.ContainsKey(enemy))
                            {
                                EnemyVisual enemyRect = enemyVisuals[enemy];
                                double enemyX = Canvas.GetLeft(enemyRect);
                                double enemyY = Canvas.GetTop(enemyRect);

                                if (newX + shotVisual.EllipseSize >= enemyX &&
                                    newX <= enemyX + enemyRect.Width &&
                                    newY + shotVisual.EllipseSize >= enemyY &&
                                    newY <= enemyY + enemyRect.Height)
                                {

                                    if (CountKills)
                                    {
                                        enemy.SetHealth((int)(enemy.GetHealth() - shot.getPoints()));  // Reduce the enemy's health
                                        gameStat.PlayerScore += 5;
                                        Debug.WriteLine("score: " + gameStat.PlayerScore);
                                        scoreLabel.Text = $"Score: {gameStat.PlayerScore}";
                                        Debug.WriteLine("pataike i enemy");
                                        await connection.SendAsync("SendEnemyUpdate", enemy.ToString());
                                        if (enemy.GetHealth() <= 0)
                                        {
                                            enemiesToRemove.Add(enemy); // Add the enemy to the removal list
                                            enemyVisuals.Remove(enemy);
                                            EnemyContainer.Children.Remove(enemyRect);
                                            Debug.WriteLine("mire enemy");
                                            await connection.SendAsync("SendDeadEnemy", enemy.ToString());
                                        }
                                    }

                                    ShotContainer.Children.Remove(shotVisual);
                                    shotHitEnemy = true;
                                    shouldRender = false;
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

                });
            }
        }

        public static Shot RemoveShot(Shot shot, double newX, double newY, Obstacle obstacle, double elipse)
        {
            if (obstacle.WouldOverlap(newX, newY, elipse, elipse))
            {
                shot = null;
            }
            return shot;
        }

        public static void SingleShot(double playerX, double playerY, double playerWidth, double playerHeight, string playerColor, out Shot shot)
        {
            Shot localShot;

            double playerCenterX = playerX + playerWidth / 2;
            double playerCenterY = playerY + playerHeight / 2;

            switch (playerColor)
            {
                case "#FF008000":
                    localShot = new GreenShot(new Shot());
                    break;
                case "#FFFF0000":
                    localShot = new RedShot(new Shot());
                    break;
                case "#FF0000FF":
                    localShot = new BlueShot(new Shot());
                    break;
                default:
                    localShot = new BlueShot(new Shot());   // defaultu padarys melyna shot
                    break;
            }

            localShot.setPosition(playerCenterX - localShot.getSize() / 2, playerCenterY - localShot.getSize() / 2);
            shot = localShot;
        }

        private void UpdatePlayer(PlayerVisual playerVisual, double moveX, double moveY)
        {
            Canvas.SetLeft(playerVisual, moveX);
            Canvas.SetTop(playerVisual, moveY);

            currentPlayer.SetCurrentPosition(moveX, moveY);
        }

        /*
         
           MapObjectFactory objectFactory = new MapObjectFactory();
        
        IMapObject healthBoost = objectFactory.CreateMapObject("healthboost");
        IMapObject coin = objectFactory.CreateMapObject("coin");
        IMapObject weapon = objectFactory.CreateMapObject("weapon");
        IMapObject shield = objectFactory.CreateMapObject("shield");
        IMapObject speedBoost = objectFactory.CreateMapObject("speedboost");
        
        Player player = new Player(); 
        
        healthBoost.Interact(player);
        coin.Interact(player);
        weapon.Interact(player);
        shield.Interact(player);
        speedBoost.Interact(player);
        */
    }
}
