﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLib
{
    public class PacketBuilder
    {
        private byte id;
        private List<byte> data = new List<byte>();
        public PacketBuilder(byte id)
        {
            this.id = id;
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
                return new Packet(id, data.Count, data.ToArray());
            }
            return new Packet(id, 0, new byte[0]);
        }
    }
}
