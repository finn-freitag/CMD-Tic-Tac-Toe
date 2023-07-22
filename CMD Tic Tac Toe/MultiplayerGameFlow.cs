using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    internal class MultiplayerGameFlow : IGameFlow, IDisposable
    {
        public Game Game { get; private set; }
        
        public event EventHandler UpdateUI;

        public event EventHandler GameStarted;

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        public IPlayerEngine Player1;

        public Player CurrentPlayer { get; private set; } = Player.None;

        public TimeSpan ElapsedTime => sw.Elapsed;

        private Stopwatch sw;

        private string host = "";
        private int port = 80;

        private string playerGUID = "";

        private string gameGUID = "";

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                string res = TcpExtensions.MakeRequest(host, port, playerGUID + ":setusername:" + value);
                if (res == "error") throw new Exception("Server: Player not found!");
            }
        }

        string username = "Unknown";

        public MultiplayerGameFlow(IPlayerEngine player, string host, int port = 80)
        {
            sw = new Stopwatch();
            Player1 = player;
            this.host = host;
            this.port = port;
            Game = new Game();
            playerGUID = Guid.NewGuid().ToString();
        }

        public string CreatePrivateGame()
        {
            string res = TcpExtensions.MakeRequest(host, port, playerGUID + ":join:private:random");
            if (res == "error")
            {
                throw new GameJoinException(false);
            }
            else
            {
                gameGUID = res;
                return res;
            }
        }

        public void JoinPrivateGame(string guid)
        {
            string res = TcpExtensions.MakeRequest(host, port, playerGUID + ":join:private:" + guid);
            if (res == "error") throw new GameJoinException(false, guid);
            if (res == "occupied") throw new GameJoinException(true, guid);
            gameGUID = res;
        }

        public void JoinOrCreatePublicGame()
        {
            gameGUID = TcpExtensions.MakeRequest(host, port, playerGUID + ":join:public:random");
        }

        public string GetOpponentName()
        {
            return TcpExtensions.MakeRequest(host, port, playerGUID + ":opponentname:" + gameGUID);
        }

        public void Dispose()
        {
            TcpExtensions.MakeRequest(host, port, playerGUID + ":dispose:" + gameGUID);
        }

        public void Start()
        {
            if (gameGUID == "") JoinOrCreatePublicGame();
            if (GameStarted != null) GameStarted(this, EventArgs.Empty);
            if (UpdateUI != null) UpdateUI(this, EventArgs.Empty);
            sw.Start();

            bool first = true;

            while (true)
            {
                // receive 'ismyturn' function result
                string res = TcpExtensions.MakeRequest(host, port, playerGUID + ":ismyturn:" + gameGUID);
                if (res == "error") throw new Exception();
                string[] parts = res.Split(':');
                CurrentPlayer = parts[0] == "true" ? Player.Player2 : Player.Player1;
                (int, int, Player) lm = Game.LastMove;
                Game = new Game(parts[1]);

                // check for win
                Player win = Game.checkForWin();
                bool draw = Game.checkDraw();
                if (draw || win != Player.None)
                {
                    sw.Stop();
                    if (GameFinished != null)
                    {
                        GameFinished(this, new GameFinishedEventArgs(win, draw));
                    }
                    return;
                }

                // do GUI stuff
                if ((lm != Game.LastMove || first) && UpdateUI != null) UpdateUI(this, EventArgs.Empty);
                
                // execute move
                if (parts[0] == "true")
                {
                    Player1.Move(Game, Player.Player2);
                    string r = TcpExtensions.MakeRequest(host, port, playerGUID + ":move:" + gameGUID + ":" + Convert.ToString(Game.LastMove.x) + ":" + Convert.ToString(Game.LastMove.y));
                    if (!r.StartsWith("confirmed")) throw new Exception();
                    CurrentPlayer = Player.Player1;
                    if (UpdateUI != null) UpdateUI(this, EventArgs.Empty);
                }

                // check for win
                win = Game.checkForWin();
                draw = Game.checkDraw();
                if (draw || win != Player.None)
                {
                    sw.Stop();
                    if (GameFinished != null)
                    {
                        GameFinished(this, new GameFinishedEventArgs(win, draw));
                    }
                    return;
                }
                
                first = false;
                Thread.Sleep(1000);
            }
        }
    }

    public class GameJoinException : Exception
    {
        public string gameGUID = "";
        public bool Occupied = false;

        public GameJoinException(bool occupied, string gameGUID = "")
        {
            Occupied = occupied;
            this.gameGUID = gameGUID;
        }

        public override string Message
        {
            get
            {
                if (Occupied)
                {
                    return "This game-GUID is already used! Another one was faster!";
                }
                else
                {
                    return "An error has occurred! Maybe the game with this GUID doesn't exist!";
                }
            }
        }

        public override IDictionary Data
        {
            get
            {
                IDictionary data = base.Data;
                if (Occupied)
                {
                    data.Add("Occupied", true);
                }
                if (gameGUID != "")
                {
                    data.Add("game-GUID", gameGUID);
                }
                return data;
            }
        }
    }
}
