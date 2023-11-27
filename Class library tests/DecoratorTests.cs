using JAKE.classlibrary;
using JAKE.classlibrary.Collectibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_library_tests
{
    public class DecoratorTests
    {
        [Fact]
        public void Test_DecorateText()
        {
            Player player = new Player();
            CoinDecorator text = new CoinDecorator(player);
            string decoratedText = text.Display(10, false).text;

            Assert.Equal("+10 Points!", decoratedText);
        }
        [Fact]
        public void Test_DecorateShield()
        {
            Player player = new Player();
            ShieldDecorator shield = new ShieldDecorator(player);
            bool isShieldOn = shield.Display(10, true).shieldOn;

            Assert.True(isShieldOn);
        }
        [Fact]
        public void Test_ShowHealthBar()
        {
            Player player = new Player();
            HealthDecorator health = new HealthDecorator(player);
            float heart = health.Display(80, false).health;

            Assert.Equal(80 / 2, heart);
        }


    }
}
