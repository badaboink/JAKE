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
        protected List<Obstacle> obstacles;
        protected double windowWidth;
        protected double windowHeight;

        public MovementCommand(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
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
            double playerCurrentX = player.GetCurrentX();
            double playerCurrentY = player.GetCurrentY();
            double playerDirectionX = player.GetDirectionX();
            double playerDirectionY = player.GetDirectionY();
            double stepSize = gameStat.PlayerSpeed;
            double newX = playerCurrentX + playerDirectionX * stepSize;
            double newY = playerCurrentY + playerDirectionY * stepSize;
            Check(obstacles, ref newX, ref newY, windowWidth, windowHeight);
            player.SetCurrentPosition(newX, newY);
            return !(Math.Abs(playerCurrentX - newX) < 0.001 && Math.Abs(playerCurrentY - newY) < 0.001);
        }
    }
    
    public class MoveUp : MovementCommand
    {
        public MoveUp(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player, obstacles, windowWidth, windowHeight)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
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
        public MoveDown(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player, obstacles, windowWidth, windowHeight)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
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
        public MoveLeft(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player, obstacles, windowWidth, windowHeight)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
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
        public MoveRight(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player, obstacles, windowWidth, windowHeight)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
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
