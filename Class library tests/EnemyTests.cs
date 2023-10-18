using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
        [Fact]
        public void Test_PatrollingStrategy_ShouldUpdatePosition()
        {
            double maxX = 100;
            double maxY = 100;
            enemy.SetSpeed(5.0);
            enemy.SetCurrentPosition(20, 20);
            double presumed_x = enemy.GetCurrentX()+1*enemy.GetSpeed();
            double presumed_y = enemy.GetCurrentY();
            var obstacles = new List<Obstacle>();

            var patrollingStrategy = new PatrollingStrategy(maxX, maxY, enemy.GetSpeed(), obstacles);

            var directionXField = typeof(PatrollingStrategy).GetField("directionX", BindingFlags.NonPublic | BindingFlags.Instance);
            directionXField.SetValue(patrollingStrategy, 1);
            var directionYField = typeof(PatrollingStrategy).GetField("directionY", BindingFlags.NonPublic | BindingFlags.Instance);
            directionYField.SetValue(patrollingStrategy, 0);

            var players = new List<Player>();

            patrollingStrategy.Move(enemy, players);

            Assert.InRange(enemy.GetCurrentX(), enemy.GetSize(), maxX);
            Assert.InRange(enemy.GetCurrentY(), enemy.GetSize(), maxY);
            Assert.Equal(presumed_x, enemy.GetCurrentX());
            Assert.Equal(presumed_y, enemy.GetCurrentY());
        }

        [Fact]
        public void Test_PatrollingStrategy_ShouldUpdateDirection()
        {
            double maxX = 100;
            double maxY = 100;
            enemy.SetSpeed(5.0);
            enemy.SetCurrentPosition(100, 20);
            double presumed_x = enemy.GetCurrentX();
            double presumed_y = enemy.GetCurrentY();
            var obstacles = new List<Obstacle>();

            var patrollingStrategy = new PatrollingStrategy(maxX, maxY, enemy.GetSpeed(), obstacles);

            var directionXField = typeof(PatrollingStrategy).GetField("directionX", BindingFlags.NonPublic | BindingFlags.Instance);
            directionXField.SetValue(patrollingStrategy, 1);
            var directionYField = typeof(PatrollingStrategy).GetField("directionY", BindingFlags.NonPublic | BindingFlags.Instance);
            directionYField.SetValue(patrollingStrategy, 0);

            var players = new List<Player>();

            patrollingStrategy.Move(enemy, players);

            // if direction is changed enemy does not move till next assertion to move
            // might be problematic, but good for now
            Assert.InRange(enemy.GetCurrentX(), enemy.GetSize(), maxX);
            Assert.InRange(enemy.GetCurrentY(), enemy.GetSize(), maxY);
            Assert.Equal(presumed_x, enemy.GetCurrentX());
            Assert.Equal(presumed_y, enemy.GetCurrentY());
            Assert.NotEqual(1, patrollingStrategy.GetCurrentX());
            Assert.NotEqual(0, patrollingStrategy.GetCurrentY());
        }
        [Fact]
        public void Move_WhenClosestPlayerFound_ShouldUpdateEnemyPosition()
        {
            var obstacles = new List<Obstacle>();
            enemy.SetSpeed(5.0);
            var strategy = new ChasePlayerStrategy(obstacles);

            Player player1 = new Player(1, "a", "red");
            Player player2 = new Player(2, "a", "red");
            Player player3 = new Player(3, "a", "red");
            player1.SetCurrentPosition(1, 1);
            player2.SetCurrentPosition(100, 100);
            player3.SetCurrentPosition(25, 25);

            enemy.SetCurrentPosition(50, 50);

            double directionX = player3.GetCurrentX() - enemy.GetCurrentX();
            double directionY = player3.GetCurrentY() - enemy.GetCurrentY();
            double length = Math.Sqrt(directionX * directionX + directionY * directionY);
            if (length > 0)
            {
                directionX /= length;
                directionY /= length;
            }
            double presumed_x = enemy.GetCurrentX() + directionX * enemy.GetSpeed();
            double presumed_y = enemy.GetCurrentY() + directionY * enemy.GetSpeed();


            var players = new List<Player> { player1, player2, player3 };

            strategy.Move(enemy, players);

            Assert.Equal(presumed_x, enemy.GetCurrentX(), 2);
            Assert.Equal(presumed_y, enemy.GetCurrentY(), 2);
        }

        [Fact]
        public void Test_Move_WhenClosestPlayerIsNull_ShouldNotChangePosition()
        {
            // Arrange
            Enemy enemy = new(1, "red");
            List<Player> players = new List<Player>();
            var obstacles = new List<Obstacle>();
            ChaseAndHopStrategy strategy = new ChaseAndHopStrategy(obstacles);

            // Act
            strategy.Move(enemy, players);

            // Assert
            Assert.Equal(0, enemy.GetCurrentX());
            Assert.Equal(0, enemy.GetCurrentY());
        }

    }
}
