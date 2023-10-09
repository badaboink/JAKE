using JAKE.classlibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_library_tests
{
    public class EnemyTests
    {
        private readonly Enemy enemy;
        public EnemyTests()
        {
            enemy = new Enemy(1, "red");
            enemy.SetCurrentPosition(0, 0);
        }
        [Fact]
        public void Test_Closest_Player_1()
        {
            Player player1 = new Player(1, "a", "red");
            Player player2 = new Player(2, "a", "red");
            Player player3 = new Player(3, "a", "red");
            player1.SetCurrentPosition(1, 1);
            player2.SetCurrentPosition(1, 0);
            player3.SetCurrentPosition(2, 1);
            List<Player> list = new List<Player>();
            list.Add(player1);
            list.Add(player2);
            list.Add(player3);
            Assert.Equal(player2, enemy.FindClosestPlayer(list));
        }
    }
}
