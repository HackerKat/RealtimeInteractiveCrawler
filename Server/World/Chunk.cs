using System.Numerics;

namespace Server
{
    public class Chunk
    {
        public const int CHUNK_SIZE = 25;

        Tile[][] tiles;
        Vector2 chunkPos;
        public Vector2 Position
        {
            get;
            set;
        }

        public Chunk(Vector2 chunkPos)
        {
            this.chunkPos = chunkPos;
            Position = new Vector2(chunkPos.X * CHUNK_SIZE * Tile.TILE_SIZE, chunkPos.Y * CHUNK_SIZE * Tile.TILE_SIZE);

            tiles = new Tile[CHUNK_SIZE][];

            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                tiles[i] = new Tile[CHUNK_SIZE];
            }

            //for (int x = 0; x < CHUNK_SIZE; x++)
            //    for (int y = 0; y < CHUNK_SIZE; y++)
            //        SetTile(TileType.GROUND, x, y);
        }

        public void SetTile(TileType type, int x, int y, Tile[] neighbours)
        {
            tiles[x][y] = new Tile(type, neighbours);
            tiles[x][y].Position = new Vector2(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE) + Position;
        }
        
        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= CHUNK_SIZE || y >= CHUNK_SIZE)
                return null;

            return tiles[x][y];
        }

        /*
        public void Draw(RenderTarget target, RenderStates states)
        {

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    if (tiles[x][y] == null) continue;

                    target.Draw(tiles[x][y]);
                }
            }
        }
        */
    }
}
