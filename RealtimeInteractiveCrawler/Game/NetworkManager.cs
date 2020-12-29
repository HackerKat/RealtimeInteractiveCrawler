using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace RealtimeInteractiveCrawler
{
    public class NetworkManager
    {

        private NetworkStream stream;
        public NetworkManager()
        {

        }

        public void SendData(Packet packet)
        {
            //write id of the packet
            stream.WriteByte(packet.Id);
            //write size of the packet
            stream.Write(BitConverter.GetBytes(packet.Size), 0, 4);
            //write data
            stream.Write(packet.Data, 0, packet.Data.Length);
        }

        public Packet ReadData()
        {
            byte id = (byte)stream.ReadByte();
            byte[] sizeBytes = new byte[4];
            stream.Read(sizeBytes, 0, sizeBytes.Length);
            int size = BitConverter.ToInt32(sizeBytes, 0);
            byte[] data = new byte[size];
            stream.Read(data, 0, size);

            return new Packet(id, size, data);
        }

        public void Connect(String server)
        {
            try
            {
                Int32 port = 8888;
                TcpClient client = new TcpClient(server, port);
                stream = client.GetStream();
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
    }
}
