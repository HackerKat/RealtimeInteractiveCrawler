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

        public Tile(TileType type)
        {
            rectShape = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
            this.type = type;

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

            rectShape.TextureRect = GetTextureRect(1, 1);
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
