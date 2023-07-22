using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class PlayerMixer : IPlayerEngine
    {
        public static Random random = new Random();

        List<(int percentage, IPlayerEngine player)> players = new List<(int, IPlayerEngine)> ();

        public void Move(Game game, Player asPlayer)
        {
            int r = random.Next(100) + 1;
            int currentpos = 0;
            for(int i = 0; i < players.Count; i++)
            {
                if(r <= players[i].percentage + currentpos)
                {
                    players[i].player.Move(game, asPlayer);
                    return;
                }
                currentpos += players[i].percentage;
            }
        }

        /// <summary>
        /// Attention: The sum of all player's percentages have to 100! Otherwise the 'PlayerMixer' doesn't work properly.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="percentage">The probability that this player makes to move.</param>
        public void AddPlayerType(IPlayerEngine player, int percentage)
        {
            players.Add((percentage, player));
        }
    }
}
