using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class MainClass
    {
        public static Random Rand;

        public static void Main(string[] args)
        {
            Rand = new Random();

            AwesomeGame game = new AwesomeGame();
            game.Run();
        }
    }
}
