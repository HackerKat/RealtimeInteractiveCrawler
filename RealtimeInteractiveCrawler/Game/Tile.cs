using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    enum TileType
    {
        NONE,
        GROUND,
        GRASS,
        PLAYER
    }

    class Tile : Transformable, Drawable
    {
        public const int TILE_SIZE = 16;

        TileType type = TileType.GROUND;
        RectangleShape rectShape;

        // Neighbours
        Tile upTile;
        Tile downTile;
        Tile leftTile;
        Tile rightTile;

        public Tile UpTile
        {
            set
            {
                upTile = value;
                UpdateView();
            }
            get { return upTile; }
        }
        public Tile DownTile
        {
            set
            {
                downTile = value;
                UpdateView();
            }
            get { return downTile; }
        }
        public Tile LeftTile
        {
            set
            {
                leftTile = value;
                UpdateView();
            }
            get { return leftTile; }
        }
        public Tile RightTile
        {
            set
            {
                rightTile = value;
                UpdateView();
            }
            get { return rightTile; }
        }

        public Tile(TileType type, Tile[] neighbours)
        {
            rectShape = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
            this.type = type;

            // Setting neighbours
            if(neighbours[0] != null) // up
            {
                UpTile = neighbours[0];
                UpTile.DownTile = this;
            }
            if(neighbours[1] != null) // down
            {
                DownTile = neighbours[1];
                DownTile.UpTile = this;
            }
            if(neighbours[2] != null) // left
            {
                LeftTile = neighbours[2];
                LeftTile.RightTile = this;
            }
            if(neighbours[3] != null) // right
            {
                RightTile = neighbours[3];
                RightTile.LeftTile = this;
            }

            switch (type)
            {
                case TileType.GRASS:
                    rectShape.Texture = Content.TexTile1;
                    break;
                case TileType.GROUND:
                    rectShape.Texture = Content.TexTile0;                
                    break;
                case TileType.PLAYER:
                    rectShape.Texture = Content.TexPlay0;
                    break;
                default:
                    break;
            }

            UpdateView();
        }

        public void UpdateView()
        {
            int i = MainClass.Rand.Next(0, 3);
            // When tile has neighbours on each side
            if (upTile != null && downTile != null && leftTile != null && rightTile != null)
            {
                rectShape.TextureRect = GetTextureRect(1 + i, 1);
            }
            // When tile has no neighbours
            else if(upTile == null && downTile == null && leftTile == null && rightTile == null)
            {
                rectShape.TextureRect = GetTextureRect(9 + i, 3);
            }

            // ------------------------------------ up / down / left / right
            // When there is no up neighbour
            else if (upTile == null && downTile != null && leftTile != null && rightTile != null)
            {
                rectShape.TextureRect = GetTextureRect(1 + i, 0);
            }
            // When there is no down neighbour
            else if (upTile != null && downTile == null && leftTile != null && rightTile != null)
            {
                rectShape.TextureRect = GetTextureRect(1 + i, 2);
            }
            // When there is no left neighbour
            else if (upTile != null && downTile != null && leftTile == null && rightTile != null)
            {
                rectShape.TextureRect = GetTextureRect(0, i);
            }
            // When there is no right neighbour
            else if (upTile != null && downTile != null && leftTile != null && rightTile == null)
            {
                rectShape.TextureRect = GetTextureRect(4, i);
            }

            // ------------------------------------
            // when there is no up and left neighbour
            else if (upTile == null && downTile != null && leftTile == null && rightTile != null)
            {
                rectShape.TextureRect = GetTextureRect(i * 2, 3);
            }
            // when there is no up and right neighbour
            else if (upTile == null && downTile != null && leftTile != null && rightTile == null)
            {
                rectShape.TextureRect = GetTextureRect(1 + i * 2, 3);
            }
            // when there is no down and left neighbour
            else if (upTile != null && downTile == null && leftTile == null && rightTile != null)
            {
                rectShape.TextureRect = GetTextureRect(i * 2, 4);
            }
            // when there is no down and right neighbour
            else if (upTile != null && downTile == null && leftTile != null && rightTile == null)
            {
                rectShape.TextureRect = GetTextureRect(1 + i * 2, 4);
            }

        }

        public IntRect GetTextureRect(int i, int j)
        {
            int x = i * TILE_SIZE + i * 2;
            int y = j * TILE_SIZE + j * 2;
            return new IntRect(x, y, TILE_SIZE, TILE_SIZE);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(rectShape, states);
        }
    }
}
