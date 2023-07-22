using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class PlayerAITrainer : IPlayerEngine
    {
        public IPlayerEngine Trainer;
        public PlayerAI AI;

        public PlayerAITrainer(IPlayerEngine trainer)
        {
            Trainer = trainer;
            AI = new PlayerAI();
        }

        public void Move(Game game, Player asPlayer)
        {
            Game copy = (Game)game.Clone();
            Trainer.Move(game, asPlayer);
            (int x, int y, Player player) Move = game.LastMove;
            AI.Train(copy, asPlayer, Move.x, Move.y);
        }
    }
}
