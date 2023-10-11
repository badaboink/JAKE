using JAKE.classlibrary;

namespace Server.GameData
{
    public interface IGameDataService
    {
        // Add a new player and return the player object
        Player AddPlayer(string playerName, string playerColor, string connectionID);

        Player RemovePlayer(string connectionID);

        // Get player data as a list of strings
        List<string> GetPlayerList();

        string GetPlayerData(int id);
        void EditPlayerPosition(int id, double x, double y);

        void UpdateDeadPlayer(int id);

        // Get obstacle data as a list of strings
        string GetObstacleData();
        Enemy AddEnemies();
        void RemoveEnemy(int id);
        void UpdateEnemy(int id, int health);
        List<string> GetEnemies();
        List<string> UpdateEnemyPositions();
        //Player FindClosestPlayer(Enemy enemy);
        DateTime GetCurrentGameTime();
        void SetGameTime(DateTime gametime);
        void AddObserver(string connectionID, Observer observer);
        void RemoveObserver(string connectionID);
        Dictionary<string, Observer> GetObservers();

        //IMapObject AddCoin();
        //void RemoveCoin(int id);
        //List<string> GetCoins();
    }
}
