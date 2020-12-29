﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Packet
    {
        public byte Id 
        {
            get;
            private set;
        }
        public int Size
        {
            get;
            private set;
        }
        private byte[] data;
        public byte[] Data
        {
            get
            {
                return data;
            }
        }
        public Packet(byte id, int size, byte[] data)
        {
            this.Id = id;
            this.Size = size;
            Array.Copy(data, this.data, size);
        }


    }
}
