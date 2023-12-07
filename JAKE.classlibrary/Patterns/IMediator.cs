using JAKE.classlibrary.Collectibles;
using JAKE.classlibrary.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IMediator
    {
        void SendMessage(string message, string sender = null, string receiver = null);
    }
}
