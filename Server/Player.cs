using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Server
{
    public class Player
    {
        public int PosX
        {
            get;
            set;
        }
        public int PosY
        {
            get;
            set;
        }
        public int ConnId
        {
            get;
            set;
        }

        public Vector2 Position
        {
            get;
            private set;
        } = new Vector2();

        public Player(int x, int y, int connId)
        {
            PosX = x;
            PosY = y;
            ConnId = connId;
            Position = new Vector2(x, y);
        }
    }
}
