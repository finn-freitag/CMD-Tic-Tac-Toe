using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class Game : ICloneable
    {
        public static readonly Random Random = new Random();

        public Player[] gamearea = new Player[9];

        public (int x, int y, Player player) LastMove = (-1, -1, Player.None);

        public Player CurrentPlayer
        {
            get
            {
                if (LastMove.player == Player.Player1) return Player.Player2;
                return Player.Player1;
            }
        }

        public Game()
        {
            for(int i = 0; i < gamearea.Length; i++)
            {
                gamearea[i] = Player.None;
            }
        }

        public Game(string str)
        {
            string[] parts = str.Split(',');
            for(int i = 0; i < parts[0].Length; i++)
            {
                gamearea[i] = ToPlayer(parts[0][i]);
            }
            LastMove = (Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), ToPlayer(parts[3][0]));
        }

        public override string ToString()
        {
            string str = "";
            for(int i = 0; i < gamearea.Length; i++)
            {
                str += ToString(gamearea[i]);
            }
            str += ",";
            str += Convert.ToString(LastMove.x);
            str += ",";
            str += Convert.ToString(LastMove.y);
            str += ",";
            str += ToString(LastMove.player);
            return str;
        }

        public Player getState(int x, int y)
        {
            return gamearea[x + y * 3];
        }

        public void setState(int x, int y, Player type)
        {
            gamearea[x + y * 3] = type;
            LastMove = (x, y, type);
        }

        public bool isFree(int x, int y)
        {
            return gamearea[x + y * 3] == Player.None;
        }

        public bool isAllFree()
        {
            bool res = true;
            for(int i = 0; i < gamearea.Length; i++)
            {
                if (gamearea[i] != Player.None) res = false;
            }
            return res;
        }

        public Player checkForWin()
        {
            for(int i = 0; i < 3; i++)
            {
                if (getState(0, i) == getState(1, i) && getState(1, i) == getState(2, i)) return getState(2, i);
                if (getState(i, 0) == getState(i, 1) && getState(i, 1) == getState(i, 2)) return getState(i, 2);
            }
            if (getState(0, 0) == getState(1, 1) && getState(1, 1) == getState(2, 2)) return getState(1, 1);
            if (getState(2, 0) == getState(1, 1) && getState(1, 1) == getState(0, 2)) return getState(1, 1);
            return Player.None;
        }

        public bool checkDraw()
        {
            //if (checkForWin() != Player.None) return false;
            bool emptySpot = false;
            for(int i = 0; i < 9; i++)
            {
                if (gamearea[i] == Player.None) emptySpot = true;
            }
            return !emptySpot;
        }

        public bool Undo()
        {
            if (LastMove.player != Player.None)
            {
                gamearea[LastMove.x + LastMove.y * 3] = Player.None;
                LastMove = (LastMove.x, LastMove.y, Player.None);
                return true;
            }
            else return false;
        }

        public (Player winningPlayer, int x, int y)[] getWinningPlayer()
        {
            List<(Player, int, int)> res = new List<(Player, int, int)>();
            for (int i = 0; i < 3; i++)
            {
                var match = isMatch(getState(0, i), getState(1, i), getState(2, i));
                if (match.matchingPlayer != Player.None) res.Add((match.matchingPlayer, match.index, i));
                var match2 = isMatch(getState(i, 0), getState(i, 1), getState(i, 2));
                if (match2.matchingPlayer != Player.None) res.Add((match2.matchingPlayer, i, match2.index));
            }
            var match3 = isMatch(getState(0, 0), getState(1, 1), getState(2, 2));
            if (match3.matchingPlayer != Player.None) res.Add((match3.matchingPlayer, match3.index, match3.index));
            var match4 = isMatch(getState(2, 0), getState(1, 1), getState(0, 2));
            if (match4.matchingPlayer != Player.None) res.Add((match4.matchingPlayer, 2 - match4.index, match4.index));
            return res.ToArray();
        }

        private (Player matchingPlayer, int index) isMatch(Player p1, Player p2, Player p3)
        {
            if (p1 == p2 && p1 != Player.None && p3 == Player.None) return (p1, 2);
            if (p1 == p3 && p1 != Player.None && p2 == Player.None) return (p1, 1);
            if (p3 == p2 && p3 != Player.None && p1 == Player.None) return (p3, 0);
            return (Player.None, -1);
        }

        public object Clone()
        {
            Game g = new Game();
            for(int i = 0; i < gamearea.Length; i++)
            {
                g.gamearea[i] = gamearea[i];
            }
            return g;
        }

        public static string ToString(Player type)
        {
            if (type == Player.Player1) return "O";
            if (type == Player.Player2) return "X";
            return "#";
        }

        public static Player ToPlayer(char type)
        {
            if (type == 'O') return Player.Player1;
            if (type == 'X') return Player.Player2;
            return Player.None;
        }

        public static (int x, int y) getFreeRandomPos(Game game)
        {
            int x = Random.Next(0, 3);
            int y = Random.Next(0, 3);
            while(game.getState(x, y) != Player.None)
            {
                x = Random.Next(0, 3);
                y = Random.Next(0, 3);
            }
            return (x, y);
        }
    }

    public enum Player : byte
    {
        None = 0,
        Player1 = 1,
        Player2 = 2
    }

    public enum GameState : byte
    {
        None = 0,
        Draw = 1,
        Player1Win = 2,
        Player2Win = 3
    }
}
