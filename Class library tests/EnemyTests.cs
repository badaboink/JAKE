﻿using JAKE.classlibrary;
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
using JAKE.classlibrary.Enemies;
using JAKE.classlibrary.Patterns.Strategies;
using Server.GameData;

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
            Player player1 = new Player(1, "a", "red", "red", "round");
            Player player2 = new Player(2, "a", "red", "red", "round");
            Player player3 = new Player(3, "a", "red", "red", "round");
            player1.SetCurrentPosition(1, 1);
            player2.SetCurrentPosition(1, 0);
            player3.SetCurrentPosition(2, 1);
            List<Player> list = new List<Player>
            {
                player1,
                player2,
                player3
            };
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

            Obstacle obstacle = new Obstacle(5.0, 5.0, 600, 600);
            var obstacles = new List<Obstacle> { obstacle };

            var patrollingStrategy = new PatrollingStrategy(maxX, maxY, enemy.GetSpeed(), obstacles);

            var directionXField = typeof(PatrollingStrategy).GetField("directionX", BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            directionXField.SetValue(patrollingStrategy, 1);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var directionYField = typeof(PatrollingStrategy).GetField("directionY", BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            directionYField.SetValue(patrollingStrategy, 0);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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

            Obstacle obstacle = new Obstacle(5.0, 5.0, 600, 600);
            var obstacles = new List<Obstacle> { obstacle };

            var patrollingStrategy = new PatrollingStrategy(maxX, maxY, enemy.GetSpeed(), obstacles);

            var directionXField = typeof(PatrollingStrategy).GetField("directionX", BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            directionXField.SetValue(patrollingStrategy, 1);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var directionYField = typeof(PatrollingStrategy).GetField("directionY", BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            directionYField.SetValue(patrollingStrategy, 0);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var players = new List<Player>();

            patrollingStrategy.Move(enemy, players);

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
            Obstacle obstacle = new Obstacle(5.0, 5.0, 600, 600);
            var obstacles = new List<Obstacle> { obstacle };
            enemy.SetSpeed(5.0);
            var strategy = new ChasePlayerStrategy(obstacles);

            Player player1 = new Player(1, "a", "red", "red", "round");
            Player player2 = new Player(2, "a", "red", "red", "round");
            Player player3 = new Player(3, "a", "red", "red", "round");
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
        public void Move_WhenClosestPlayerFound_CantMove()
        {
            Obstacle obstacle = new Obstacle(5.0, 5.0, 50, 50);
            var obstacles = new List<Obstacle> { obstacle };
            enemy.SetSpeed(5.0);
            var strategy = new ChasePlayerStrategy(obstacles);

            Player player1 = new Player(1, "a", "red", "red", "round");
            Player player2 = new Player(2, "a", "red", "red", "round");
            Player player3 = new Player(3, "a", "red", "red", "round");
            player1.SetCurrentPosition(1, 1);
            player2.SetCurrentPosition(100, 100);
            player3.SetCurrentPosition(25, 25);

            enemy.SetCurrentPosition(50, 50);
            double oldx = 50;
            double oldy = 50;

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

            Assert.Equal(oldx, enemy.GetCurrentX(), 2);
            Assert.NotEqual(oldy, enemy.GetCurrentY(), 2);
        }

        [Fact]
        public void Test_Move_WhenClosestPlayerIsNull_ShouldNotChangePosition()
        {
            Enemy enemy = new(1, "red");
            List<Player> players = new List<Player>();
            Obstacle obstacle = new Obstacle(5.0, 5.0, 600, 600);
            var obstacles = new List<Obstacle> { obstacle };
            ChaseAndHopStrategy strategy = new ChaseAndHopStrategy(obstacles);

            strategy.Move(enemy, players);

            Assert.Equal(0, enemy.GetCurrentX());
            Assert.Equal(0, enemy.GetCurrentY());
        }
        [Fact]
        public void Move_EnemyMovesAndSetsNewStrategy()
        {
            double moveDirectionX = 1.0;
            double moveDirectionY = 0.5;
            double enemySpeed = 5.0;
            Obstacle obstacle = new Obstacle(10.0, 10.0, 0, 0);

            var obstacles = new List<Obstacle> { obstacle };

            var enemy = new Enemy(1, "Red", enemySpeed, 30, 25, 15); 
            enemy.SetCurrentPosition(20, 20);
            var hoppingStrategy = new HoppingStrategy(moveDirectionX, moveDirectionY, obstacle, obstacles);

            hoppingStrategy.Move(enemy, new List<Player>());

            double newX = enemy.GetCurrentX();
            double newY = enemy.GetCurrentY();

            Assert.NotEqual(20, newX);
            Assert.NotEqual(20, newY);

            var newStrategy = enemy.GetCurrentMovementStrategy();
            Assert.IsType<ChaseAndHopStrategy>(newStrategy);
        }
        [Fact]
        public void GroupToOne_MovesTowardsKing()
        {
            Obstacle obstacle = new Obstacle(5.0, 5.0, 600, 600);
            var obstacles = new List<Obstacle> { obstacle };
            var king = new Enemy(1, "King", 5.0, 30, 25, 15);
            king.SetCurrentPosition(50, 50);
            var enemy = new Enemy(2, "Minion", 3.0, 20, 15, 10);

            king.SetMovementStrategy(new ChasePlayerStrategy(obstacles));
            var groupToOneStrategy = new GroupToOneStrategy(obstacles, king);
            enemy.SetMovementStrategy(groupToOneStrategy);

            Player player1 = new Player(1, "a", "red", "red", "round");
            Player player2 = new Player(2, "a", "red", "red", "round");
            Player player3 = new Player(3, "a", "red", "red", "round");
            player1.SetCurrentPosition(1, 1);
            player2.SetCurrentPosition(100, 100);
            player3.SetCurrentPosition(25, 25);
            var players = new List<Player>
            {
                player1, player2, player3
            };
            double initialDistance = CalculateDistance(enemy.GetCurrentX(), enemy.GetCurrentY(), king.GetCurrentX(), king.GetCurrentY());

            king.Move(players);
            groupToOneStrategy.Move(enemy, players);

            double newX = enemy.GetCurrentX();
            double newY = enemy.GetCurrentY();

            Assert.NotEqual(0.0, newX);
            Assert.NotEqual(0.0, newY);
            double newDistance = CalculateDistance(enemy.GetCurrentX(), enemy.GetCurrentY(), king.GetCurrentX(), king.GetCurrentY());
            Assert.True(newDistance < initialDistance);
        }
        [Fact]
        public void GroupToOne_CantMove()
        {
            Obstacle obstacle = new Obstacle(5.0, 5.0, 20, 20);
            var obstacles = new List<Obstacle> { obstacle };
            enemy.SetSpeed(5.0);
            var king = new Enemy(1, "King", 5.0, 30, 25, 15);
            king.SetCurrentPosition(50, 50);
            king.SetMovementStrategy(new ChasePlayerStrategy(obstacles));
            var groupToOneStrategy = new GroupToOneStrategy(obstacles, king);
            enemy.SetMovementStrategy(groupToOneStrategy);

            Player player1 = new Player(1, "a", "red", "red", "round");
            Player player2 = new Player(2, "a", "red", "red", "round");
            Player player3 = new Player(3, "a", "red", "red", "round");
            player1.SetCurrentPosition(1, 1);
            player2.SetCurrentPosition(100, 100);
            player3.SetCurrentPosition(25, 25);

            double oldx = enemy.GetCurrentX();
            double oldy = enemy.GetCurrentY();

            var players = new List<Player> { player1, player2, player3 };

            king.Move(players);
            groupToOneStrategy.Move(enemy, players);

            Assert.Equal(oldx, enemy.GetCurrentX(), 2);
            Assert.Equal(oldy, enemy.GetCurrentY(), 2);
        }
        private double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
