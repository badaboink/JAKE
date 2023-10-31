using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{

    public abstract class MovementCommand : Command
    {
        protected Player _player;
        protected ObstacleChecker obstacleChecker;
        protected double windowWidth;
        protected double windowHeight;

        public MovementCommand(Player player, List<Obstacle> obstacles) : base(player)
        {
            this.obstacleChecker = new ObstacleChecker(obstacles);
            this._player = player;
        }

        public override bool Execute()
        {
            return Move();
        }

        public override void Undo()
        {
            Move();
        }

        public bool Move()
        {
            GameStats gameStat = GameStats.Instance;
            Coordinates playerCurrent = player.GetCurrentCoords();
            Coordinates playerDirection = player.GetDirectionCoords();
            double stepSize = gameStat.PlayerSpeed;
            Coordinates nextCoords = player.GetNextCoords(stepSize);
            obstacleChecker.PositionNextToObstacle(playerCurrent, playerDirection, ref nextCoords);
            player.SetCurrentPosition(nextCoords);
            return !(Math.Abs(playerCurrent.x - nextCoords.x) < 0.001 && Math.Abs(playerCurrent.y - nextCoords.y) < 0.001);
        }
    }
    
    public class MoveUp : MovementCommand
    {
        public MoveUp(Player player, List<Obstacle> obstacles) : base(player, obstacles)
        {
        }

        public override bool Execute()
        {
            player.SetCurrentDirection(0, -1);
            return base.Execute();
        }

        public override void Undo()
        {
            player.SetCurrentDirection(0, 1);
            base.Undo();
        }

    }

    public class MoveDown : MovementCommand
    {
        public MoveDown(Player player, List<Obstacle> obstacles) : base(player, obstacles)
        {
        }

        public override bool Execute()
        {
            player.SetCurrentDirection(0, 1);
            return base.Execute();
        }

        public override void Undo()
        {
            player.SetCurrentDirection(0, -1);
            base.Execute();
        }
    }

    public class MoveLeft : MovementCommand
    {
        public MoveLeft(Player player, List<Obstacle> obstacles) : base(player, obstacles)
        {
        }

        public override bool Execute()
        {
            player.SetCurrentDirection(-1, 0);
            return base.Execute();
        }

        public override void Undo()
        {
            player.SetCurrentDirection(1, 0);
            base.Execute();
        }
    }

    public class MoveRight : MovementCommand
    {
        public MoveRight(Player player, List<Obstacle> obstacles) : base(player, obstacles)
        {
        }

        public override bool Execute()
        {
            player.SetCurrentDirection(1, 0);
            return base.Execute();
        }

        public override void Undo()
        {
            player.SetCurrentDirection(-1, 0);
            base.Undo();
        }
    }


}
