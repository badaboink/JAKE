using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.Interpreter
{
    public class CommandInterpreter : IMessageInterpreter
    {
        public string Interpret(string message)
        {
            if (message.ToLower() == "/help")
            {
                return "\nUse arrow buttons to move, space button to shoot.\nAvoid and shoot enemies - blue circles, green circles, as well as corona" +
                    "elements.\nPick up power ups - coins, health, shields.\nScore and health are displayed in the left most corner of the game screen.\n" +
                    "To access settings click escape button.\n" +
                    "Try out various chat emoticons by typing: #heart, #star, #happy, #sad...\n" +
                    "If someone is being abusive in game - log off and close your eyes.";
            }
            else if (message.StartsWith("/"))
            {
                return "To get help, type /help";
            }
            return null;
        }
    }
}
