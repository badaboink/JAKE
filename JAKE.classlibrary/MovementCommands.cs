using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    
    public class MoveUp : Command
    {
        List<Obstacle> obstacles;
        double windowWidth;
        double windowHeight;
        public MoveUp(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

        public override void Execute()
        {
            player.SetCurrentDirection(0, -1);
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
        }

    }

    public class MoveDown : Command
    {
        List<Obstacle> obstacles;
        double windowWidth;
        double windowHeight;
        public MoveDown(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

        public override void Execute()
        {
            player.SetCurrentDirection(0, 1);
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
        }
    }

    public class MoveLeft : Command
    {
        List<Obstacle> obstacles;
        double windowWidth;
        double windowHeight;
        public MoveLeft(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

        public override void Execute()
        {
            player.SetCurrentDirection(-1, 0);
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
        }
    }

    public class MoveRight : Command
    {
        List<Obstacle> obstacles;
        double windowWidth;
        double windowHeight;
        public MoveRight(Player player, List<Obstacle> obstacles, double windowWidth, double windowHeight) : base(player)
        {
            this.obstacles = obstacles;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

        public override void Execute()
        {
            player.SetCurrentDirection(1, 0);
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
        }
    }

    public class Undo : Command
    {
        public Undo(Player player) : base(player)
        {
        }

        public override void Execute()
        {
            player.Undo();
        }
    }


}
