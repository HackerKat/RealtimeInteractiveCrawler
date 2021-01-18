using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using NetworkLib;
using System.Threading;
using System.Collections.Concurrent;
using System.Numerics;
using System.Diagnostics;

namespace Server
{
    public class NetworkManager
    {
        private List<Thread> clients;
        //private bool threadShouldEnd = false;
        private TcpListener server;
        private ConcurrentDictionary<TcpClient, int> connections = new ConcurrentDictionary<TcpClient, int>();
        public ConcurrentDictionary<int, Player> Players
        {
            get;
            private set;
        } = new ConcurrentDictionary<int, Player>();
        private int connectionId;
        private int seed;
        private ConcurrentBag<int> itemsToDestroy = new ConcurrentBag<int>();
        public NetworkManager(int seed)
        {
            this.seed = seed;
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
            foreach (TcpClient client in connections.Keys)
            {
                client.Close();
            }
            foreach (Thread t in clients)
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
            connections.TryAdd(client, newConnId); //wird connectionId geadded und danach incrementiert
            SendAcceptPacket(client);

            foreach (TcpClient c in connections.Keys)
            {
                if (client != c)
                {
                    SendNewPlayerJoined(c, newConnId);
                }
            }
            Thread newThread = new Thread(() => ClientThreadLoop(client));
            clients.Add(newThread);
            newThread.Name = "ClientThread: " + newConnId;
            Console.WriteLine("new Thread: " + newThread.Name);
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
                        //Console.WriteLine("ping packet received");
                        PacketBuilder pb = new PacketBuilder(0);
                        pb.Add(14);
                        Packet packet = pb.Build(); //ping packet
                        SendData(packet, client.GetStream());
                        //Console.WriteLine("Send pong");
                        break;
                    case PacketType.UPDATE_MY_POS:
                        SendPlayerUpdate(client, p);
                        break;
                    case PacketType.UPDATE_ITEM:
                        SendItemUpdate(client, p);
                        break;
                    case PacketType.UPDATE_ENEMY_HEALTH:
                        SendEnemyHealth(client, p);
                        break;
                    case PacketType.UPDATE_PLAYER_HEALTH:
                        UpdatePlayerHealth(client, p);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SendNewPlayerJoined(TcpClient client, int newClientId)
        {
            PacketBuilder pb = new PacketBuilder(PacketType.NEW_PLAYER);
            foreach (Player p in Players.Values)
            {
                if (p.ConnId == newClientId)
                {
                    pb.Add(p.ConnId);
                    pb.Add(p.Position.X);
                    pb.Add(p.Position.Y);
                    pb.Add(p.Health);
                }
            }
            Packet packet = pb.Build();
            lock (client)
            {
                SendData(packet, client.GetStream());
            }
        }

        public void SendAcceptPacket(TcpClient client)
        {
            Vector2 spawnPoint = Server.world.GetSpawnPoint(Server.rand);
            int clientId = connections[client];
            Player pl = new Player(spawnPoint.X, spawnPoint.Y, clientId);
            Players.TryAdd(clientId, pl);
            PacketBuilder pb = new PacketBuilder(PacketType.INIT);
            pb.Add(clientId); //connection id
            pb.Add(seed); //seed
            pb.Add(spawnPoint.X); //spawn posX
            pb.Add(spawnPoint.Y); //spawn posY
            pb.Add(pl.Health); //spawn posY
            pb.Add(Server.world.enemies.Count); //count of enemies
            foreach (Entity enemy in Server.world.enemies)
            {
                pb.Add(enemy.Id);
                pb.Add(enemy.Position.X); //position is float
                pb.Add(enemy.Position.Y);
                pb.Add(enemy.Health);
            }
            pb.Add(itemsToDestroy.Count);
            foreach (int item in itemsToDestroy)
            {
                pb.Add(item);
            }
           
            Packet packet = pb.Build();
            //Console.WriteLine("Init packet is built");
            lock (client)
            {
                SendData(packet, client.GetStream());
            }

            foreach (int id in Players.Keys)
            {
                if (id != clientId)
                {
                    SendNewPlayerJoined(client, id);
                }
            }
        }

        public void SendPlayerUpdate(TcpClient client, Packet p)
        {
            PacketReader pr = new PacketReader(p);
            int id = pr.GetInt();
            float x = pr.GetFloat();
            float y = pr.GetFloat();
            Player netplayer = Players[id];
            netplayer.Position = new Vector2(x, y);
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_OTHER_POS);
            pb.Add(id);
            pb.Add(netplayer.Position.X);
            pb.Add(netplayer.Position.Y);
            Packet packet = pb.Build();
            foreach (TcpClient c in connections.Keys)
            {
                if (c != client)
                {
                    lock (c)
                    {
                        SendData(packet, c.GetStream());
                    }
                }
            }
        }

        public void SendItemUpdate(TcpClient client, Packet p)
        {
            PacketReader pr = new PacketReader(p);
            int id = pr.GetInt();
            //Console.WriteLine("received update on item: " + id);
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_ITEM);
            pb.Add(id);
            Packet packet = pb.Build();
            itemsToDestroy.Add(id);
            //Console.WriteLine("sent update on item: " + id);
            foreach (TcpClient c in connections.Keys)
            {
                if (c != client)
                {
                    lock (c)
                    {
                        SendData(packet, c.GetStream());
                    }
                }
            }
        }

        public void SendEnemyUpdate()
        {
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_ENEMY);
            int enemieCount = Server.world.enemies.Count;
            pb.Add(Server.world.enemies.Count);
            foreach (Entity enemy in Server.world.enemies)
            {
                pb.Add(enemy.Id);
                pb.Add(enemy.Position.X);
                pb.Add(enemy.Position.Y);
                pb.Add(enemy.Health);
                pb.Add(enemy.chunk.chunkPos.X);
                pb.Add(enemy.chunk.chunkPos.Y);
                //Console.WriteLine("send enemy with: " + enemy.Id + " with health " + enemy.Health);
            }
            Packet packet = pb.Build();

            foreach (TcpClient client in connections.Keys)
            {
                lock (client)
                {
                    SendData(packet, client.GetStream());
                }
            }
        }

        public void SendEnemyHealth(TcpClient client, Packet packet)
        {
            PacketReader pr = new PacketReader(packet);
            int id = pr.GetInt();
            int health = pr.GetInt();
            //Console.WriteLine("received enemy with: " + id + " with health " + health);
            foreach (Entity enemy in Server.world.enemies)
            {
                if (enemy.Id == id)
                {
                    enemy.Health = health;
                    if(enemy.Health <= 0)
                    {
                        enemy.HasRecentlyDied = true;
                    }
                    break;
                }
            }
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_ENEMY_HEALTH);

            pb.Add(id);
            pb.Add(health);
            Packet p = pb.Build();
            //Console.WriteLine("send enemy update");
            foreach (TcpClient c in connections.Keys)
            {
                if (client != c)
                {
                    lock (c)
                    {
                        SendData(p, c.GetStream());
                    }
                }
            }
        }

        public void UpdatePlayerHealth(TcpClient client, Packet packet)
        {
            int id = connections[client];
            PacketReader pr = new PacketReader(packet);
            int health = pr.GetInt();
            Players[id].Health = health;

            foreach (TcpClient c in connections.Keys)
            {
                if(c != client)
                {
                    PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_PLAYER_HEALTH);
                    pb.Add(id);
                    pb.Add(health);
                    Packet p = pb.Build();
                    lock (c)
                    {
                        SendData(p, c.GetStream());
                    }
                }
            }
        }

        public void UpdatePlayerHealth(int playerId)
        {
            Player player = Players[playerId];
            foreach (TcpClient c in connections.Keys)
            {
                PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_PLAYER_HEALTH);
                pb.Add(playerId);
                pb.Add(player.Health);
                Packet p = pb.Build();
                //Console.WriteLine("player: " + playerId + " health " + player.Health);
                lock (c)
                {
                    SendData(p, c.GetStream());
                }
            }
        }

        public void SendDeadEnemy(int id)
        {
            PacketBuilder pb = new PacketBuilder(PacketType.ENEMY_DIED);
            pb.Add(id);
            Packet p = pb.Build();

            foreach (TcpClient c in connections.Keys)
            {
                lock (c)
                {
                    SendData(p, c.GetStream());
                }
            }
        }
    }
}
