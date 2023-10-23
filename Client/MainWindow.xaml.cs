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
using JAKE.client.Visuals;
using JAKE.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

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
        private bool shieldOn = false;
        //private Dictionary<Weapon, WeaponVisual> weaponVisuals = new Dictionary<Weapon, WeaponVisual>();
        private static List<Zombie> miniZombieList = new List<Zombie>();
        private Dictionary<Zombie, ZombiesVisual> zombieVisuals = new Dictionary<Zombie, ZombiesVisual>();
        private BossZombie boss = new BossZombie("",0,0,0,0, miniZombieList);
        ZombiesVisual bossVisual = new ZombiesVisual();


        //TODO: zombie visual, main iskvietimai, gavimai responses, judejimas, logika ar veikia
        private bool isCollidingWithHealthBoost = false;


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
        IBuilderVisual<PlayerVisual> playerVisualBuilder = new PlayerVisualBuilder();
        IBuilderVisual<EnemyVisual> enemyVisualBuilder = new EnemyVisualBuilder();
        private async Task GameStart()
        {            
            // Create and show the ColorChoiceForm as a pop-up
            ColorChoiceForm colorChoiceForm = new ColorChoiceForm();
            colorChoiceForm.ShowDialog();
            string selectedColor = colorChoiceForm.SelectedColor;
            string name = colorChoiceForm.Name;
            string shotColor = colorChoiceForm.ShotColor;
            string shotShape = colorChoiceForm.ShotShape;

            await connection.SendAsync("SendColor", selectedColor, name, shotColor, shotShape);
            connection.On<int, string, string, string>("GameStart", (id, name, color, obstacleData) =>
            {
                currentPlayer = new Player(id, name, color, shotColor, shotShape);
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
                Debug.WriteLine("gameupdate userdata count: " + userData.Count);
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
                            //playerInfo.SetCurrentPosition(x, y);
                            playerInfoList.Add(playerInfo);
                            Debug.WriteLine("PRIDEJO " + playerInfo.GetId() + " LIST COUNT DABAR " +  playerInfoList.Count);
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
                                //playerInfo.SetCurrentPosition(x, y);
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

                if (parts.Length == 7)
                {
                    int playerId = int.Parse(parts[0]);
                    string playerName = parts[1];
                    string playerColor = parts[2];
                    int x = int.Parse(parts[3]);
                    int y = int.Parse(parts[4]);
                    string shotColor = parts[5];
                    string shotShape = parts[6];
                    Debug.WriteLine("LSITO COUNT again" + playerInfoList.Count);
                    Debug.WriteLine("PLAYERIO ID again" + playerId);
                    playerInfoList[playerId - 1].SetCurrentPosition(x, y);
                    playerInfoList[playerId - 1].SetShotColor(shotColor);  // keiciau
                    playerInfoList[playerId - 1].SetShotShape(shotShape);
                    
                    Dispatcher.Invoke(() =>
                    {
                        Player playerInfo = playerInfoList[playerId - 1];
                        PlayerVisual playerVisual = playerVisuals[playerInfo];

                        Canvas.SetLeft(playerVisual, playerInfo.GetCurrentX());
                        Canvas.SetTop(playerVisual, playerInfo.GetCurrentY());

                        if(playerName == "DEAD")
                        {
                            HandlePlayerDeath(playerInfo);
                        }
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
            connection.On<int, double, double, string, string>("UpdateShotsFired", (playerid, X, Y, shotColor, shotShape) =>
            {
                Player playerToUpdate = playerInfoList.FirstOrDefault(player => player.MatchesId(playerid));
                if (playerToUpdate != null)
                {
                    CreateShot(playerVisuals[playerToUpdate], X, Y, playerToUpdate.GetShotColor(), playerToUpdate.GetShotShape());
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
                if (parts.Length == 7)
                {
                    int playerId = int.Parse(parts[0]);
                    string playerName = parts[1];
                    string playerColor = parts[2];
                    int x = int.Parse(parts[3]);
                    int y = int.Parse(parts[4]);
                    string shotColor = parts[5];
                    string shotShape = parts[6];
                    Player playerToDelete = new Player(playerId, playerName, playerColor , shotColor, shotShape);
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
                string image = "coin.png";
                foreach (string coinstring in coinsdata)
                {
                    string[] parts = coinstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int coinId = int.Parse(parts[0]);
                        double coinX = double.Parse(parts[1]);
                        double coinY = double.Parse(parts[2]);
                        int coinWidth = int.Parse(parts[3]);
                        int coinHeight = int.Parse(parts[4]);
                        int points = int.Parse(parts[5]);
                        Coin coin = new Coin(coinId, coinX, coinY, points, image);
                        Debug.WriteLine("coin to string: " + coin.ToString());
                        if (!coins.Contains(coin))
                        {                         
                            coins.Add(coin);
                            Dispatcher.Invoke(() =>
                            {
                              
                                CoinVisual coinVisual = new CoinVisual();

                                //CoinVisual coinVisual = new CoinVisual("coin.png", coinWidth, coinHeight);
                                //coinVisual.CoinImageHeight = coinHeight;
                                //coinVisual.CoinImageWidth = coinWidth;
                                //coinVisual.CoinImageSource = image;

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

            connection.On<int>("SendingPickedCoin", (coinid) =>
            {
                Dispatcher.Invoke(() =>
                {
                        foreach (var pair in coinVisuals)
                        {
                            Coin coin = pair.Key;
                            CoinVisual coinVisual = pair.Value;

                            if (coin.id == coinid)
                            {
                                coins.Remove(coin);
                                coinVisuals.Remove(coin);
                                CoinContainer.Children.Remove(coinVisual);
                                break;
                            }
                        }
                       
                });
            });
          
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
                        int shieldWidth = int.Parse(parts[3]);
                        int shieldHeight = int.Parse(parts[4]);
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
            connection.On<List<string>>("SendingHealthBoosts", (healthdata) =>
            {
                string image = "healthBoost.png";
                foreach (string healthstring in healthdata)
                {
                    Debug.WriteLine("health: " + healthstring);
                    string[] parts = healthstring.Split(':');
                    if (parts.Length == 6)
                    {
                        int healthId = int.Parse(parts[0]);
                        double healthX = double.Parse(parts[1]);
                        double healthY = double.Parse(parts[2]);
                        int healthWidth = int.Parse(parts[3]);
                        int healthHeight = int.Parse(parts[4]);
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
            connection.On<List<string>>("SendingSpeedBoosts", (speeddata) =>
            {
              
                foreach (string speedstring in speeddata)
                {
                    Debug.WriteLine("speedboost: " + speedstring);
                    string image = "speedboost.png";
                    string[] parts = speedstring.Split(':');
                    if (parts.Length == 7)
                    {
                        int speedId = int.Parse(parts[0]);
                        double speedX = double.Parse(parts[1]);
                        double speedY = double.Parse(parts[2]);
                        int speedWidth = int.Parse(parts[3]);
                        int speedHeight = int.Parse(parts[4]);
                        int speedVal = int.Parse(parts[5]);
                        int time = int.Parse(parts[6]);
                        SpeedBoost speed = new SpeedBoost(speedId, speedX, speedY, speedVal, image);
                        if (!speedBoosts.Contains(speed))
                        {
                            Debug.WriteLine("kuria speedboost");
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

            connection.On<List<string>>("SendingBossZombie", (bossdata) =>
            {
                Debug.WriteLine("bossdata: " + bossdata[0]);
               
                    string image = "boss.png";
                    List<Zombie> miniZombieListLocal = new List<Zombie>();
                    BossZombie bossLocal = new BossZombie("", 0, 0, 0, 0, miniZombieListLocal);
                    for (int i = 0; i < bossdata.Count; i++)
                    {
                        string data = bossdata[i];
                        string[] parts = data.Split(':');
                        if (parts.Length == 5)
                        {
                            string name = parts[0];
                            double zombieX = double.Parse(parts[1]);
                            double zombieY = double.Parse(parts[2]);
                            int zombieSize = int.Parse(parts[3]);
                            int zombieHealth = int.Parse(parts[4]);
                            Zombie zombie = new Zombie(name, zombieHealth, zombieX, zombieY, zombieSize);
                            if (i == 0) //bosas
                            {
                                if (boss.Name == "") //naujas
                                {
                                    boss = new BossZombie(name, zombieHealth, zombieX, zombieY, zombieSize, miniZombieListLocal);
                                    Dispatcher.Invoke(() =>
                                    {

                                        ZombiesVisual zombieVisual = new ZombiesVisual();
                                        zombieVisual.ZombieSize = zombieSize;
                                        zombieVisual.ZombieName = name;
                                        Debug.WriteLine("zombiesize : " + zombieVisual.ZombieSize);
                                        Canvas.SetLeft(zombieVisual, zombieX);
                                        Canvas.SetTop(zombieVisual, zombieY);
                                        bossVisual = zombieVisual;
                                        ZombieContainer.Children.Add(zombieVisual);
                                        Debug.WriteLine("isvisiblezombie : " + zombieVisual.IsVisible);
                                        HandleZombieCollisions(playerVisuals[currentPlayer]);
                                    });
                                }
                                else //atnaujint bosa
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        boss.Health = zombieHealth;
                                        boss.SetCurrentPosition(zombieX, zombieY);
                                        Canvas.SetLeft(bossVisual, zombieX);
                                        Canvas.SetTop(bossVisual, zombieY);

                                        HandleEnemyCollisions(playerVisuals[currentPlayer]);
                                    });
                                }

                            }
                            else //minions
                            {
                                if (!miniZombieList.Contains(zombie)) //naujas minion
                                {
                                    Zombie mini = new Zombie(name, zombieHealth, zombieX, zombieY, zombieSize);
                                    miniZombieListLocal.Add(mini);
                                    Dispatcher.Invoke(() =>
                                    {

                                        ZombiesVisual zombieVisual = new ZombiesVisual();
                                        zombieVisual.ZombieSize = zombieSize;
                                        zombieVisual.ZombieName = name;
                                        Canvas.SetLeft(zombieVisual, zombieX);
                                        Canvas.SetTop(zombieVisual, zombieY);
                                        zombieVisuals[mini] = zombieVisual;
                                        ZombieContainer.Children.Add(zombieVisual);
                                        HandleZombieCollisions(playerVisuals[currentPlayer]);
                                    });
                                }
                                else //atnaujint minion
                                {
                                    ZombiesVisual zombieVisual = zombieVisuals[zombie];
                                    zombie.Health = zombieHealth;
                                    zombie.SetCurrentPosition(zombieX, zombieY);
                                    Canvas.SetLeft(zombieVisual, zombieX);
                                    Canvas.SetTop(zombieVisual, zombieY);

                                    HandleZombieCollisions(playerVisuals[currentPlayer]);
                                }
                            }

                        }
                    }
                    miniZombieList = miniZombieListLocal;
                    boss.minions = miniZombieList;
 
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

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (gamestarted)
            {
                Controller controller = new Controller();
                // Handle arrow key presses here and update the player's position
                // based on the arrow key input.
                bool execute = true;
                switch (e.Key)
                {
                    case Key.Left:
                        controller.SetCommand(new MoveLeft(currentPlayer, obstacles, this.ActualWidth, this.ActualHeight));
                        //move = true;
                        break;
                    case Key.Right:
                        controller.SetCommand(new MoveRight(currentPlayer, obstacles, this.ActualWidth, this.ActualHeight));
                        //move = true;
                        break;
                    case Key.Up:
                        controller.SetCommand(new MoveUp(currentPlayer, obstacles, this.ActualWidth, this.ActualHeight));
                        //move = true;
                        break;
                    case Key.Down:
                        controller.SetCommand(new MoveDown(currentPlayer, obstacles, this.ActualWidth, this.ActualHeight));
                        //move = true;
                        break;
                    case Key.Z:
                        controller.SetCommand(new Undo(currentPlayer));
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
                HandleZombieCollisions(playerVisuals[currentPlayer]);
            }
        }

        private void UpdatePlayer(PlayerVisual playerVisual, double moveX, double moveY)
        {
            Canvas.SetLeft(playerVisual, moveX);
            Canvas.SetTop(playerVisual, moveY);
        }

        private async void Move(double newX, double newY)
        {
            UpdatePlayer(playerVisuals[currentPlayer], newX, newY);
            await connection.SendAsync("SendMove", currentPlayer.GetId(), newX, newY);
        }

        private class ShootCommand : Command
        {
            MainWindow window;
            public ShootCommand(Player player, MainWindow mainWindow) : base(player)
            {
                this.window = mainWindow;
            }

            public override void Execute()
            {
                window.Shoot((int)player.GetDirectionX(), (int)player.GetDirectionY());
            }
        }

        protected void Shoot(int deltaX, int deltaY)
        {
            CreateShot(playerVisuals[currentPlayer], deltaX, deltaY, currentPlayer.GetShotColor(), currentPlayer.GetShotShape());
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
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(8);
            timer.Tick += (sender, args) =>
            {
                testLabel.Text = "";
                //heartLabel.Visibility = Visibility.Hidden;
                //shieldBorder.Visibility = Visibility.Hidden;
                timer.Stop();
            };

            timer.Start();
        }
        private void HideShieldDisplay()
        {
            // tekstas dingsta po 10 sekundziu
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += (sender, args) =>
            {
                ShieldOn shieldObj = new ShieldOn(currentPlayer);
                bool shieldVisible = shieldObj.HideShield().shieldOn;
                if (!shieldVisible)
                {
                    shieldBorder.Visibility = Visibility.Hidden;
                    gameStat.ShieldOn = false;
                    timer.Stop();
                }          
            };

            timer.Start();
        }

        public static bool CheckCollision(double x1, double y1, double width1, double height1, double x2, double y2, double width2, double height2)
        {
            return x1 + width1 >= x2 && x1 <= x2 + width2 && y1 + height1 >= y2 && y1 <= y2 + height2;
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

                    if (CheckCollision(playerX, playerY, playerVisual.Width, playerVisual.Height,
                                                       enemyX, enemyY, enemyRect.Width, enemyRect.Height))
                    {
                        await HandleCollision(playerVisual, enemy);
                    }
                }
            }
        }
        private async void HandleZombieCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);

            foreach (Zombie mini in miniZombieList)
            {
                if (zombieVisuals.ContainsKey(mini))
                {
                    ZombiesVisual miniRect = zombieVisuals[mini];
                    double miniX = Canvas.GetLeft(miniRect);
                    double miniY = Canvas.GetTop(miniRect);

                    if (CheckCollision(playerX, playerY, playerVisual.Width, playerVisual.Height,
                                                       miniX, miniY, miniRect.Width, miniRect.Height))
                    {
                        await HandleCollisionZ(10);
                    }
                }
            }
            if (bossVisual.Name != "")
            {
                double miniX = Canvas.GetLeft(bossVisual);
                double miniY = Canvas.GetTop(bossVisual);

                if (CheckCollision(playerX, playerY, playerVisual.Width, playerVisual.Height,
                                                   miniX, miniY, bossVisual.Width, bossVisual.Height))
                {
                    await HandleCollisionZ(30);
                }
            }
        }
        private async Task HandleCollision(PlayerVisual playerVisual, Enemy enemy)
        {
            if (!gameStat.ShieldOn)
            {
                gameStat.PlayerHealth -= 5;
                healthLabel.Text = $"Health: {gameStat.PlayerHealth}";

                HealthAdd healthObj = new HealthAdd(currentPlayer);
                healthBar.Width = gameStat.PlayerHealth <= 0
                                    ? healthObj.DisplayHealth(0).health
                                    : healthObj.DisplayHealth(gameStat.PlayerHealth / 2).health;

                if (gameStat.PlayerHealth <= 0)
                {
                    HandlePlayerDeath(currentPlayer);
                    await connection.SendAsync("UpdateDeadPlayer", currentPlayer.GetId());
                }
            }

            collisionCheckedEnemies[enemy] = true;
        }
   
        private async Task HandleCollisionZ(int damage)
        {
            if (!gameStat.ShieldOn)
            {
                gameStat.PlayerHealth -= damage;
                healthLabel.Text = $"Health: {gameStat.PlayerHealth}";

                HealthAdd healthObj = new HealthAdd(currentPlayer);
                healthBar.Width = gameStat.PlayerHealth <= 0
                                    ? healthObj.DisplayHealth(0).health
                                    : healthObj.DisplayHealth(gameStat.PlayerHealth / 2).health;

                if (gameStat.PlayerHealth <= 0)
                {
                    HandlePlayerDeath(currentPlayer);
                    await connection.SendAsync("UpdateDeadPlayer", currentPlayer.GetId());
                }
            }
        }

        private void HandlePlayerDeath(Player player)
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


        private async void HandleCoinsCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);

            // Create a copy of the coins collection
            List<Coin> coinsCopy = new List<Coin>(coins);

            foreach (Coin coin in coinsCopy)
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
                        coin.Interact(gameStat);
                        scoreLabel.Text = $"Score: {gameStat.PlayerScore}";
                        Base text = new Base(currentPlayer);
                        testLabel.Text = text.DisplayObject("coin").text;
                        HideDisplay();

                        //------------- JSONAdapter
                        Dictionary<string, object> jsonObject = new Dictionary<string, object>();

                        // Add key-value pairs to the dictionary
                        jsonObject["id"] = coin.id;
                        jsonObject["x"] = coin.X;
                        jsonObject["y"] = coin.Y;
                        jsonObject["width"] = coin.Width;
                        jsonObject["height"] = coin.Height;
                        jsonObject["points"] = coin.Points;


                        // Convert the dictionary to a JSON string
                        string jsonString = JsonConvert.SerializeObject(jsonObject);
                        ServerString server = new ServerString(jsonString);
                        
                        //-----------
                        await connection.SendAsync("SendPickedCoin", server.ConvertedString);  //coin.ToString()
                    }
                }
            }
        }
        
        private async void HandleShieldsCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);
            List<Shield> shieldsCopy = new List<Shield>(shields);
            foreach (Shield shield in shieldsCopy)
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
                        //Player player = playerVisuals.FirstOrDefault(pair => pair.Value == playerVisual).Key;
                        //shield.Interact(player, shield.Time);
                        
                        Base baseObj = new Base(currentPlayer);
                        testLabel.Text = baseObj.DisplayObject("shield").text;
                        ShieldOn shieldObj = new ShieldOn(currentPlayer);
                        bool shieldVisible = shieldObj.DisplayShield().shieldOn;
                        if (shieldVisible)
                        {
                            shieldBorder.Visibility = Visibility.Visible;
                            gameStat.ShieldOn = true; //i gamestat perkelt
                            shield.Interact(gameStat);
                        }
                        HideDisplay();
                        HideShieldDisplay();
                        await connection.SendAsync("SendPickedShield", shield.ToString());

                    }
                }
            }
        }
        private async void HandleSpeedBoostsCollisions(PlayerVisual playerVisual)
        {
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);
            List<SpeedBoost> speedBoostCopy = new List<SpeedBoost>(speedBoosts);
            foreach (SpeedBoost speedBoost in speedBoostCopy)
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

                        if (gameStat.PlayerSpeed < 50)
                        {
                            speedBoost.Interact(gameStat);
                            Base text = new Base(currentPlayer);
                            testLabel.Text = text.DisplayObject("speed").text;
                            HideDisplay();

                            await connection.SendAsync("SendPickedSpeedBoost", speedBoost.ToString());
                        }

                    }
                }
            }
        }
        private async void HandleHealthBoostsCollisions(PlayerVisual playerVisual)
        {
            if (isCollidingWithHealthBoost)
            {
                Debug.WriteLine("skippp");
                return; // Skip collision handling if already colliding
            }
            double playerX = Canvas.GetLeft(playerVisual);
            double playerY = Canvas.GetTop(playerVisual);
            List<HealthBoost> healthBoostCopy = new List<HealthBoost>(healthBoosts);
            foreach (HealthBoost healthBoost in healthBoostCopy)
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
                        isCollidingWithHealthBoost = true;
                        //Player player = playerVisuals.FirstOrDefault(pair => pair.Value == playerVisual).Key;
                        //healthBoost.Interact(player, healthBoost.Health);
                        Debug.WriteLine("health pries: " + gameStat.PlayerHealth);
                        Debug.WriteLine("health reiksme: " + healthBoost.Health);
                        healthBoost.Interact(gameStat);
                        if (gameStat.PlayerHealth > 100)
                        {
                            gameStat.PlayerHealth = 100;
                        }
                        
                        healthLabel.Text = $"Health: {gameStat.PlayerHealth}";
                        Debug.WriteLine("health po: " + gameStat.PlayerHealth);
                        Debug.WriteLine("CIA IEINA");
                        healthBoosts.Remove(healthBoost);
                        healthBoostsVisuals.Remove(healthBoost);
                        HealthBoostContainer.Children.Remove(healthBoostRect);
                        Base text = new Base(currentPlayer);
                        testLabel.Text = text.DisplayObject("health").text;
                        HideDisplay();
                        HealthAdd healthObj = new HealthAdd(currentPlayer);
                        healthBar.Width = healthObj.DisplayHealth(gameStat.PlayerHealth/2).health;
                        //await connection.SendAsync("UpdatePlayerHealth", currentPlayer.GetId(), gameStat.PlayerHealth);
                        await connection.SendAsync("SendPickedHealthBoost", healthBoost.ToString());
                        isCollidingWithHealthBoost = false;
                    }
                }
            }
        }


        public async void CreateShot(PlayerVisual playerVisual, double directionX, double directionY, string color, string shape)
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
                    
                    SingleShot(playerX, playerY, playerWidth, playerHeight, color, shape, out shot);

                    Color shotColor = (Color)ColorConverter.ConvertFromString(shot.getColor());
                    solidColorBrush = new SolidColorBrush(shotColor);

                    shotVisual = new ShotVisual();
                    shotVisual.EllipseSize = shot.getSize();
                    shotVisual.PolygonSize = shot.getSize();
                    shotVisual.FillColor = solidColorBrush;
                    shotVisual.UpdateShot(solidColorBrush, shot.getShape());

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
            IShape shotShape;
            if (shape == "triangle")
            {
                shotShape = new TriangleShot();
            }
            else
            {
                shotShape = new RoundShot();
            }

            Shot localShot = new Shot(shotColor, shotShape, 5, 10, 5);

            double playerCenterX = playerX + playerWidth / 2;
            double playerCenterY = playerY + playerHeight / 2;

            localShot.setPosition(playerCenterX - localShot.getSize() / 2, playerCenterY - localShot.getSize() / 2);
            shot = localShot;
        }
    }
}
