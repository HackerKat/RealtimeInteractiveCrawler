using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
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

    class Tile : Transformable, Drawable
    {
        public const int TILE_SIZE = 16;

        public TileType type = TileType.GROUND;

        RectangleShape rectShape;
        SpriteSheet spriteSheet;

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

            switch (type)
            {
                case TileType.GRASS:
                    rectShape.Texture = Content.TexGrass;
                    break;
                case TileType.GROUND:
                    rectShape.Texture = Content.TextGround;                
                    break;
                case TileType.PLAYER:
                    rectShape.Texture = Content.TexPlayer;
                    break;
                case TileType.SLIME:
                    rectShape.Texture = Content.TexPlay1;
                    break;
                case TileType.ENEMY:
                    rectShape.Texture = Content.TexPlay2;
                    break;
                default:
                    break;
            }

            spriteSheet = new SpriteSheet(TILE_SIZE, TILE_SIZE, 1);

            UpdateView();
        }

        public void UpdateView()
        {
            int i = AwesomeGame.Rand.Next(0, 3);
            // When tile has neighbours on each side
            if (upTile != null && downTile != null && leftTile != null && rightTile != null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(1 + i, 1);
            }
            // When tile has no neighbours
            else if(upTile == null && downTile == null && leftTile == null && rightTile == null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(9 + i, 3);
            }

            // ------------------------------------ up / down / left / right
            // When there is no up neighbour
            else if (upTile == null && downTile != null && leftTile != null && rightTile != null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(1 + i, 0);
            }
            // When there is no down neighbour
            else if (upTile != null && downTile == null && leftTile != null && rightTile != null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(1 + i, 2);
            }
            // When there is no left neighbour
            else if (upTile != null && downTile != null && leftTile == null && rightTile != null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(0, i);
            }
            // When there is no right neighbour
            else if (upTile != null && downTile != null && leftTile != null && rightTile == null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(4, i);
            }

            // ------------------------------------
            // when there is no up and left neighbour
            else if (upTile == null && downTile != null && leftTile == null && rightTile != null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(i * 2, 3);
            }
            // when there is no up and right neighbour
            else if (upTile == null && downTile != null && leftTile != null && rightTile == null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(1 + i * 2, 3);
            }
            // when there is no down and left neighbour
            else if (upTile != null && downTile == null && leftTile == null && rightTile != null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(i * 2, 4);
            }
            // when there is no down and right neighbour
            else if (upTile != null && downTile == null && leftTile != null && rightTile == null)
            {
                rectShape.TextureRect = spriteSheet.GetTextureRect(1 + i * 2, 4);
            }

        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(rectShape, states);
        }

        public FloatRect GetFloatRect()
        {
            return new FloatRect(Position, new Vector2f(TILE_SIZE, TILE_SIZE));
        }
    }
}
