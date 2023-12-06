using JAKE.classlibrary.Collectibles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class GameEntityVisitor : IGameEntityVisitor
    {
        public void VisitCoin(Coin coin, Player player)
        {          
            // Logic from HandleCoinsCollisions method goes here
            //double playerX = currentPlayer.GetCurrentX();
            //double playerY = currentPlayer.GetCurrentY();

            //double coinX = Canvas.GetLeft(coinVisuals[coin]);
            //double coinY = Canvas.GetTop(coinVisuals[coin]);

            //if (playerTouchesMapObject(playerX, playerY, playerVisual.Height, coinX, coinY, coinRect.Height))
            //{
            //    GameStats gameStat = GameStats.Instance;
            //    coin.Interact(gameStat);
            //    scoreLabel.Text = $"Score: {gameStat.PlayerScore}";
            //    Player text = new CoinDecorator(currentPlayer);
            //    testLabel.Text = text.Display(gameStat.PlayerHealth, gameStat.ShieldOn).text;
            //    HideDisplay();

            //    string json = JsonConvert.SerializeObject(coin);
            //    await connection.SendAsync("SendPickedCoin", json);
            //}
            
        }

        public void VisitCorona(Corona corona)
        {
            throw new NotImplementedException();
        }

        public void VisitHealthBoost(HealthBoost healthBoost)
        {
            throw new NotImplementedException();
        }

        public void VisitShield(Shield shield)
        {
            throw new NotImplementedException();
        }

        public void VisitSpeedBoost(SpeedBoost speedBoost)
        {
            throw new NotImplementedException();
        }
    }
}
