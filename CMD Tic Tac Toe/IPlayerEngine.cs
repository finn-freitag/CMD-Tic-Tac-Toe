using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public interface IPlayerEngine
    {
        /// <summary>
        /// This function makes the player engine perform movements in the game.
        /// </summary>
        /// <param name="game">The game to act.</param>
        /// <param name="asPlayer">The player who act.</param>
        void Move(Game game, Player asPlayer);
    }
}
