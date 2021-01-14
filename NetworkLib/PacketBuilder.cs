using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLib
{
    public class PacketBuilder
    {
        private PacketType packetType;
        private List<byte> data = new List<byte>();
        public PacketBuilder(PacketType packetType)
        {
            this.packetType = packetType;
        }

        public void Add(byte b)
        {
            data.Add(b);
        }

        public void Add(int i)
        {
            data.AddRange(BitConverter.GetBytes(i));
        }

        public void Add(float f)
        {
            data.AddRange(BitConverter.GetBytes(f));
        }

        public void Add(bool b)
        {
            data.AddRange(BitConverter.GetBytes(b));
        }
        public void Add(char c)
        {
            data.AddRange(BitConverter.GetBytes(c));
        }

        public Packet Build()
        {
            if(data.Count != 0)
            {
                return new Packet(packetType, data.Count, data.ToArray());
            }
            return new Packet(packetType, 0, new byte[0]);
        }
    }
}
