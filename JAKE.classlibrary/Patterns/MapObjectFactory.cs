using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary.Collectibles;

namespace JAKE.classlibrary.Patterns
{
    public class MapObjectFactory
    {
        public IMapObject CreateMapObject(string objectType, int value, int weapsp = 0, int weapstr = 0)
        {
            switch (objectType.ToLower())
            {
                case "healthboost":
                    return new HealthBoost(value);
                case "coin":
                    return new Coin(value);
                case "weapon":
                    return new Weapon(value, weapsp, weapstr);
                case "shield":
                    return new Shield(value);
                case "speedboost":
                    return new SpeedBoost(value);
                default:
                    throw new ArgumentException("Invalid object type");
            }
        }
    }
}
