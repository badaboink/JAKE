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
        private static readonly object lockObject = new();
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

        public int PlayersCount { get; set; }

        public float PlayerHealth { get; set; }

        public int PlayerScore { get; set; }

        public int PlayerSpeed { get; set; }
        public bool ShieldOn { get; set; }
        public int Level { get; set; }
        private GameStats()
        {
            PlayersCount = 0;
            PlayerHealth = 100f;
            PlayerScore = 0;
            PlayerSpeed = 10;
            ShieldOn = false;
            state = "alive";
            Level = 1;
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
