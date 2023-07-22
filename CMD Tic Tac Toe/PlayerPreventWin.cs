using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class PlayerPreventWin : IPlayerEngine
    {
        public PlayerPreventWin()
        {

        }

        public void Move(Game game, Player asPlayer)
        {
            var winningPlayer = game.getWinningPlayer();
            if (winningPlayer.Length > 0)
            {
                for (int i = 0; i < winningPlayer.Length; i++)
                {
                    if (winningPlayer[i].winningPlayer == asPlayer)
                    {
                        game.setState(winningPlayer[i].x, winningPlayer[i].y, asPlayer);
                    }
                }
                game.setState(winningPlayer[0].x, winningPlayer[0].y, asPlayer);
                return;
            }
            var pos = Game.getFreeRandomPos(game);
            game.setState(pos.x, pos.y, asPlayer);
        }
    }
}
