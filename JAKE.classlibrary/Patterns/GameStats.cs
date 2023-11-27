﻿using System;
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

        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }

        public int PlayersCount { get; set; }

        public float PlayerHealth { get; set; }

        public int PlayerScore { get; set; }

        public int PlayerSpeed { get; set; }
        public bool ShieldOn { get; set; }
        private GameStats()
        {
            PlayersCount = 0;
            PlayerHealth = 100f;
            PlayerScore = 0;
            PlayerSpeed = 10;
            ShieldOn = false;
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
