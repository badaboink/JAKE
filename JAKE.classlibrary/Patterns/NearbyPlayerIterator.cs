using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class NearbyPlayerIterator : IPlayerIterator
    {
        private List<Player> players;
        private Player coronaInfectedPlayer;
        private int currentPosition;

        public NearbyPlayerIterator(List<Player> players, Player coronaInfectedPlayer)
        {
            this.players = players;
            this.coronaInfectedPlayer = coronaInfectedPlayer;
            this.currentPosition = 0;
        }

        public Player GetNext()
        {
            if (HasMore())
            {
                Player nextPlayer = players[currentPosition];
                currentPosition++;
                return nextPlayer;
            }
            //currentPosition = 0;
            //GetNext();
            return null; // Or handle accordingly if there are no more players
        }

        public bool HasMore()
        {

            return currentPosition < players.Count &&
                   DistanceBetweenPlayers(players[currentPosition], coronaInfectedPlayer) <= 100 ;
        }

        private double DistanceBetweenPlayers(Player player1, Player player2)
        {
            // Logic to calculate distance between players
            // Replace this with your actual distance calculation method
            return Math.Sqrt(Math.Pow(player2.GetCurrentX() - player1.GetCurrentX(), 2) + Math.Pow(player2.GetCurrentY() - player1.GetCurrentY(), 2));
        }

    }
}
