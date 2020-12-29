using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Server
{
    class Server
    {
        private static NetworkManager networkManager;
        static void Main(string[] args)
        {
            networkManager.StartServer();
        }
        
    }
}
