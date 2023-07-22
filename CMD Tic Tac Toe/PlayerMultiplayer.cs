using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    [Obsolete("This player is not able to receive the last move of the opponent if the opponent wins, because it's based on moves and this player haven't got a move after a win. Your game won't work properly using this player! Use 'MultiplayerGameFlow' instead!")]
    public class PlayerMultiplayer : IPlayerEngine, IDisposable
    {
        public string PlayerGUID { get; private set; }
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                string res = TcpExtensions.MakeRequest(host, port, PlayerGUID + ":setusername:" + value);
                if(res == "error")throw new Exception("Server: Player not found!");
            }
        }

        string username = "Unknown";

        public string GameGUID { get; private set; } = "";

        string host = "";
        int port = 80;

        public PlayerMultiplayer(bool Private, string host, int port = 80)
        {
            this.host = host;
            this.port = port;
            PlayerGUID = Guid.NewGuid().ToString();
            string join = TcpExtensions.MakeRequest(host, port, PlayerGUID + ":join:" + (Private ? "private" : "public") + ":random");
            if (join == "error" || join == "occupied")
            {
                throw new GameJoinException(false);
            }
            else
            {
                GameGUID = join;
            }
        }

        public PlayerMultiplayer(string gameGUID, string host, int port = 80)
        {
            this.host = host;
            this.port = port;
            PlayerGUID = Guid.NewGuid().ToString();
            string join = TcpExtensions.MakeRequest(host, port, PlayerGUID + ":join:private:" + gameGUID);
            if (join == "error")
            {
                throw new GameJoinException(false, gameGUID);
            }
            if (join == "occupied")
            {
                throw new GameJoinException(true, gameGUID);
            }
            GameGUID = join;
        }

        public void Move(Game game, Player asPlayer)
        {
            if (game.LastMove.player != Player.None) TcpExtensions.MakeRequest(host, port, PlayerGUID + ":move:" + GameGUID + ":" + Convert.ToString(game.LastMove.x) + ":" + Convert.ToString(game.LastMove.y));
            bool first = true;
            while (true)
            {
                if (!first)
                {
                    Thread.Sleep(1500);
                    first = false;
                }
                string res = TcpExtensions.MakeRequest(host, port, PlayerGUID + ":ismyturn:" + GameGUID);
                string[] p = res.Split(':');

                if (p[0] == "true")
                {
                    (int, int, Player) lm = game.LastMove;
                    string[] parts = p[1].Split(',');
                    for (int i = 0; i < parts[0].Length; i++)
                    {
                        game.gamearea[i] = Game.ToPlayer(parts[0][i]);
                    }
                    game.LastMove = (Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), Game.ToPlayer(parts[3][0]));
                    if (game.isAllFree() && lm.Item3 != Player.None)
                    {
                        game.setState(lm.Item1, lm.Item2, lm.Item3);
                    }
                    return;
                }
            }
        }

        public string GetOpponentName()
        {
            return TcpExtensions.MakeRequest(host, port, PlayerGUID + ":opponentname:" + GameGUID);
        }

        public void Dispose()
        {
            TcpExtensions.MakeRequest(host, port, PlayerGUID + ":dispose:" + GameGUID);
        }
    }
}
