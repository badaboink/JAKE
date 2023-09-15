using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using JAKE.classlibrary;
using JAKE.Client;

namespace JAKE.client
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private Player currentPlayer;
        private PlayerVisual currentPlayerVisuals;
        private Dictionary<Player, PlayerVisual> playerVisuals = new Dictionary<Player, PlayerVisual>();

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Create and show the ColorChoiceForm as a pop-up
            ColorChoiceForm colorChoiceForm = new ColorChoiceForm();
            colorChoiceForm.ShowDialog();
            string selectedColor = colorChoiceForm.SelectedColor;

            client = new TcpClient("localhost", 12345);
            stream = client.GetStream();

            byte[] colorBytes = Encoding.UTF8.GetBytes(selectedColor);
            stream.Write(colorBytes, 0, colorBytes.Length);
            stream.Flush();

            var receiveThread = new System.Threading.Thread(ReceivePlayerData);
            receiveThread.Start();
        }
        public String getFirstWordUsingSubString(String input)
        {
            int index = input.Contains(" ") ? input.IndexOf(" ") : 0;
            return input.Substring(0, index);
        }
        public string GetStringAfterFirstSpace(string input)
        {
            int index = input.Contains(" ") ? input.IndexOf(" ") + 1 : 0;
            return input.Substring(index);
        }
        private void ReceivePlayerData()
        {
            bool initialized = false;
            List<Player> playerInfoList = new List<Player>();

            while (true)
            {
                // Create a list to store information about all players
                
                string message = ReceiveString(stream);

                if (message.StartsWith("INIT:") && !initialized)
                {
                    string initMessage = getFirstWordUsingSubString(message);
                    // Parse and process the initialization message
                    string[] parts = initMessage.Split(':');
                    if (parts.Length == 6)
                    {
                        int playerId = int.Parse(parts[1]);
                        string playerName = parts[2];
                        string playerColor = parts[3];
                        int x = int.Parse(parts[4]);
                        int y = int.Parse(parts[5]);

                        // Create a Player object or update the current player's information
                        currentPlayer = new Player(playerId, playerName, playerColor);
                        currentPlayer.SetCurrentPosition(x, y);

                        playerInfoList.Add(currentPlayer);

                        initialized = true;
                    }
                }

                string playerList = GetStringAfterFirstSpace(message);
                // Assuming the player list format is "ID:Name:Color,ID:Name:Color,..."

                // Split the player list into individual player entries
                string[] playerEntries = playerList.Split(',');

                foreach (string playerEntry in playerEntries)
                {
                    // Split each player entry into ID, Name, and Color parts
                    string[] parts = playerEntry.Split(':');

                    if (parts.Length == 5)
                    {
                        int playerId = int.Parse(parts[0]);
                        string playerName = parts[1];
                        string playerColor = parts[2];
                        int x = int.Parse(parts[3]);
                        int y = int.Parse(parts[4]);

                        // Create a PlayerInfo object to store player information
                        Player playerInfo = new Player(playerId, playerName, playerColor);
                        playerInfo.SetCurrentPosition(x, y);
                        playerInfoList.Add(playerInfo);
                    }
                }

                // Update the client's view to display the players
                UpdateClientView(playerInfoList);
            }
        }
        
        private void UpdateClientView(List<Player> playerInfoList)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (Player playerInfo in playerInfoList)
                {
                    if (playerVisuals.ContainsKey(playerInfo))
                    {
                        PlayerVisual playerVisual = playerVisuals[playerInfo];
                        Canvas.SetLeft(playerVisual, playerInfo.GetCurrentX());
                        Canvas.SetTop(playerVisual, playerInfo.GetCurrentY());
                    }
                    else
                    {
                        PlayerVisual playerVisual = new PlayerVisual();
                        ColorConverter converter = new ColorConverter();
                        Color playerColor = (Color)ColorConverter.ConvertFromString(playerInfo.GetColor());
                        SolidColorBrush solidColorBrush = new SolidColorBrush(playerColor);
                        playerVisual.PlayerColor = solidColorBrush;
                        playerVisual.UpdateColor(solidColorBrush);
                        Canvas.SetLeft(playerVisual, playerInfo.GetCurrentX());
                        Canvas.SetTop(playerVisual, playerInfo.GetCurrentY());
                        playerVisuals[playerInfo] = playerVisual;
                        playersContainer.Items.Add(playerVisual);
                    }
                }
            });
        }
        private string ReceiveString(NetworkStream stream)
        {
            byte[] buffer = new byte[1024]; // Adjust the buffer size as needed
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the game loop (if needed)
            //StartGameLoop();
        }
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Suppress arrow key events at the window level to prevent scrolling or other default behaviors.
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            int deltaX = 0;
            int deltaY = 0;

            Console.WriteLine($"Key pressed: {e.Key}");

            // Handle arrow key presses here and update the player's position
            // based on the arrow key input.
            if (e.Key == Key.Left)
            {
                deltaX = -1; // Move left
            }
            else if (e.Key == Key.Right)
            {
                deltaX = 1; // Move right
            }
            else if (e.Key == Key.Up)
            {
                deltaY = -1; // Move up
            }
            else if (e.Key == Key.Down)
            {
                deltaY = 1; // Move down
            }
            //UpdatePlayerPosition(playerVisuals[currentPlayer], deltaX, deltaY);

            string movementUpdateMessage = UpdatePlayerPosition(playerVisuals[currentPlayer], deltaX, deltaY);

            // Send the movement update message to the server using the client's network stream
            byte[] updateData = Encoding.UTF8.GetBytes(movementUpdateMessage);
            stream.Write(updateData, 0, updateData.Length);
            stream.Flush();

        }
        private string UpdatePlayerPosition(PlayerVisual playerVisual, int deltaX, int deltaY)
        {
            double currentX = Canvas.GetLeft(playerVisual);
            double currentY = Canvas.GetTop(playerVisual);

            // Calculate the new position
            double newX = currentX + deltaX * 100; // Adjust 'moveSpeed' as needed
            double newY = currentY + deltaY * 100;

            // Set the new position
            Canvas.SetLeft(playerVisual, newX);
            Canvas.SetTop(playerVisual, newY);

            return $"MOVE:{currentPlayer.GetId()}:{newX}:{newY}";
        }
    }
}
