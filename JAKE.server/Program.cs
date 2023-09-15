using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using JAKE.classlibrary;

class Server
{
    private List<Player> players = new List<Player>();
    private TcpListener listener;
    private Dictionary<int, NetworkStream> playerStreams = new Dictionary<int, NetworkStream>();

    public Server()
    {
        listener = new TcpListener(IPAddress.Any, 12345);
    }

    public void Start()
    {
        listener.Start();
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
        Console.WriteLine($"{playerId} {tempColor}");
        //Color playerColor = Color.FromName(tempColor);
        // Create a new player with a random color
        Player newPlayer = new Player(playerId, "a", tempColor);
        players.Add(newPlayer);
        playerStreams.Add(playerId, stream);

        // Broadcast the updated player list to all clients
        BroadcastPlayerList();

        // Handle player input, gameplay, etc., here...

        // When the player disconnects, remove them from the player list and broadcast the updated list
        //players.Remove(newPlayer);
        //BroadcastPlayerList();

        //client.Close();
    }

    private void BroadcastPlayerList()
    {
        string playerList = string.Join(",", players.Select(player => $"{player.GetId()}:{player.GetName()}:{player.GetColor()}"));
        Console.WriteLine($"{playerList}");
        foreach (Player player in players)
        {
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
}

class Program
{
    static void Main()
    {
        Server server = new Server();
        server.Start();
    }
}
