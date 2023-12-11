using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{

    public abstract class Movement : Command
    {
        protected Player _player;
        protected ObstacleChecker obstacleChecker;
        protected double windowWidth;
        protected double windowHeight;
        protected double stepSize;

        [ExcludeFromCodeCoverage]
        protected Movement(Player player, List<Obstacle> obstacles) : base(player)
        {
            this.obstacleChecker = new ObstacleChecker(obstacles);
            this._player = player;
            GameStats gameStat = GameStats.Instance;
            this.stepSize = gameStat.PlayerSpeed;
        }

        public override bool Execute()
        {
            player.SetCurrentDirection(GetDirectionX(), GetDirectionY());
            return Move();
        }

        public override void Undo()
        {
            player.SetCurrentDirection(-GetDirectionX(), -GetDirectionY());
            Move();
        }

        protected abstract double GetDirectionX();
        protected abstract double GetDirectionY();

        // Template method
        public bool Move()
        {
            Coordinates playerCurrent = player.GetCurrentCoords();
            Coordinates playerDirection = player.GetDirectionCoords();
            Coordinates nextCoords = player.GetNextCoords(stepSize);
            obstacleChecker.PositionNextToObstacle(playerCurrent, playerDirection, ref nextCoords);
            bool res = !(Math.Abs(playerCurrent.x - nextCoords.x) < 0.001 && Math.Abs(playerCurrent.y - nextCoords.y) < 0.001);
            player.SetCurrentPosition(nextCoords);
            return res;
        }
    }
    
    public class MoveUp : Movement
    {
        public MoveUp(Player player, List<Obstacle> obstacles) : base(player, obstacles)
        {
        }

        protected override double GetDirectionX() => 0;
        protected override double GetDirectionY() => -1;

    }

    public class MoveDown : Movement
    {
        public MoveDown(Player player, List<Obstacle> obstacles) : base(player, obstacles)
        {
        }

        protected override double GetDirectionX() => 0;
        protected override double GetDirectionY() => 1;
    }

    public class MoveLeft : Movement
    {
        public MoveLeft(Player player, List<Obstacle> obstacles) : base(player, obstacles)
        {
        }

        protected override double GetDirectionX() => -1;
        protected override double GetDirectionY() => 0;
    }

    public class MoveRight : Movement
    {
        public MoveRight(Player player, List<Obstacle> obstacles) : base(player, obstacles)
        {
        }

        protected override double GetDirectionX() => 1;
        protected override double GetDirectionY() => 0;
    }
}
