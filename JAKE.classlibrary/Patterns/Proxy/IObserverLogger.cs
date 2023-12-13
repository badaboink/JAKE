using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.Proxy
{
    public interface IObserverLogger
    {
        public Task SendPlayerMessage(string name, string message);
    }
}
