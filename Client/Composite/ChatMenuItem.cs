using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.client.Composite
{
    public class ChatMenuItem : IMenuItem
    {
        public void Execute(MainWindow mainWindow)
        {
            var chatWindow = mainWindow.chatWindow;

            if (chatWindow != null && !chatWindow.IsLoaded)
            {
                chatWindow = new Chat(mainWindow, mainWindow.mediator);
                chatWindow.SetUsername(mainWindow.username);
                chatWindow.Show();
                mainWindow.chatWindow = chatWindow;
            }
        }
    }
}
