using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class GameStats
    {
        private static GameStats instance = null;
        private static readonly object lockObject = new object();

        private int playersCount;
        private float playerHealth;
        private int playerScore;

        public int PlayersCount
        {
            get { return playersCount; }
            set { playersCount = value; }
        }

        public float PlayerHealth
        {
            get { return playerHealth; }
            set { playerHealth = value; }
        }

        public int PlayerScore
        {
            get { return playerScore; }
            set { playerScore = value; }
        }

        private GameStats()
        {
            playersCount = 0;
            playerHealth = 100f;
            playerScore = 0;
        }

        public static GameStats Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new GameStats();
                        }
                    }
                }
                return instance;
            }
        }

    }
}
