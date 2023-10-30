using JAKE.classlibrary.Patterns;

namespace Class_library_tests
{
    public class SingletonTests
    {
        [Fact]
        public void SingletonPattern_InstanceIsNotNull()
        {
            // Arrange
            var instance = GameStats.Instance;

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void SingletonPattern_ReturnsSameInstance()
        {
            // Arrange
            var instance1 = GameStats.Instance;
            var instance2 = GameStats.Instance;

            // Assert
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void DefaultValues_AreSetCorrectly()
        {
            // Arrange
            var gameStats = GameStats.Instance;

            // Assert
            Assert.Equal(0, gameStats.PlayersCount);
            Assert.Equal(100f, gameStats.PlayerHealth);
            Assert.Equal(0, gameStats.PlayerScore);
            Assert.Equal(10, gameStats.PlayerSpeed);
            Assert.False(gameStats.ShieldOn);
        }

        [Fact]
        public void Setters_Getters_WorkCorrectly()
        {
            // Arrange
            var gameStats = GameStats.Instance;

            // Act
            gameStats.PlayersCount = 5;
            gameStats.PlayerHealth = 80f;
            gameStats.PlayerScore = 50;
            gameStats.PlayerSpeed = 15;
            gameStats.ShieldOn = true;

            // Assert
            Assert.Equal(5, gameStats.PlayersCount);
            Assert.Equal(80f, gameStats.PlayerHealth);
            Assert.Equal(50, gameStats.PlayerScore);
            Assert.Equal(15, gameStats.PlayerSpeed);
            Assert.True(gameStats.ShieldOn);
        }
    }
}