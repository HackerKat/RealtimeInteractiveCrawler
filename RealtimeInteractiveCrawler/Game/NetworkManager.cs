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

        private NetworkStream stream;
        private Thread networkThread;
        private bool threadShouldEnd = false;
        private TcpClient client;
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
            stream.WriteByte((byte)packet.PacketType);
            //write size of the packet
            stream.Write(BitConverter.GetBytes(packet.Size), 0, 4);
            //write data
            stream.Write(packet.Data, 0, packet.Data.Length);
            stream.Flush();
        }

        public Packet ReadData()
        {
            PacketType packetType = (PacketType)stream.ReadByte();
            byte[] sizeBytes = new byte[4];
            stream.Read(sizeBytes, 0, sizeBytes.Length);
            int size = BitConverter.ToInt32(sizeBytes, 0);
            byte[] data = new byte[size];
            stream.Read(data, 0, size);

            Console.WriteLine("PacketType: " + packetType.ToString());
            Console.WriteLine("Packet size: " + size);

            return new Packet(packetType, size, data);
        }

        public void StopNetwork()
        {
            threadShouldEnd = true;
            networkThread.Join();
            stream.Close();
            client.Close();
        }

        public void Connect(string server)
        {
            try
            {
                int port = 12534;
                client = new TcpClient(server, port);
                stream = client.GetStream();
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
    }
}
