using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using JAKE.classlibrary;

class Server
{
    private List<Player> players = new List<Player>();
    private TcpListener listener;
    private Dictionary<int, NetworkStream> playerStreams = new Dictionary<int, NetworkStream>();
    private List<Obstacle> obstacles = new List<Obstacle>();
    //Enemies
    private List<Enemy> enemies = new List<Enemy>();
    private int enemySpawnIntervalInSeconds = 10;
    private int enemyUpdateIntervalInMilliseconds = 200;

    public Server()
    {
        listener = new TcpListener(IPAddress.Any, 12345);
        StartEnemySpawnTimer();
        StartEnemyUpdateTimer();
    }
    private void StartEnemySpawnTimer()
    {
        Timer enemySpawnTimer = new Timer(EnemySpawnCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(enemySpawnIntervalInSeconds));
    }

    private void EnemySpawnCallback(object state)
    {
        // Create and add a new enemy
        Random random = new Random();
        Enemy enemy = new Enemy(enemies.Count + 1, "Red", 2); // Initialize with appropriate parameters
        double spawnX = random.Next(0, 1936); // Adjust the spawn area
        double spawnY = random.Next(0, 1056); // Adjust the spawn area
        enemy.SetCurrentPosition(spawnX, spawnY);
        lock (enemies)
        {
            enemies.Add(enemy);
        }

        // Broadcast the new enemy information to connected clients
        Console.WriteLine("Enemy spawned");
        BroadcastEnemyPositions();
    }

    //private void BroadcastEnemySpawn(Enemy enemy)
    //{
    //    string enemySpawnMessage = $"ENEMY_SPAWN:{enemy}";
    //    Console.WriteLine(enemySpawnMessage);
    //    foreach (var stream in playerStreams.Values)
    //    {
    //        SendString(stream, enemySpawnMessage);
    //    }
    //}

    

    public void Start()
    {
        listener.Start();
        GenerateObstacles();
        Console.WriteLine("Server started. Waiting for connections...");
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            // Handle the new client in a separate thread
            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        string tempColor = ReceiveString(stream);

        // Generate a unique ID for the player (you can use a better method)
        int playerId;
        lock (players)
        {
            playerId = players.Count + 1;
        }

        Player newPlayer = new Player(playerId, "a", tempColor);

        Random random = new Random();
        newPlayer.SetCurrentPosition(0, 0);
        players.Add(newPlayer);
        playerStreams.Add(playerId, stream);

        // Send new player their player info separately
        string obstacleData = string.Join(",", obstacles.Select(obstacle => obstacle.ToString()));
        string fullObstacleData = $"ObstacleData:{obstacleData}";
        Console.WriteLine(fullObstacleData);
        byte[] obstacleBytes = Encoding.UTF8.GetBytes(fullObstacleData);
        stream.Write(obstacleBytes, 0, obstacleBytes.Length);

        string initializationMessage = $"INIT:{newPlayer.GetId()}:{newPlayer.GetName()}:{newPlayer.GetColor()}:{newPlayer.GetCurrentX()}:{newPlayer.GetCurrentY()} ";
        Console.Write(initializationMessage);
        byte[] initBytes = Encoding.UTF8.GetBytes(initializationMessage);
        stream.Write(initBytes, 0, initBytes.Length);
        stream.Flush();

        // Broadcast the updated player list to all clients
        BroadcastPlayerList();

        // Handle player input, gameplay, etc., here...
        while (true)
        {
            string message = ReceiveString(stream);

            // Handle movement updates
            if (message.StartsWith("MOVE:"))
            {
                HandleMovementUpdate(message);
            }

            // Handle other types of messages as needed
            // ...
        }

        // When the player disconnects, remove them from the player list and broadcast the updated list
        //players.Remove(newPlayer);
        //BroadcastPlayerList();

        //client.Close();
    }

    private void GenerateObstacles()
    {
        Random random = new Random();
        for (int i = 0; i < random.Next(8, 10); i++)
        {
            //nzn kuris geriau atrodo
            //int temp = random.Next(0, 2);
            //int widthTemp = temp == 0 ? random.Next(400, 500) : 50;
            //int heightTemp = temp == 1 ? random.Next(400, 500) : 50;

            double widthTemp = random.Next(50, 300);
            double heightTemp = (widthTemp < 150) ? random.Next(200, 300) : random.Next(50, 100);

            // TO DO: atsisakau daryti packing algoritma


            double xtemp = random.Next(60, (int)(1920-widthTemp));
            double ytemp = random.Next(60, (int)(1080 - heightTemp));

            Obstacle obstacle = new Obstacle(widthTemp, heightTemp, xtemp, ytemp);
            obstacles.Add(obstacle);
        }
    }
    private void HandleMovementUpdate(string message)
    {
        // Parse the movement update message
        string[] parts = message.Split(':');
        if (parts.Length == 4 && parts[0] == "MOVE")
        {
            Console.WriteLine(message);
            int playerId = int.Parse(parts[1]);
            int deltaX = int.Parse(parts[2]);
            int deltaY = int.Parse(parts[3]);

            // Find the player by ID and update their position
            Player playerToUpdate = players.FirstOrDefault(p => p.GetId() == playerId);
            if (playerToUpdate != null)
            {
                playerToUpdate.SetCurrentPosition(deltaX, deltaY);
            }

            // Broadcast the updated player positions to all clients
            BroadcastPlayerList();
        }
    }

    private void BroadcastPlayerList()
    {
        foreach (Player player in players)
        {
            string playerList = "PLAYER_LIST;" + string.Join(",", players
                .Where(p => p.GetId() != player.GetId()) // Exclude the current player
                .Select(p => $"{p.GetId()}:{p.GetName()}:{p.GetColor()}:{p.GetCurrentX()}:{p.GetCurrentY()}"));

            if (playerList.Length > "PLAYER_LIST;".Length) { Console.WriteLine($"{playerList}"); }

            NetworkStream stream = GetStreamForPlayer(player);
            SendString(stream, playerList);
        }
    }

    private NetworkStream GetStreamForPlayer(Player player)
    {
        if (playerStreams.TryGetValue(player.GetId(), out NetworkStream stream))
        {
            return stream;
        }
        else
        {
            // Handle the case where the stream for the player is not found (e.g., throw an exception).
            throw new InvalidOperationException($"Stream for player {player.GetId()} not found.");
        }
    }

    // Utility methods for sending and receiving strings
    private void SendString(NetworkStream stream, string data)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(data);
        stream.Write(bytes, 0, bytes.Length);
    }

    private string ReceiveString(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
    }

    private void UpdateEnemyPositions()
    {
        lock (enemies)
        {
            foreach (var enemy in enemies)
            {
                // Find the closest player
                Player closestPlayer = FindClosestPlayer(enemy);

                if (closestPlayer != null)
                {
                    // Calculate direction vector from enemy to closest player
                    double directionX = closestPlayer.GetCurrentX() - enemy.GetCurrentX();
                    double directionY = closestPlayer.GetCurrentY() - enemy.GetCurrentY();

                    // Normalize the direction vector
                    double length = Math.Sqrt(directionX * directionX + directionY * directionY);
                    if (length > 0)
                    {
                        directionX /= length;
                        directionY /= length;
                    }

                    // Define enemy movement speed
                    double enemySpeed = enemy.GetSpeed();

                    // Update enemy position based on direction and speed
                    double newX = enemy.GetCurrentX() + directionX * enemySpeed;
                    double newY = enemy.GetCurrentY() + directionY * enemySpeed;

                    enemy.SetCurrentPosition(newX, newY);
                }
            }
        }
    }

    private Player FindClosestPlayer(Enemy enemy)
    {
        Player closestPlayer = null;
        double closestDistance = double.MaxValue;

        foreach (var player in players)
        {
            // Calculate the distance between enemy and player
            double distance = Math.Sqrt(
                Math.Pow(player.GetCurrentX() - enemy.GetCurrentX(), 2) +
                Math.Pow(player.GetCurrentY() - enemy.GetCurrentY(), 2));

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    private void StartEnemyUpdateTimer()
    {
        Timer enemyUpdateTimer = new Timer(EnemyUpdateCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(enemyUpdateIntervalInMilliseconds));
    }

    private void EnemyUpdateCallback(object state)
    {
        UpdateEnemyPositions();
        BroadcastEnemyPositions();
    }

    private void BroadcastEnemyPositions()
    {
        string message;
        lock (enemies)
        {
            string enemyPositions = string.Join(",", enemies.Select(enemy =>
                $"{enemy.GetId()}:{enemy.GetColor()}:{enemy.GetCurrentX()}:{enemy.GetCurrentY()}"));
            message = $"ENEMY_POSITIONS;{enemyPositions}";
        }
        //Console.WriteLine(message);
        foreach (var stream in playerStreams.Values)
        {
            SendString(stream, message);
        }
    }

}

class Program
{
    static void Main()
    {
        Server server = new Server();
        server.Start();
    }
}


