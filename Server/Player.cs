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
        public int ConnId
        {
            get;
            set;
        }

        public Vector2 Position
        {
            get;
            set;
        } = new Vector2();

        public Player(float x, float y, int connId)
        {
            ConnId = connId;
            Position = new Vector2(x, y);
        }
    }
}
