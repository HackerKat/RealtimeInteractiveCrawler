using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using NetworkLib;
using System.Threading;

namespace Server
{
    public class NetworkManager
    {
        private List<Thread> clients;
        //private bool threadShouldEnd = false;
        private TcpListener server;
        private Dictionary<TcpClient, int> connections;
        private Dictionary <Player, int> players = new Dictionary<Player, int>();
        private int connectionId;
        private Random rand = new Random();
        private int seed;
        
        public NetworkManager()
        {
            seed = rand.Next(1, int.MaxValue);
        }

        public void SendData(Packet packet, NetworkStream stream)
        {
            stream.WriteByte((byte)packet.PacketType); //write id of the packet
            stream.Write(BitConverter.GetBytes(packet.Size), 0, 4); //write size of the packet
            stream.Write(packet.Data, 0, packet.Data.Length); //write data
        }

        public Packet ReadData(NetworkStream stream)
        {
            PacketType packetType = (PacketType)stream.ReadByte();
            byte[] sizeBytes = new byte[4];
            stream.Read(sizeBytes, 0, sizeBytes.Length);
            int size = BitConverter.ToInt32(sizeBytes, 0);
            byte[] data = new byte[size];
            stream.Read(data, 0, size);

            return new Packet(packetType, size, data);
        }

        public void StopNetwork()
        {
            //threadShouldEnd = true;
            
            foreach(TcpClient client in connections.Keys)
            {
                client.Close();
            }
            foreach(Thread t in clients)
            {
                t.Join(); //wait until this thread ends itself
            }
            server.Stop();
        }

        public void StartServer()
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, 12534);
                connections = new Dictionary<TcpClient, int>();
                clients = new List<Thread>();
                server.Start();
                Console.WriteLine("Server started");
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to connect...");
            }
        }

        public void Accept()
        {
            TcpClient client = server.AcceptTcpClient();
            
            Console.WriteLine("Connection established");
            int newConnId = connectionId;
            connectionId++;
            connections.Add(client, newConnId); //wird connectionId geadded und danach incrementiert
            SendAcceptPacket(client);

            foreach(TcpClient c in connections.Keys)
            {
                if(client != c)
                {
                    SendNewPlayerJoined(c, newConnId);
                }
            }
            Thread newThread = new Thread(() => ClientThreadLoop(client));
            clients.Add(newThread);
            newThread.Start();
        }

        public void ClientThreadLoop(TcpClient client)
        {
            while (client.Connected)
            {
                Packet p = ReadData(client.GetStream());

                switch (p.PacketType)
                {
                    case PacketType.PING:
                        Console.WriteLine("ping packet received");
                        PacketBuilder pb = new PacketBuilder(0);
                        pb.Add(14);
                        Packet packet = pb.Build(); //ping packet
                        SendData(packet, client.GetStream());
                        Console.WriteLine("Send pong");
                        break;
                    case PacketType.UPDATE_POS:
                        SendPlayerUpdate(client, p);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SendNewPlayerJoined(TcpClient client, int newClientId)
        {
            PacketBuilder pb = new PacketBuilder(PacketType.NEW_PLAYER);
            foreach(Player p in players.Keys)
            {
                if(p.ConnId == newClientId)
                {
                    pb.Add(p.ConnId);
                    pb.Add(p.PosX); 
                    pb.Add(p.PosY);
                }
            }
            Packet packet = pb.Build();
            SendData(packet, client.GetStream());
        }

        public void SendAcceptPacket(TcpClient client)
        {
            PacketBuilder pb = new PacketBuilder(PacketType.INIT);
            int clientId = connections[client];
            pb.Add(clientId); //connection id
            pb.Add(seed); //seed
            int x = rand.Next(50, 500);
            int y = rand.Next(50, 500);
            pb.Add(x); //spawn posX
            pb.Add(y); //spawn posY
            Player pl = new Player(x, y, clientId);
            players.Add(pl, clientId);
            foreach (int id in players.Values)
            {
                if(id != clientId)
                {
                    SendNewPlayerJoined(client, id);
                }
            }
            Packet packet = pb.Build();
            SendData(packet, client.GetStream());
        }

        public void SendPlayerUpdate(TcpClient client, Packet p)
        {
            PacketReader pr = new PacketReader(p);
            int spriteId = pr.GetInt();
            int posX = pr.GetInt();
            int posY = pr.GetInt();
            int conId = pr.GetInt();
            
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_POS);
            pb.Add(spriteId);
            pb.Add(posX);
            pb.Add(posY);
            pb.Add(conId);
            Packet packet = pb.Build();
            SendData(packet, client.GetStream());
        }
       
    }
}
