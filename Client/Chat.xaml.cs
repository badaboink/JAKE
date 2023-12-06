using System;
using System.Collections.Generic;
using System.Linq;
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

namespace JAKE.client
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        private string username = "You";
        public event EventHandler<string> MessageSent;
        public Chat(MainWindow mainWindow)
        {
            InitializeComponent();
            textBoxMessage.PreviewKeyDown += textBoxMessage_PreviewKeyDown;
            mainWindow.NameEntered += MainWindow_NameEntered;
            mainWindow.MessageGot += GetMessage;
        }

        private void MainWindow_NameEntered(object sender, string e)
        {
            username = e;
        }

        private void SendMessage()
        {
            string message = textBoxMessage.Text;

            if (!string.IsNullOrWhiteSpace(message))
            {
                // Create a new TextBlock for the message
                TextBlock messageTextBlock = new TextBlock();
                messageTextBlock.Text = $"{username}: {message}";
                messageTextBlock.TextWrapping = TextWrapping.Wrap; // Set text wrapping

                // Create a Border for the message
                Border messageBorder = new Border();
                messageBorder.Child = messageTextBlock;
                messageBorder.BorderBrush = Brushes.Black; // Set border color
                messageBorder.BorderThickness = new Thickness(1); // Set border thickness
                messageBorder.Margin = new Thickness(0, 0, 0, 5); // Set margin

                // Add the Border to the StackPanel
                stackPanel.Children.Add(messageBorder);

                // Scroll to the bottom
                scrollViewer.ScrollToBottom();
                MessageSent?.Invoke(this, message);
                // Clear the message input
                textBoxMessage.Clear();
            }
        }

        private void GetMessage(object sender, string e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Your existing code for updating the UI goes here
                TextBlock messageTextBlock = new TextBlock();
                messageTextBlock.Text = $"{e}";
                messageTextBlock.TextWrapping = TextWrapping.Wrap;

                Border messageBorder = new Border();
                messageBorder.Child = messageTextBlock;
                messageBorder.BorderBrush = Brushes.Black;
                messageBorder.BorderThickness = new Thickness(1);
                messageBorder.Margin = new Thickness(0, 0, 0, 5);

                stackPanel.Children.Add(messageBorder);

                scrollViewer.ScrollToBottom();
            });
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void textBoxMessage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
                e.Handled = true;
            }
        }
    }
}
