using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace Server
{
    public class Server
    {
        public static World world;
        public static Random rand;
        static void Main(string[] args)
        {
            rand = new Random();
            int seed = rand.Next(1, int.MaxValue);
            world = new World();
            NetworkManager networkManager = new NetworkManager(seed);
            GameLoop gameLoop = new GameLoop(seed, networkManager);

            Thread t = new Thread(gameLoop.Run);
            t.Start();

            networkManager.StartServer();
            while (true)
            {
                networkManager.Accept();
            }
            t.Join();
        }
    }
}
