using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class MultiplayerServer
    {
        Dictionary<string, (Game game, string Player1, string Player2, bool ready)> games = new Dictionary<string, (Game, string, string, bool)>();

        Queue<string> pendingGames = new Queue<string>();

        public Dictionary<string, OnlinePlayer> players = new Dictionary<string, OnlinePlayer>();

        public IPlayerEngine ReplacementPlayer = new PlayerMiniMax();

        public MultiplayerServer(int port = 80)
        {
            TcpExtensions.HostServer(port, Answer);
        }

        public string Answer(string request)
        {
            try
            {
                string[] parts = request.Split(':');
                string clientGUID = parts[0];
                string command = parts[1];
                if (command == "join") // ["private"|"public"],[gameGUID|"random"]
                {
                    string gameGUID = "error";
                    if (parts[2] == "private")
                    {
                        if (parts[3] == "random")
                        {
                            gameGUID = Guid.NewGuid().ToString();
                            OnlinePlayer op = new OnlinePlayer(clientGUID);
                            players.Add(op.GUID, op);
                            games.Add(gameGUID, (new Game(), op.GUID, null, false));
                        }
                        else
                        {
                            if (games.ContainsKey(parts[3]))
                            {
                                if (games[parts[3]].Player2 == null)
                                {
                                    OnlinePlayer op = new OnlinePlayer(clientGUID);
                                    players.Add(op.GUID, op);
                                    games[parts[3]] = (games[parts[3]].game, games[parts[3]].Player1, op.GUID, true);
                                    gameGUID = parts[3];
                                }
                                else
                                {
                                    gameGUID = "occupied";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (pendingGames.Count > 0)
                        {
                            gameGUID = pendingGames.Dequeue();
                            OnlinePlayer op = new OnlinePlayer(clientGUID);
                            players.Add(op.GUID, op);
                            games[gameGUID] = (games[gameGUID].game, games[gameGUID].Player1, op.GUID, true);
                        }
                        else
                        {
                            gameGUID = Guid.NewGuid().ToString();
                            OnlinePlayer op = new OnlinePlayer(clientGUID);
                            players.Add(op.GUID, op);
                            games.Add(gameGUID, (new Game(), op.GUID, null, false));
                            pendingGames.Enqueue(gameGUID);
                        }
                    }
                    return gameGUID;
                }
                if (command == "setusername") // username
                {
                    if (players.ContainsKey(clientGUID))
                    {
                        players[clientGUID].Username = parts[2];
                        return "confirmed";
                    }
                }
                if (command == "move") // gameGUID,x,y
                {
                    string gameGUID = parts[2];
                    if (games.ContainsKey(gameGUID))
                    {
                        Game game = games[gameGUID].game;
                        OnlinePlayer pl1 = players[games[gameGUID].Player1];
                        OnlinePlayer pl2 = players[games[gameGUID].Player2];
                        if ((game.CurrentPlayer == Player.Player1 && pl1.GUID == clientGUID)
                            || (game.CurrentPlayer == Player.Player2 && pl2.GUID == clientGUID))
                        {
                            int x = Convert.ToInt32(parts[3]);
                            int y = Convert.ToInt32(parts[4]);
                            if (game.isFree(x, y))
                            {
                                game.setState(x, y, clientGUID == pl1.GUID ? Player.Player1 : Player.Player2);
                                return "confirmed:" + (clientGUID == pl1.GUID ? invertPlayers(game.ToString()) : game.ToString());
                            }
                            else
                            {
                                return "alreadyused";
                            }
                        }
                        else
                        {
                            return "notyourturn";
                        }
                    }
                }
                if (command == "opponentname") // gameGUID
                {
                    if (games.ContainsKey(parts[2]))
                    {
                        if (clientGUID == games[parts[2]].Player1) return players[games[parts[2]].Player2].Username;
                        if (clientGUID == games[parts[2]].Player2) return players[games[parts[2]].Player1].Username;
                    }
                }
                if (command == "ismyturn") // gameGUID
                {
                    if (games.ContainsKey(parts[2]))
                    {
                        if (games[parts[2]].Player1 == null && games[parts[2]].ready)
                        {
                            ReplacementPlayer.Move(games[parts[2]].game, Player.Player1);
                        }
                        if (games[parts[2]].Player2 == null && games[parts[2]].ready)
                        {
                            ReplacementPlayer.Move(games[parts[2]].game, Player.Player2);
                        }
                        Player currentPl = games[parts[2]].game.CurrentPlayer;
                        bool res = (currentPl == Player.Player1 && clientGUID == games[parts[2]].Player1)
                            || (currentPl == Player.Player2 && clientGUID == games[parts[2]].Player2);
                        return (res ? "true" : "false") + ":" + (clientGUID == games[parts[2]].Player1 ? invertPlayers(games[parts[2]].game.ToString()) : games[parts[2]].game.ToString());
                    }
                }
                if (command == "dispose") // gameGUID
                {
                    if (games.ContainsKey(parts[2]))
                    {
                        if (games[parts[2]].Player1 == clientGUID) games[parts[2]] = (games[parts[2]].game, null, games[parts[2]].Player2, true);
                        if (games[parts[2]].Player2 == clientGUID) games[parts[2]] = (games[parts[2]].game, games[parts[2]].Player2, null, true);
                    }
                    if (players.ContainsKey(clientGUID)) players.Remove(clientGUID);
                    return "disposed";
                }
            }
            catch { }
            return "error";
        }

        private string invertPlayers(string str)
        {
            return str.Replace('X', 'I').Replace('O', 'X').Replace('I', 'O');
        }
    }

    public class OnlinePlayer
    {
        public string GUID = "";
        public string Username = "Unknown";

        public OnlinePlayer(string guid)
        {
            GUID = guid;
        }
    }
}
