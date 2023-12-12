using JAKE.classlibrary.Collectibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IGameEntityVisitor
    {
        void VisitCoin(Coin coin);
        void VisitSpeedBoost(SpeedBoost speedBoost);
        void VisitHealthBoost(HealthBoost healthBoost);
        void VisitShield(Shield shield);

    }
}
