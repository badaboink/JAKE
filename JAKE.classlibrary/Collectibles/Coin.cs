using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JAKE.classlibrary.Patterns;


namespace JAKE.classlibrary.Collectibles
{
    [ExcludeFromCodeCoverage]
    public class Coin : IMapObject, IEntity
    {

        public int Points { get; set; }
        public int id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public string? Image { get; set; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public Coin(int id, double x, double y, int points, string image)
        {
            this.id = id;
            X = x;
            Y = y;
            Width = 20;
            Height = 20;
            Points = points;
            Image = image;
        }

        public Coin(int id, double x, double y, int points)
        {
            this.id = id;
            X = x;
            Y = y;
            Width = 20;
            Height = 20;
            Points = points;
        }
        public Coin(int points)
        {
            this.Points = points;
        }
        public Coin()
        {
            // Parameterless constructor required for XML  serialization
        }

        public void Interact(GameStats gameStats)
        {
            gameStats.PlayerScore += this.Points;
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
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override bool Equals(object obj)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {
            if (obj is Coin otherCoin)
            {
                return this.MatchesId(otherCoin.id);
            }
            return false;
        }

        public void SetPoints(int points)
        {
            this.Points = points;
        }
        public override string ToString()
        {
            return $"{id}:{X}:{Y}:{Width}:{Height}:{Points}";
        }
        public string ToXML()
        {
            using var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public void Accept(IGameEntityVisitor visitor)
        {
            visitor.VisitCoin(this);
        }
    }
}