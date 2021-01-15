using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RealtimeInteractiveCrawler
{
    // TODO Singleton
    class World : Transformable, Drawable
    {
        public const int WORLD_SIZE = 3;

        Chunk[][] chunks;
        float counter;

        public World()
        {
            chunks = new Chunk[WORLD_SIZE][];

            for (int i = 0; i < WORLD_SIZE; i++)
            {
                chunks[i] = new Chunk[WORLD_SIZE];
            }
        }

        public void GenerateWorld(int seed)
        {

            int size = WORLD_SIZE * Chunk.CHUNK_SIZE;
            MapHandler mapHandler = new MapHandler(size, size, 30, seed);
            for (int i = 0; i < 6; i++)
            {
                mapHandler.DoSimulationStep();
            }

            for (int y = 0; y < mapHandler.MapHeight; y++)
                for (int x = 0; x < mapHandler.MapWidth; x++)
                    SetTile(mapHandler.Map[x, y], x, y);

            List<Vector2i> treasures = mapHandler.PlaceTreasure();
            for (int i = 0; i < treasures.Count; i++)
            {
                mapHandler.Map[treasures[i].X, treasures[i].Y] = TileType.SLIME;
                SetTile(TileType.SLIME, treasures[i].X, treasures[i].Y);
            }

            List<Vector2i> enemies = mapHandler.PlaceEnemies();
            for (int i = 0; i < enemies.Count; i++)
            {
                mapHandler.Map[(int)enemies[i].X, (int)enemies[i].Y] = TileType.ENEMY;
                SetTile(TileType.ENEMY, (int)enemies[i].X, (int)enemies[i].Y);
            }

            Tile tile = GetTile(10, 10);
            if(tile != null)
            {
                return;
            }

        }

        public void SetTile(TileType type, int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            Vector2i tilePos = GetTilePosFromChunk(x, y);

            Tile[] neighbours = new Tile[4];
            neighbours[0] = GetTile(x, y - 1); // up
            neighbours[1] = GetTile(x, y + 1); // down
            neighbours[2] = GetTile(x - 1, y); // left 
            neighbours[3] = GetTile(x + 1, y); // right

            chunk.SetTile(type, tilePos.X, tilePos.Y, neighbours);
        }

        // x and y are absolut coordinates
        public Tile GetTile(int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            
            if (chunk == null)
                return null;

            Vector2i tilePos = GetTilePosFromChunk(x, y);

            return chunk.GetTile(tilePos.X, tilePos.Y);
        }

        // Gets absolut pos
        public Tile GetTileAbsolutPos(int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            if (chunk == null)
                return null;

            int X = x + chunk.chunkPos.X * Chunk.CHUNK_SIZE;
            int Y = y + chunk.chunkPos.Y * Chunk.CHUNK_SIZE;

            return chunk.GetTile(X, Y);
        }

        // Return chunk coordinates in array
        // x and y are absolute coordinates
        public Chunk GetChunk(int x, int y)
        {
            int X = x / Chunk.CHUNK_SIZE;
            int Y = y / Chunk.CHUNK_SIZE;

            if (X >= WORLD_SIZE || Y >= WORLD_SIZE || X < 0 || Y < 0)
            {
                return null;
            }

            try
            {
                if (chunks[X][Y] == null)
                    chunks[X][Y] = new Chunk(new Vector2i(X, Y));

                return chunks[X][Y];
            }
            catch (Exception){ return null; }
                
        }
        // Gets absolut coordinates
        // Returns relative tile Pos
        public Vector2i GetTilePosFromChunk(int x, int y)
        {
            int X = x / Chunk.CHUNK_SIZE;
            int Y = y / Chunk.CHUNK_SIZE;

            return new Vector2i(x - X * Chunk.CHUNK_SIZE, y - Y * Chunk.CHUNK_SIZE);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                for (int y = 0; y < WORLD_SIZE; y++)
                {
                    if (chunks[x][y] == null) continue;

                    target.Draw(chunks[x][y]);
                }
            }
        }
    }
}
