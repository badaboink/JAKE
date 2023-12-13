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

        public event EventHandler<ChatEvent> MessageSent;
        public event EventHandler<ChatEvent> MessageGot;

        public void SendMessage(string message, string sender = null, string receiver = null)
        {
            if (receiver == null)
            {
                // Broadcast message to all
                MessageSent?.Invoke(this, new ChatEvent(sender, message));
                //MessageGot?.Invoke(this, new ChatEvent(sender, message));
            }
            else
            {
                // Send message to a specific receiver
                //MessageSent?.Invoke(this, new ChatEvent(sender, message, receiver));
                MessageGot?.Invoke(this, new ChatEvent(sender, message, receiver));
            }
        }

    }

    public class ChatEvent : EventArgs
    {
        public ChatEvent(string sender, string message, string receiver = null)
        {
            this.sender = sender;
            this.message = message;
            this.receiver = receiver;
        }

        public string sender { get; set; }
        public string receiver { get; set; }
        public string message { get; set; }

    }
}
