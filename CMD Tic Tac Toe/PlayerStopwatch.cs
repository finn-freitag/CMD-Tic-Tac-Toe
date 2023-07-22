using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class PlayerStopwatch : IPlayerEngine
    {
        public IPlayerEngine Player = null;

        public TimeSpan LastMoveDuration = TimeSpan.Zero;

        public TimeSpan EntireMoveDuration = TimeSpan.Zero;

        public PlayerStopwatch(IPlayerEngine player)
        {
            Player = player;
        }

        public void Move(Game game, Player asPlayer)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Player.Move(game, asPlayer);
            sw.Stop();
            LastMoveDuration = sw.Elapsed;
            EntireMoveDuration += sw.Elapsed;
        }
    }
}
