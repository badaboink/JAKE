using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;

namespace Class_library_tests
{
    public class CommandTests
    {

        public CommandTests()
        {
            GameStats gameStats = GameStats.Instance;
            gameStats.PlayerSpeed = 10;
            gameStats.WindowHeight = 500;
            gameStats.WindowWidth = 500;
        }

        [Fact]
        public void Test_Execute_Move_Up()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveUp(player, new List<Obstacle>());

            command.Execute();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(40, player.GetCurrentY());
        }

        [Fact]
        public void Test_Undo_Move_Up()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveUp(player, new List<Obstacle>());

            command.Execute();
            command.Undo();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(50, player.GetCurrentY());
        }

        [Fact]
        public void Test_Execute_Move_Left()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveLeft(player, new List<Obstacle>());

            command.Execute();

            Assert.Equal(40, player.GetCurrentX());
            Assert.Equal(50, player.GetCurrentY());
        }

        [Fact]
        public void Test_Undo_Move_Left()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveLeft(player, new List<Obstacle>());

            command.Execute();
            command.Undo();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(50, player.GetCurrentY());
        }

        [Fact]
        public void Test_Execute_Move_Right()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveRight(player, new List<Obstacle>());

            command.Execute();

            Assert.Equal(60, player.GetCurrentX());
            Assert.Equal(50, player.GetCurrentY());
        }

        [Fact]
        public void Test_Undo_Move_Right()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveRight(player, new List<Obstacle>());

            command.Execute();
            command.Undo();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(50, player.GetCurrentY());
        }

        [Fact]
        public void Test_Execute_Move_Down()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveDown(player, new List<Obstacle>());

            command.Execute();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(60, player.GetCurrentY());
        }

        [Fact]
        public void Test_Undo_Move_Down()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveDown(player, new List<Obstacle>());

            command.Execute();
            command.Undo();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(50, player.GetCurrentY());
        }

        [Fact]
        public void Test_Controller_Execute()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveDown(player, new List<Obstacle>());
            Controller controller = new Controller();
            controller.SetCommand(command);

            controller.Execute();
            controller.Execute();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(70, player.GetCurrentY());
        }

        [Fact]
        public void Test_Controller_Execute_null()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveDown(player, new List<Obstacle>());
            Controller controller = new Controller();

            controller.Execute();
            controller.Execute();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(50, player.GetCurrentY());
        }

        [Fact]
        public void Test_Controller_Execute_Undo()
        {
            GameStats.Instance.PlayerSpeed = 10;
            Player player = new Player();
            player.SetCurrentPosition(50, 50);
            Command command = new MoveDown(player, new List<Obstacle>());
            Controller controller = new Controller();
            controller.SetCommand(command);

            controller.Execute();
            controller.Undo();
            controller.Execute();
            controller.Undo();

            Assert.Equal(50, player.GetCurrentX());
            Assert.Equal(50, player.GetCurrentY());
        }
    }
}
