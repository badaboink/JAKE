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
#pragma warning disable CA1822 // Mark members as static
        public IMapObject CreateMapObject(string objectType, int value, int weapsp = 0, int weapstr = 0)
#pragma warning restore CA1822 // Mark members as static
        {
            return objectType.ToLower() switch
            {
                "healthboost" => new HealthBoost(value),
                "coin" => new Coin(value),
                "weapon" => new Weapon(value, weapsp, weapstr),
                "shield" => new Shield(value),
                "speedboost" => new SpeedBoost(value),
                "corona" => new Corona(),
                _ => throw new ArgumentException("Invalid object type"),
            };
        }
    }
}
