using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            String ipAdress = Console.ReadLine();
            AwesomeGame game = new AwesomeGame(ipAdress);
            game.Run();
        }
    }
}
