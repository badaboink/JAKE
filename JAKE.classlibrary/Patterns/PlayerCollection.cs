using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public class PlayerCollection : IPlayerCollection
    {
        private List<Player> players;
        public PlayerCollection(List<Player> players)
        {
            this.players = players;
        }

        public List<Player> getPlayers()
        {
            return players;
        }
        public IPlayerIterator CreateNearbyPlayersIterator(Player coronaInfectedPlayer)
        {
            // Logic to iterate through nearby players
            // based on coronaInfectedPlayer's position
            return new NearbyPlayerIterator(this.players, coronaInfectedPlayer);
        }

        IPlayerIterator IPlayerCollection.CreateNearbyPlayersIterator(Player coronaInfectedPlayer)
        {
            throw new NotImplementedException();
        }
    }
}
