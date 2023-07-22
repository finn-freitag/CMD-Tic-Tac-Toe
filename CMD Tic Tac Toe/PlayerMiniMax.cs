using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class PlayerMiniMax : IPlayerEngine
    {
        public void Move(Game game, Player asPlayer)
        {
            int index = minimax(game, asPlayer).bestMove;
            game.setState(index / 3, index % 3, asPlayer);
        }

        private (int maxval, int bestMove) minimax(Game game, Player asPlayer) // https://de.wikipedia.org/wiki/Minimax-Algorithmus
        {
            if (game.checkDraw() || game.checkForWin() != Player.None)
            {
                return (evaluate(game, asPlayer), -1);
            }
            int maxval = Int32.MinValue;
            int bestMove = 0;
            for (int i = 0; i < game.gamearea.Length; i++)
            {
                int x = i / 3;
                int y = i % 3;
                if (game.isFree(x, y))
                {
                    Game g = (Game)game.Clone();
                    g.setState(x, y, asPlayer);

                    int subval = -minimax(g, asPlayer == Player.Player1 ? Player.Player2 : Player.Player1).maxval;

                    if(subval > maxval)
                    {
                        maxval = subval;
                        bestMove = i;
                    }
                }
            }
            return (maxval, bestMove);
        }

        private int evaluate(Game game, Player asPlayer)
        {
            if (game.checkDraw()) return 0;
            Player win = game.checkForWin();
            if (win == asPlayer) return 10;
            if (win != Player.None) return -10;
            return 0;
        }
    }
}
