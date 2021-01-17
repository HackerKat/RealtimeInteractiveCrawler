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
        static void Main(string[] args)
        {
            Random rand = new Random();
            int seed = rand.Next(1, int.MaxValue);
            World world = new World();
            NetworkManager networkManager = new NetworkManager(seed, world);
            GameLoop gameLoop = new GameLoop(rand, seed, world, networkManager);

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
