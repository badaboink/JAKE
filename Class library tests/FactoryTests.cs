using JAKE.classlibrary;
using JAKE.classlibrary.Collectibles;
using JAKE.classlibrary.Enemies;
using JAKE.classlibrary.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_library_tests
{
    public class FactoryTests
    {
        //coin
        [Fact]
        public void Test_Create_Coin_With_Factory()
        {
            MapObjectFactory objectFactory = new MapObjectFactory();
            Coin coinFromFactory = (Coin)objectFactory.CreateMapObject("coin", 10);

            int expectedCoinPoints = 10;
            int coinPoints = coinFromFactory.Points;

            Assert.Equal(expectedCoinPoints, coinPoints);
        }

        //healthboost
        [Fact]
        public void Test_Create_HealthBoost_With_Factory()
        {
            MapObjectFactory objectFactory = new MapObjectFactory();
            HealthBoost HealthBoostFromFactory = (HealthBoost)objectFactory.CreateMapObject("healthboost", 10);

            int expectedHealthBoostValue = 10;
            int healthBoostValue = HealthBoostFromFactory.Health;

            Assert.Equal(expectedHealthBoostValue, healthBoostValue);
        }

        //speedboost
        [Fact]
        public void Test_Create_SpeedBoost_With_Factory()
        {
            MapObjectFactory objectFactory = new MapObjectFactory();
            SpeedBoost SpeedBoostFromFactory = (SpeedBoost)objectFactory.CreateMapObject("speedboost", 10);

            int expectedSpeedBoostValue = 10;
            int speedBoostValue = SpeedBoostFromFactory.Speed;

            Assert.Equal(expectedSpeedBoostValue, speedBoostValue);
        }
        //shield
        [Fact]
        public void Test_Create_Shield_With_Factory()
        {
            MapObjectFactory objectFactory = new MapObjectFactory();
            Shield shieldFromFactory = (Shield)objectFactory.CreateMapObject("shield", 10);

            int expectedShieldTime = 10;
            int shieldTime = shieldFromFactory.Time;

            Assert.Equal(expectedShieldTime, shieldTime);
        }
        //exeption
        [Fact]
        public void Test_Create_Exeption_With_Factory()
        {
            MapObjectFactory objectFactory = new MapObjectFactory();

            Assert.Throws<ArgumentException>(() => objectFactory.CreateMapObject("mushroom", 10));
        }

        [Fact]  //interact coin
        public void Test_Interact_Player_Coin()
        {
            GameStats gameStat = GameStats.Instance;
            Coin coin = new Coin(10);
            coin.Interact(gameStat);

            int expectedPoints = 10;
            int points = gameStat.PlayerScore;

            Assert.Equal(expectedPoints, points);
            GameStats.Instance = null;

        }

        [Fact]  //interact healthboost
        public void Test_Interact_Player_HealthBoost()
        {
            GameStats gameStat = GameStats.Instance;
            HealthBoost healthBoost = new HealthBoost(10);
            healthBoost.Interact(gameStat);

            float expectedHealth = 110; //100 pradinis
            float health = gameStat.PlayerHealth;

            Assert.Equal(expectedHealth, health);
            GameStats.Instance = null;
        }
        //interact speedboost
        [Fact]
        public void Test_Interact_Player_SpeedBoost()
        {
            GameStats gameStat = GameStats.Instance;
            SpeedBoost speedBoost = new SpeedBoost(10);
            speedBoost.Interact(gameStat);

            int expectedSpeed = 20; //10 pradinis
            int speed = gameStat.PlayerSpeed;

            Assert.Equal(expectedSpeed, speed);
            GameStats.Instance = null;

        }
        //interact shield
        [Fact]
        public void Test_Interact_Player_Shield()
        {
            GameStats gameStat = GameStats.Instance;
            Shield shield = new Shield(10);
            shield.Interact(gameStat);
            Assert.True(gameStat.ShieldOn);
            GameStats.Instance = null;

        }
    }
}
