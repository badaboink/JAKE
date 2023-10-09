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
                case "healthboost":
                    return new HealthBoost();
                case "coin":
                    return new Coin();
                case "weapon":
                    return new Weapon();
                case "shield":
                    return new Shield();
                case "speedboost":
                    return new SpeedBoost();
                default:
                    throw new ArgumentException("Invalid object type");
            }
        }
    }
}
