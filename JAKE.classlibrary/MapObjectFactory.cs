using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary
{
    public class MapObjectFactory
    {
        public IMapObject CreateMapObject(string objectType)
        {
            switch (objectType.ToLower())
            {
                case "healthboost10":
                    return new HealthBoost(10);
                case "healthboost20":
                    return new HealthBoost(20);
                case "coin":
                    return new Coin(10);
                case "weapon":
                    return new Weapon(30,10,15);
                case "shield":
                    return new Shield(30);
                case "speedboost":
                    return new SpeedBoost(25);
                default:
                    throw new ArgumentException("Invalid object type");
            }
        }
    }
}
