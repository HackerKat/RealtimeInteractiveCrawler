using System.Numerics;

namespace Server
{
    public class Chunk
    {
        public const int CHUNK_SIZE = 25;

        Tile[][] tiles;
        public Vector2 chunkPos;
        public Vector2 Position
        {
            get;
            set;
        }

        public Vector2 Origin { get; set; }

        public Chunk(Vector2 chunkPos)
        {
            this.chunkPos = chunkPos;
            Position = new Vector2(chunkPos.X * CHUNK_SIZE * Tile.TILE_SIZE, chunkPos.Y * CHUNK_SIZE * Tile.TILE_SIZE);

            Origin = new Vector2(Position.X + Tile.TILE_SIZE * CHUNK_SIZE * 0.5f, Position.Y + Tile.TILE_SIZE * CHUNK_SIZE * 0.5f);

            tiles = new Tile[CHUNK_SIZE][];

            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                tiles[i] = new Tile[CHUNK_SIZE];
            }
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
    }
}
