using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    
    class Player
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

        public Player(int x, int y, int connId)
        {
            PosX = x;
            PosY = y;
            ConnId = connId;
        }
    }
}
