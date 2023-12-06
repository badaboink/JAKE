﻿using JAKE.classlibrary.Collectibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IGameEntityVisitor
    {
        // Other visit methods for different entities
        void VisitCoin(Coin coin, Player player);
        void VisitSpeedBoost(SpeedBoost speedBoost);
        void VisitHealthBoost(HealthBoost healthBoost);
        void VisitShield(Shield shield);
        void VisitCorona(Corona corona);

    }
}
