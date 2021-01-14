using System;
using System.Collections.Generic;
using System.Numerics;

namespace Server
{
    // TODO Singleton
    class World
    {
        public const int WORLD_SIZE = 5;

        Chunk[][] chunks;

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
            if (false)
            {/*
            // Ground
            int[] xCords = { 3, 46 };
            int[] yCords = { 18, 32 };
            for (int x = xCords[0]; x <= xCords[1]; x++)
                for (int y = yCords[0]; y <= yCords[1]; y++)
                    SetTile(TileType.GROUND, x, y);
            // Grass
            xCords = new int[]{ 3, 46 };
            yCords = new int[] { 17, 17 };
            for (int x = xCords[0]; x <= xCords[1]; x++)
                for (int y = yCords[0]; y <= yCords[1]; y++)
                    SetTile(TileType.GRASS, x, y);

            // Random Walls
            xCords = new int[] { 3, 4 };
            yCords = new int[] { 1, 17 };
            for (int x = xCords[0]; x <= xCords[1]; x++)
                for (int y = yCords[0]; y <= yCords[1]; y++)
                    SetTile(TileType.GROUND, x, y);
            xCords = new int[] { 45, 46 };
            yCords = new int[] { 1, 17 };
            for (int x = xCords[0]; x <= xCords[1]; x++)
                for (int y = yCords[0]; y <= yCords[1]; y++)
                    SetTile(TileType.GROUND, x, y);
            xCords = new int[] { 3, 46 };
            yCords = new int[] { 1, 2 };
            for (int x = xCords[0]; x <= xCords[1]; x++)
                for (int y = yCords[0]; y <= yCords[1]; y++)
                    SetTile(TileType.GROUND, x, y);
            */
            }
            int size = WORLD_SIZE * Chunk.CHUNK_SIZE;
            MapHandler mapHandler = new MapHandler(size, size, 30, seed);
            for (int i = 0; i < 6; i++)
            {
                mapHandler.DoSimulationStep();
            }

            for (int x = 0; x < mapHandler.MapHeight; x++)
                for (int y = 0; y < mapHandler.MapWidth; y++)
                    SetTile(mapHandler.Map[x, y], x, y);

            List<Vector2> treasures = mapHandler.PlaceTreasure();
            for (int i = 0; i < treasures.Count; i++)
            {
                mapHandler.Map[(int)treasures[i].X, (int)treasures[i].Y] = TileType.SLIME;
                SetTile(TileType.SLIME, (int)treasures[i].X, (int)treasures[i].Y);
            }
        
        }

        public void SetTile(TileType type, int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            Vector2 tilePos = GetTilePosFromChunk(x, y);

            Tile[] neighbours = new Tile[4];
            neighbours[0] = GetTile(x, y - 1); // up
            neighbours[1] = GetTile(x, y + 1); // down
            neighbours[2] = GetTile(x - 1, y); // left 
            neighbours[3] = GetTile(x + 1, y); // right

            chunk.SetTile(type, (int)tilePos.X, (int)tilePos.Y, neighbours);
        }

        public Tile GetTile(int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            if (chunk == null)
                return null;

            Vector2 tilePos = GetTilePosFromChunk(x, y);

            return chunk.GetTile((int)tilePos.X, (int)tilePos.Y);
        }

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
                    chunks[X][Y] = new Chunk(new Vector2(X, Y));

                return chunks[X][Y];
            }
            catch (Exception)
            {

                return null;
            }

        }

        public Vector2 GetTilePosFromChunk(int x, int y)
        {
            int X = x / Chunk.CHUNK_SIZE;
            int Y = y / Chunk.CHUNK_SIZE;

            return new Vector2(x - X * Chunk.CHUNK_SIZE, y - Y * Chunk.CHUNK_SIZE);
        }

        /*
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
        */
    }
}
