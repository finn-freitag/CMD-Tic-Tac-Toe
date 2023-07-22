using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class GameFlow : IGameFlow
    {
        public Game Game { get; private set; }

        public event EventHandler UpdateUI;

        public event EventHandler GameStarted;

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        public int SimulatedPlayerDelay = 0;

        public IPlayerEngine Player1;
        public IPlayerEngine Player2;

        public Player CurrentPlayer { get; private set; } = Player.None;

        public TimeSpan ElapsedTime => sw.Elapsed;

        Stopwatch sw;

        public GameFlow(IPlayerEngine player1, IPlayerEngine player2)
        {
            Player1 = player1;
            Player2 = player2;
            sw = new Stopwatch();
        }

        public void Start()
        {
            if (GameStarted != null) GameStarted(this, EventArgs.Empty);
            sw.Start();
            Game = new Game();
            bool firstPlayer = true;
            CurrentPlayer = Player.Player1;
            if (UpdateUI != null) UpdateUI(this, EventArgs.Empty);
            while (true)
            {
                Player winner = Game.checkForWin();
                if (winner != Player.None)
                {
                    if (GameFinished != null) GameFinished(this, new GameFinishedEventArgs(winner, false));
                    CurrentPlayer = Player.None;
                    sw.Stop();
                    return;
                }
                if (Game.checkDraw())
                {
                    if (GameFinished != null) GameFinished(this, new GameFinishedEventArgs(Player.None, true));
                    CurrentPlayer = Player.None;
                    sw.Stop();
                    return;
                }

                // Player move
                if (firstPlayer)
                {
                    Player1.Move(Game, Player.Player1);
                }
                else
                {
                    Player2.Move(Game, Player.Player2);
                }

                firstPlayer = !firstPlayer;
                if (firstPlayer) CurrentPlayer = Player.Player1; else CurrentPlayer = Player.Player2;

                if (UpdateUI != null) UpdateUI(this, EventArgs.Empty);

                Thread.Sleep(SimulatedPlayerDelay);
            }
        }
    }

    public class GameFinishedEventArgs : EventArgs
    {
        public Player Winner = Player.None;
        public bool isDraw = false;

        public GameFinishedEventArgs(Player Winner, bool isDraw)
        {
            this.Winner = Winner;
            this.isDraw = isDraw;
        }
    }
}
