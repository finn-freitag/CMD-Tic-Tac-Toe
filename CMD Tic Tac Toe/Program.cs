using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class Program
    {
        static void MiniMax_Main(string[] args)
        {
            IPlayerEngine player1 = new PlayerCMD();
            IPlayerEngine player2 = new PlayerMiniMax();
            int delay = 1000;

            GameFlow gameflow = new GameFlow(player1, player2);
            gameflow.UpdateUI += Gameflow_UpdateUI;
            gameflow.GameFinished += Gameflow_GameFinished;
            gameflow.SimulatedPlayerDelay = delay;
            while (true)
            {
                gameflow.Start();
                Console.WriteLine("Press 'x' if you don't want to play again:");
                if (Console.ReadLine() == "x") break;
            }
        }

        static void /*Mixer_*/Main(string[] args)
        {
            IPlayerEngine player1 = new PlayerCMD();
            PlayerMixer player2 = new PlayerMixer();
            player2.AddPlayerType(new PlayerMiniMax(), 50);
            player2.AddPlayerType(new PlayerRandom(), 50);
            int delay = 1000;

            GameFlow gameflow = new GameFlow(player1, player2);
            gameflow.UpdateUI += Gameflow_UpdateUI;
            gameflow.GameFinished += Gameflow_GameFinished;
            gameflow.SimulatedPlayerDelay = delay;
            while (true)
            {
                gameflow.Start();
                Console.WriteLine("Press 'x' if you don't want to play again:");
                if (Console.ReadLine() == "x") break;
            }
        }

        static void AI_Main(string[] args)
        {
            IPlayerEngine player1 = new PlayerCMD();
            IPlayerEngine player2 = new PlayerAI();
            int delay = 1000;

            if (File.Exists("ai.ttt")) ((PlayerAI)player2).LoadFromString(File.ReadAllText("ai.ttt"));

            GameFlow gameflow = new GameFlow(player1, player2);
            gameflow.UpdateUI += Gameflow_UpdateUI;
            gameflow.GameFinished += Gameflow_GameFinished;
            gameflow.SimulatedPlayerDelay = delay;
            while (true)
            {
                gameflow.Start();
                Console.WriteLine("Press 'x' if you don't want to play again:");
                if (Console.ReadLine() == "x") break;
            }
        }

        static void Multiplayer_Main(string[] args)
        {
            IPlayerEngine player = new PlayerCMD();

            MultiplayerGameFlow gameflow = new MultiplayerGameFlow(player, "127.0.0.1");
            gameflow.UpdateUI += Gameflow_UpdateUI;
            gameflow.GameFinished += Gameflow_GameFinished;
            gameflow.Start();
        }

        static void Broken_Multiplayer_Main(string[] args)
        {
            IPlayerEngine player2 = new PlayerMultiplayer(false, "127.0.0.1", 80);
            IPlayerEngine player1 = new PlayerCMD();

            GameFlow gameflow = new GameFlow(player1, player2);
            gameflow.UpdateUI += Gameflow_UpdateUI;
            gameflow.GameFinished += Gameflow_GameFinished;
            gameflow.SimulatedPlayerDelay = 0;
            gameflow.Start();
        }

        static void AI_Train_Main(string[] args)
        {
            IPlayerEngine player1 = new PlayerAITrainer(new PlayerMiniMax());
            IPlayerEngine player2 = new PlayerMiniMax(); //((PlayerAITrainer)player1).AI;
            int delay = 0;

            if (File.Exists("ai.ttt")) ((PlayerAITrainer)player1).AI.LoadFromString(File.ReadAllText("ai.ttt"));

            GameFlow gameflow = new GameFlow(player1, player2);
            //gameflow.UpdateUI += Gameflow_UpdateUI;
            //gameflow.GameFinished += Gameflow_GameFinished;
            gameflow.SimulatedPlayerDelay = delay;
            for(int i = 0; i < 500; i++)
            {
                gameflow.Start();
                Console.WriteLine(i);
            }
            File.WriteAllText("ai.ttt", ((PlayerAITrainer)player1).AI.ToString());
        }

        private static void Gameflow_GameFinished(object sender, GameFinishedEventArgs e)
        {
            Console.Clear();
            IGameFlow gameflow = (IGameFlow)sender;
            RenderGame(gameflow.Game);
            if (e.isDraw)
            {
                Console.WriteLine("Draw!");
            }
            else
            {
                Console.WriteLine(PlayerToString(e.Winner) + " wins!");
            }
            Console.ReadKey();
        }

        private static string PlayerToString(Player player)
        {
            if (player == Player.Player1) return "player 1";
            if (player == Player.Player2) return "player 2";
            return "none";
        }

        private static void Gameflow_UpdateUI(object sender, EventArgs e)
        {
            Console.Clear();
            IGameFlow gameflow = (IGameFlow)sender;
            RenderGame(gameflow.Game);
            Console.WriteLine("It's " + PlayerToString(gameflow.CurrentPlayer) + "'s turn!");
        }

        public static void RenderGame(Game game)
        {
            for(int y = 0; y < 3; y++)
            {
                for(int x = 0; x < 3; x++)
                {
                    Console.Write(" " + Game.ToString(game.getState(x, y)));
                }
                Console.WriteLine();
            }
        }
    }
}
