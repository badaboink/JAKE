using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Patterns;

namespace JAKE.classlibrary.Collectibles
{
    [ExcludeFromCodeCoverage]
    public class HealthBoost : IMapObject
    {
        public int Health { get;  set; }

        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Image { get; set; }
        public HealthBoost(int id, double x, double y, int health, string image)
        {
            Health = health;
            Width = 20;
            Height = 20;
            Image = image;
            X = x;
            Y = y;
            this.id = id;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public HealthBoost(int health)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.Health = health;
        }
        public void Interact(GameStats gameStats)
        {
            gameStats.PlayerHealth += this.Health;
        }
        public bool MatchesId(int id)
        {
            return this.id == id;
        }

        public void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"{id}:{X}:{Y}:{Width}:{Height}:{Health}";
        }

        public void Accept(IGameEntityVisitor visitor)
        {
            visitor.VisitHealthBoost(this);
        }
    }
}
