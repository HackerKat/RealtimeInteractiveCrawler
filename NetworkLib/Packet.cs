using System;

namespace NetworkLib
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
            this.data = new byte[size];
            Array.Copy(data, this.data, size);
        }
    }
}
