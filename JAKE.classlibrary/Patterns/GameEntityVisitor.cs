using JAKE.classlibrary.Collectibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class GameEntityVisitor : IGameEntityVisitor
    {
        public void VisitCoin(Coin coin)
        {
            coin.Points += 10;
            
        }

        public void VisitHealthBoost(HealthBoost healthBoost)
        {
            healthBoost.Health += 10;
        }

        public void VisitShield(Shield shield)
        {
            shield.Time += 5;
        }

        public void VisitSpeedBoost(SpeedBoost speedBoost)
        {
            speedBoost.Time += 5;
        }
    }
}
