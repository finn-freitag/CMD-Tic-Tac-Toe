using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public class PlayerCMD : IPlayerEngine
    {
        public PlayerCMD()
        {

        }

        public void Move(Game game, Player asPlayer)
        {
            while (true)
            {
                string input = Console.ReadLine();
                try
                {
                    string[] inp = input.Split(new char[] { '|', ' ', ';', ':', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                    int x = Convert.ToInt32(inp[0]) - 1;
                    int y = Convert.ToInt32(inp[1]) - 1;
                    if (game.isFree(x, y))
                    {
                        game.setState(x, y, asPlayer);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("This field is already used!");
                    }
                }
                catch
                {
                    Console.WriteLine("Wrong input!");
                }
            }
        }
    }
}
