using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.Proxy
{
    public class ObserverLogger : IObserverLogger
    {
        private IObserverLogger observer;
        public ObserverLogger(IObserverLogger observer)
        {
            this.observer = observer;
        }

        public async Task SendPlayerMessage(string name, string message)
        {
            using(StreamWriter sw = new StreamWriter("ChatLogs/" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "_ChatLogs.txt", true))
            {
                sw.WriteLine(string.Format($"{name}: {message}"));
            }
            await observer.SendPlayerMessage(name, message);
        }
    }
}
