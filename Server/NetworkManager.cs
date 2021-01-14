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
        private int connectionId;
        
        public NetworkManager()
        {

        }

        public void SendData(Packet packet, NetworkStream stream)
        {
            //write id of the packet
            stream.WriteByte(packet.Id);
            //write size of the packet
            stream.Write(BitConverter.GetBytes(packet.Size), 0, 4);
            //write data
            stream.Write(packet.Data, 0, packet.Data.Length);
        }

        public Packet ReadData(NetworkStream stream)
        {
            byte id = (byte)stream.ReadByte();
            byte[] sizeBytes = new byte[4];
            stream.Read(sizeBytes, 0, sizeBytes.Length);
            int size = BitConverter.ToInt32(sizeBytes, 0);
            byte[] data = new byte[size];
            stream.Read(data, 0, size);

            return new Packet(id, size, data);
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
            
            Console.WriteLine("Connection establisehd");
            connections.Add(client, connectionId++); //wird connectionId geadded und danach incrementiert
            SendAcceptPacket(client);
            Thread newThread = new Thread(() => ClientThreadLoop(client));
            clients.Add(newThread);
            newThread.Start();
        }

        public void ClientThreadLoop(TcpClient client)
        {
            while (client.Connected)
            {
                Packet p = ReadData(client.GetStream());

                switch (p.Id)
                {
                    case 0:
                        Console.WriteLine("ping packet received");
                        PacketBuilder pb = new PacketBuilder(0);
                        pb.Add(14);
                        Packet packet = pb.Build(); //ping packet
                        SendData(packet, client.GetStream());
                        Console.WriteLine("Send pong");
                        break;
                    case 1:
                        break;
                    case 2:
                        //SendInitData(client);
                        break;
                    case 3:
                        SendPlayerUpdate(client, p);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SendAcceptPacket(TcpClient client)
        {
            PacketBuilder pb = new PacketBuilder(1);
            int clientId = connections[client];
            pb.Add(clientId);
            Packet packet = pb.Build();
            SendData(packet, client.GetStream());
            SendInitData(client);
        }

        public void SendInitData(TcpClient client)
        {
            PacketBuilder pb = new PacketBuilder(2);
            pb.Add(66); //seed
            pb.Add(300); //spawn posX
            pb.Add(150); //spawn posY
            foreach(TcpClient c in connections.Keys)
            {
                if(c != client)
                {
                    //other player pos
                    //entity list
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
            
            PacketBuilder pb = new PacketBuilder(3);
            pb.Add(spriteId);
            pb.Add(posX);
            pb.Add(posY);
            pb.Add(conId);
            Packet packet = pb.Build();
            SendData(packet, client.GetStream());
        }
       
    }
}
