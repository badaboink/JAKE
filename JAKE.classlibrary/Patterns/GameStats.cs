using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class GameStats
    {
        private static GameStats? instance = null;
        private static readonly object lockObject = new object();

        private int playersCount;
        private float playerHealth;
        private int playerScore;
        private int playerSpeed;
        private bool shieldOn;
        private double windowHeight;
        private double windowWidth;
        public string state { get; set; }

        public double WindowWidth
        {
            get { return windowWidth; }
            set { windowWidth = value; }
        }
        public double WindowHeight
        {
            get { return windowHeight; }
            set { windowHeight = value; }
        }

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

        public int PlayerSpeed
        {
            get { return playerSpeed; }
            set { playerSpeed = value; }
        }
        public bool ShieldOn
        {
            get { return shieldOn; }
            set { shieldOn = value; }
        }
        private GameStats()
        {
            playersCount = 0;
            playerHealth = 100f;
            playerScore = 0;
            playerSpeed = 10;
            shieldOn = false;
            state = "alive";
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
            set
            {
                if (instance != null)
                {
                    lock (lockObject)
                    {
                        if (instance != null)
                        {
                            instance = null;
                        }
                    }
                }
                
            }
        }

    }
}
