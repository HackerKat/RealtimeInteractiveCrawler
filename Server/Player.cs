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
            get
            {
                return Origin;
            }
            set 
            {
                Origin = new Vector2(value.X + Tile.TILE_SIZE * 0.5f, value.Y - Tile.TILE_SIZE);
            }
        }

        public Vector2 Origin
        {
            get;
            set;
        }

        public Player(float x, float y, int connId)
        {
            ConnId = connId;
            Position = new Vector2(x, y);

            
        }
    }
}
