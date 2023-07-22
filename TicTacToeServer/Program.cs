using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CMD_Tic_Tac_Toe;

namespace TicTacToeServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new MultiplayerServer();
        }
    }
}
