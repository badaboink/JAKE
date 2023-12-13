using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.Flyweight
{
    public abstract class Flyweight
    {
        protected string color;

        public abstract void Display(string text);
    }

    public class FlyweightFactory
    {
        private Dictionary<string, Flyweight> _flyweightMap = new Dictionary<string,Flyweight>();

        public Flyweight GetFlyweight(string key)
        {
            Flyweight flyweight;
            if (_flyweightMap.ContainsKey(key))
            {
                flyweight = _flyweightMap[key];
                return flyweight;
            }
            return null;
        }

        public void SetFlyweight(Flyweight flyweight, string key)
        {
            _flyweightMap[key] = flyweight;
        }
    }
}
