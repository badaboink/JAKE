﻿using JAKE.classlibrary;
using JAKE.classlibrary.Enemies;
using JAKE.classlibrary.Collectibles;
using JAKE.classlibrary.Patterns;

namespace Server.GameData
{
    public interface IGameDataService
    {
        // Add a new player and return the player object
        Player AddPlayer(string playerName, string playerColor, string connectionID, string shotcolor, string shotshape);

        Player RemovePlayer(string connectionID);

        // Get player data as a list of strings
        List<string> GetPlayerList();

        string GetPlayerData(int id);
        void EditPlayerPosition(int id, double x, double y);
        void EditPlayerState(int id, string state, string color);

        void UpdateStatePlayer(int id, string state);

        // Get obstacle data as a list of strings
        string GetObstacleData();
        Enemy AddEnemies();
        void RemoveEnemy(int id);
        void UpdateEnemy(int id, int health);
        List<string> GetEnemies();
        List<string> UpdateEnemyPositions();
        DateTime GetCurrentGameTime();
        void SetGameTime(DateTime gametime);
        void AddObserver(string connectionID, Observer observer);
        void RemoveObserver(string connectionID);
        Dictionary<string, Observer> GetObservers();

        Coin AddCoin(int points);
        void RemoveCoin(int id);
        List<string> GetCoins();
        Shield AddShield(int time);
        void RemoveShield(int id);
        List<string> GetShields();
        HealthBoost AddHealthBoost(int health);
        void RemoveHealthBoost(int id);
        List<string> GetHealthBoosts();
        SpeedBoost AddSpeedBoost(int speed);
        void RemoveSpeedBoost(int id);
        List<string> GetSpeedBoosts();
        Corona AddCorona();
        void RemoveCorona(int id);
        List<string> GetCoronas();

        Coin ReturnCoin(int id);
        Shield ReturnShield(int id);
        Enemy AddZombieBoss();
        bool IsBossAlive();
        int GetLevel();
        void SetLevel(int level);
        List<Player> InfectCorona(int id, double x, double y);
    }
}
