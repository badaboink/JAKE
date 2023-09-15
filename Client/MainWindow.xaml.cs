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

        public MainWindow()
        {
            InitializeComponent();
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
        private void ReceivePlayerData()
        {
            while (true)
            {
                string playerList = ReceiveString(stream);
                // Assuming the player list format is "ID:Name:Color,ID:Name:Color,..."

                // Split the player list into individual player entries
                string[] playerEntries = playerList.Split(',');

                // Create a list to store information about all players
                List<Player> playerInfoList = new List<Player>();

                foreach (string playerEntry in playerEntries)
                {
                    // Split each player entry into ID, Name, and Color parts
                    string[] parts = playerEntry.Split(':');

                    if (parts.Length == 3)
                    {
                        int playerId = int.Parse(parts[0]);
                        string playerName = parts[1];
                        string playerColor = parts[2];

                        // Create a PlayerInfo object to store player information
                        Player playerInfo = new Player(playerId, playerName, playerColor);
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
                playersContainer.Items.Clear();

                Random random = new Random();

                foreach (Player playerInfo in playerInfoList)
                {
                    // Create a new PlayerVisual user control
                    PlayerVisual playerVisual = new PlayerVisual();
                    ColorConverter converter = new ColorConverter();
                    Color playerColor = (Color)ColorConverter.ConvertFromString(playerInfo.GetColor());
                    SolidColorBrush solidColorBrush = new SolidColorBrush(playerColor);

                    PlayerVisualWrapper visualWrapper = new PlayerVisualWrapper(solidColorBrush);
                    playerVisual.PlayerColor = solidColorBrush;

                    // Customize the playerVisual's appearance if needed
                    //playerVisual.Width = 50;
                    //playerVisual.Height = 50;

                    Canvas.SetLeft(playerVisual, random.Next(0, 800-50));
                    Canvas.SetTop(playerVisual, random.Next(0, 600-50));

                    // Add the playerVisual to the ItemsControl
                    playersContainer.Items.Add(playerVisual);
                    //playersContainer.Children.Add(visualWrapper);
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

    }
}
