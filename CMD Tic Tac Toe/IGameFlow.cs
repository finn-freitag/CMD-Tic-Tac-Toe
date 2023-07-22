using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public interface IGameFlow
    {
        /// <summary>
        /// The game.
        /// </summary>
        Game Game { get; }

        /// <summary>
        /// The player who performs the next move.
        /// </summary>
        Player CurrentPlayer { get; }

        /// <summary>
        /// Starts the game.
        /// </summary>
        void Start();
    }
}
