using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class PlayerRandom : IPlayerEngine
    {
        public void Move(Game game, Player asPlayer)
        {
            (int x, int y) pos = Game.getFreeRandomPos(game);
            game.setState(pos.x, pos.y, asPlayer);
        }
    }
}
