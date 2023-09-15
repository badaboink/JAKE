using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JAKE.client
{
    public partial class Form1 : Form
    {
        private int cubeX = 100;
        private int cubeY = 100;
        private const int cubeSize = 50;
        private const int moveDistance = 10;

        private TcpClient client;
        private NetworkStream stream;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Show the color choice dialog to the player
            string selectedColor = ShowColorChoiceDialog();

            // Connect to the server
            client = new TcpClient("localhost", 12345);
            stream = client.GetStream();

            byte[] colorBytes = Encoding.ASCII.GetBytes(selectedColor);
            stream.Write(colorBytes, 0, colorBytes.Length);
            stream.Flush();

            // Start a thread to receive and process player data from the server
            var receiveThread = new System.Threading.Thread(ReceivePlayerData);
            receiveThread.Start();
        }
        private string ShowColorChoiceDialog()
        {
            using (var colorChoiceForm = new ColorChoiceForm())
            {
                colorChoiceForm.ShowDialog();
                return colorChoiceForm.SelectedColor;
            }
        }
        private string ReceiveString(NetworkStream stream)
        {
            byte[] buffer = new byte[1024]; // Adjust the buffer size as needed
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
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
                        Color playerColor = Color.FromName(parts[2]);

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
            this.Invoke(new Action(() =>
            {
                Graphics g = gameCanvas.CreateGraphics();
                Random random = new Random();

                foreach (Player playerInfo in playerInfoList)
                {
                    // Render each player as a colored cube
                    Brush playerBrush = new SolidBrush(playerInfo.Color);

                    int canvasWidth = gameCanvas.Width;
                    int canvasHeight = gameCanvas.Height;
                    int cubeSize = 50;

                    int cubeX = random.Next(0, canvasWidth - cubeSize);
                    int cubeY = random.Next(0, canvasHeight - cubeSize);

                    g.FillRectangle(playerBrush, cubeX, cubeY, cubeSize, cubeSize);

                    playerBrush.Dispose(); // Dispose of the brush to free resources
                }
            }));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Create a Graphics object to draw on the form
            Graphics g = e.Graphics;

            // Draw the cube
            g.FillRectangle(Brushes.Blue, cubeX, cubeY, cubeSize, cubeSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Move the cube based on arrow key input
            switch (e.KeyCode)
            {
                case Keys.Left:
                    cubeX -= moveDistance;
                    break;
                case Keys.Right:
                    cubeX += moveDistance;
                    break;
                case Keys.Up:
                    cubeY -= moveDistance;
                    break;
                case Keys.Down:
                    cubeY += moveDistance;
                    break;
            }

            // Send the cube's new position to the server
            string newPosition = $"{cubeX},{cubeY}";
            byte[] data = Encoding.ASCII.GetBytes(newPosition);
            stream.Write(data, 0, data.Length);

            // Redraw the form to update the cube's position
            this.Invalidate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up resources when the form is closing
            stream.Close();
            client.Close();
            base.OnFormClosing(e);
        }

        public class Player
        {
            public int Id { get; }
            public string Name { get; }
            public Color Color { get; }

            public Player(int id, string name, Color color)
            {
                Id = id;
                Name = name;
                Color = color;
            }
        }

    }
}