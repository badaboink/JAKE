using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class ChatMediator : IMediator
    {

        public event EventHandler<string> MessageSent;

        public void SendMessage(string message, string sender = null, string receiver = null)
        {
            if (receiver == null)
            {
                // Broadcast message to all
                MessageSent?.Invoke(this, $"{sender}: {message}");
            }
            else
            {
                // Send message to a specific receiver
                MessageSent?.Invoke(this, $"System: {message}");
            }
        }

    }
}
