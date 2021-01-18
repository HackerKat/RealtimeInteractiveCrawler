using SFML.Graphics;
using SFML.System;
using System;

namespace RealtimeInteractiveCrawler
{
    public class Chunk : Transformable, Drawable
    {
        public const int CHUNK_SIZE = 25;
        public Vector2i chunkPos;

        private Tile[][] tiles;
        

        public Chunk(Vector2i chunkPos)
        {
            this.chunkPos = chunkPos;
            Position = new Vector2f(chunkPos.X * CHUNK_SIZE * Tile.TILE_SIZE, chunkPos.Y * CHUNK_SIZE * Tile.TILE_SIZE);
            Origin = new Vector2f(CHUNK_SIZE * Tile.TILE_SIZE * 0.5f, CHUNK_SIZE * Tile.TILE_SIZE * 0.5f);
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
            tiles[x][y] = new Tile(type, neighbours, this);
            tiles[x][y].Position = new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE) + Position;
        }
        
        // Gets relative position
        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= CHUNK_SIZE || y >= CHUNK_SIZE)
                return null;

            return tiles[x][y];
        }

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

    }
}
