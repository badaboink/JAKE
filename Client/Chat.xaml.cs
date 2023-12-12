using JAKE.classlibrary.Patterns;
using JAKE.classlibrary.Patterns.Interpreter;
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
        private readonly ChatMediator mediator;
        private readonly IMessageInterpreter specialSymbolInterpreter;
        private readonly IMessageInterpreter profanityFilterInterpreter;
        private readonly IMessageInterpreter commandFilterInterpreter;
        public Chat(MainWindow mainWindow, ChatMediator mediator)
        {
            InitializeComponent();
            this.mediator = mediator;
            this.mediator.MessageSent += GetMessage;
            specialSymbolInterpreter = new SpecialSymbolInterpreter();
            profanityFilterInterpreter = new ProfanityFilterInterpreter();
            commandFilterInterpreter = new CommandInterpreter();

            textBoxMessage.PreviewKeyDown += textBoxMessage_PreviewKeyDown;
            mainWindow.NameEntered += MainWindow_NameEntered;
            mainWindow.MessageGot += GetMessage;
        }

        private void MainWindow_NameEntered(object sender, string e)
        {
            username = e;
        }
        public void SetUsername(string newName)
        {
            username = newName;
        }

        private void SendMessage()
        {
            string message = textBoxMessage.Text;

            if (!string.IsNullOrWhiteSpace(message))
            {
                string help = commandFilterInterpreter.interpret(message);
                if(help != null)
                {
                    mediator.SendMessage(help, "System", username);
                }
                else
                {
                    message = specialSymbolInterpreter.interpret(message);
                    message = profanityFilterInterpreter.interpret(message);
                    mediator.SendMessage(message, username, null);
                }
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
