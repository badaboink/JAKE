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
            Base text = new Base(player);
            string decoratedText = text.DisplayObject("coin").text;

            Assert.Equal("+10 POINTS!", decoratedText);
        }
        [Fact]
        public void Test_DecorateShield()
        {
            Player player = new Player();
            ShieldOn shield = new ShieldOn(player);
            bool isShieldOn = shield.DisplayShield().shieldOn;

            Assert.True(isShieldOn);
        }
        [Fact]
        public void Test_HideShield()
        {
            Player player = new Player();
            ShieldOn shield = new ShieldOn(player);
            bool isShieldOn = shield.HideShield().shieldOn;

            Assert.False(isShieldOn);
        }
        [Fact]
        public void Test_ShowHealthBar()
        {
            Player player = new Player();
            HealthAdd health = new HealthAdd(player);
            float heart = health.DisplayHealth(80).health;

            Assert.Equal(80, heart);
        }


    }
}
