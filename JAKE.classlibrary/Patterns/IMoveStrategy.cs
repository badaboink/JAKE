﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IMoveStrategy
    {
        void Move(Enemy enemy, List<Player> players);
        void MoveZombie(Zombie zombie, List<Player> players);
    }
}
