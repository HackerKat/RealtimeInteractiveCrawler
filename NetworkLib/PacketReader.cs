using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLib
{
    public class PacketReader
    {
        private Queue<byte> data = new Queue<byte>();

        public PacketReader(Packet packet)
        {
            foreach(byte b in packet.Data)
            {
                data.Enqueue(b);
            }
        }

        public byte GetByte()
        {
            return data.Dequeue();
        }

        public int GetInt()
        {
            byte[] d = new byte[4];
            for(int i = 0; i < 4; i++)
            {
                d[i] = data.Dequeue();
            }
            return BitConverter.ToInt32(d, 0);
        }

        public float GetFloat()
        {
            byte[] d = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                d[i] = data.Dequeue();
            }
            return BitConverter.ToSingle(d, 0);
        }

        public bool GetBool()
        {
            byte[] d = new byte[1];
            d[0] = data.Dequeue();
            return BitConverter.ToBoolean(d, 0);
        }

        public char GetChar()
        {
            byte[] d = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                d[i] = data.Dequeue();
            }
            return BitConverter.ToChar(d, 0);
        }

    }
}
