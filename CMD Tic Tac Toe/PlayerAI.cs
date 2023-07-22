using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    [Obsolete("This AI is very bad (because it uses single neurons instead of a network) and is probably more worse than 'PlayerRandom'.")]
    public class PlayerAI : IPlayerEngine
    {
        private Perzeptron[] aiPlayer = new Perzeptron[9];

        public PlayerAI()
        {
            Perzeptron.Random = Game.Random;
            for(int i = 0; i < aiPlayer.Length; i++)
            {
                aiPlayer[i] = new Perzeptron(9, 1, -1, 1);
            }
        }

        public void Move(Game game, Player asPlayer)
        {
            double[] d = new double[9];
            for(int i = 0; i < aiPlayer.Length; i++)
            {
                for(int j = 0; j < aiPlayer[i].input.Length - 1; j++)
                {
                    aiPlayer[i].input[j] = PlayerToValue(asPlayer, game.gamearea[j]);
                }
                d[i] = aiPlayer[i].calcSigmoid();
            }
            double max = d[0];
            int index = 0;
            for(int i = 1; i < d.Length; i++)
            {
                if (max < d[i] && game.gamearea[i] == Player.None)
                {
                    max = d[i];
                    index = i;
                }
            }
            game.gamearea[index] = asPlayer;
            game.LastMove = (index % 3, index / 3, asPlayer);
        }

        public void Train(Game game, Player asPlayer, int x, int y)
        {
            int index = x + y * 3;
            for (int i = 0; i < aiPlayer.Length; i++)
            {
                for (int j = 0; j < aiPlayer[i].input.Length - 1; j++)
                {
                    aiPlayer[i].input[j] = PlayerToValue(asPlayer, game.gamearea[j]);
                }
                aiPlayer[i].trainSigmoidWithCorrection(index == i ? 1 : 0);
            }
        }

        private double PlayerToValue(Player own, Player other)
        {
            if (other == Player.None) return -5;
            if (own == other) return 1; else return 0;
        }

        public override string ToString()
        {
            string res = "";
            for(int i = 0; i < aiPlayer.Length;i++)
            {
                res += "|" + aiPlayer[i].ToString();
            }
            res = res.Substring(1);
            return res;
        }

        public void LoadFromString(string weights)
        {
            string[] strs = weights.Split('|');
            for(int i = 0; i < aiPlayer.Length; i++)
            {
                aiPlayer[i].LoadWeightsFromString(strs[i]);
            }
        }
    }
}
