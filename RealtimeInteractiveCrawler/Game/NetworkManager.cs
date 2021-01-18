using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using NetworkLib;

namespace RealtimeInteractiveCrawler
{
    public class NetworkManager
    {

        public NetworkStream Stream;
        private Thread networkThread;
        private bool threadShouldEnd = false;
        public TcpClient Client;
        public MessageQueue MessageQueue
        {
            get;
            private set;
        } = new MessageQueue();
        public NetworkManager()
        {

        }

        public void SendData(Packet packet)
        {
            //write id of the packet
            Stream.WriteByte((byte)packet.PacketType);
            //write size of the packet
            Stream.Write(BitConverter.GetBytes(packet.Size), 0, 4);
            //write data
            Stream.Write(packet.Data, 0, packet.Data.Length);
            Stream.Flush();
        }

        public Packet ReadData()
        {
            PacketType packetType = (PacketType)Stream.ReadByte();
            byte[] sizeBytes = new byte[4];
            Stream.Read(sizeBytes, 0, sizeBytes.Length);
            int size = BitConverter.ToInt32(sizeBytes, 0);
            byte[] data = new byte[size];
            Stream.Read(data, 0, size);

            //Console.WriteLine("PacketType: " + packetType.ToString());
            //Console.WriteLine("Packet size: " + size);

            return new Packet(packetType, size, data);
        }

        public void StopNetwork()
        {
            threadShouldEnd = true;
            networkThread.Join();
            Stream.Close();
            Client.Close();
        }

        public void Connect(string server)
        {
            try
            {
                int port = 80;
                Client = new TcpClient(server, port);
                Stream = Client.GetStream();
                threadShouldEnd = false;
                networkThread = new Thread(ReadLoop);
                networkThread.Start();
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

        public void ReadLoop()
        {
            while (!threadShouldEnd)
            {
                Packet p = ReadData();
                MessageQueue.Push(p);
            }
        }

        public void SendItemUpdate(int id)
        {
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_ITEM);
            pb.Add(id);
            Packet packet = pb.Build();
            SendData(packet);
            //Console.WriteLine("PAcket sent with item: " + id);
        }

        public void SendEnemyHealth(int id, int health)
        {
            Console.WriteLine("Packet sent with enemy id: " + id + " and health " + health);
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_ENEMY_HEALTH);
            pb.Add(id);
            pb.Add(health);
            Packet packet = pb.Build();
            SendData(packet);
        }

        public void SendChangeTile(int x, int y)
        {
            PacketBuilder pb = new PacketBuilder(PacketType.TILE_UPDATED);
            pb.Add(x);
            pb.Add(y);
            Packet packet = pb.Build();
            SendData(packet);
        }

        public void SendMyPlayerHealth(int health)
        {
            //Console.WriteLine("Packet sent with enemy id: " + id + " and health " + health);
            PacketBuilder pb = new PacketBuilder(PacketType.UPDATE_PLAYER_HEALTH);
            pb.Add(health);
            Packet packet = pb.Build();
            SendData(packet);
        }
    }
}
