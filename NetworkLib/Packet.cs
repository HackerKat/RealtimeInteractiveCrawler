using System;

namespace NetworkLib
{
    public enum PacketType
    {
        INIT,
        PING,
        NEW_PLAYER,
        UPDATE_MY_POS,
        UPDATE_OTHER_POS,
        UPDATE_ENEMY
    }
    public class Packet
    {
        public PacketType PacketType
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
        public Packet(PacketType packetType, int size, byte[] data)
        {
            this.PacketType = packetType;
            this.Size = size;
            this.data = new byte[size];
            Array.Copy(data, this.data, size);
        }
    }
}
