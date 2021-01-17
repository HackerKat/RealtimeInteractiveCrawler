using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Server
{
    public enum TileType
    {
        NONE,
        GROUND,
        GRASS,
        SLIME,
        PLAYER,
        ENEMY
    }

    public class Tile
    {
        public const int TILE_SIZE = 16;

        public TileType type = TileType.GROUND;
        
        // Neighbours
        Tile upTile;
        Tile downTile;
        Tile leftTile;
        Tile rightTile;

        public Vector2 Position
        {
            get;
            set;
        }

        public Tile UpTile
        {
            set
            {
                upTile = value;
            }
            get { return upTile; }
        }
        public Tile DownTile
        {
            set
            {
                downTile = value;
            }
            get { return downTile; }
        }
        public Tile LeftTile
        {
            set
            {
                leftTile = value;
            }
            get { return leftTile; }
        }
        public Tile RightTile
        {
            set
            {
                rightTile = value;
            }
            get { return rightTile; }
        }

        public Tile(TileType type, Tile[] neighbours)
        {
            //rectShape = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
            this.type = type;

            // Setting neighbours
            if(neighbours[0] != null) // up
            {
                upTile = neighbours[0];
                upTile.DownTile = this;
            }
            if(neighbours[1] != null) // down
            {
                downTile = neighbours[1];
                downTile.UpTile = this;
            }
            if(neighbours[2] != null) // left
            {
                leftTile = neighbours[2];
                leftTile.RightTile = this;
            }
            if(neighbours[3] != null) // right
            {
                rightTile = neighbours[3];
                rightTile.LeftTile = this;
            }
        }
    }
}
