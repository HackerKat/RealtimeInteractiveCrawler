using System;
using System.Collections.Generic;
using System.Numerics;
using System.Collections.Concurrent;

namespace Server
{
    // TODO Singleton
    public class World
    {
        public const int WORLD_SIZE = 5;

        public Chunk[][] chunks;
        public ConcurrentBag<Entity> enemies = new ConcurrentBag<Entity>();
        private int entityId = 1000;
        
        //public static Dictionary<Tile, Item> Items = new Dictionary<Tile, Item>();

        public World()
        {
            chunks = new Chunk[WORLD_SIZE][];

            for (int i = 0; i < WORLD_SIZE; i++)
            {
                chunks[i] = new Chunk[WORLD_SIZE];
            }
        }

        public Vector2 GetSpawnPoint(Random rand)
        {
            Chunk firstChunk = chunks[0][0];
            int tx = rand.Next(Chunk.CHUNK_SIZE);
            int ty = rand.Next(Chunk.CHUNK_SIZE);

            while(firstChunk.GetTile(tx, ty).type != TileType.GROUND)
            {
                tx = rand.Next(Chunk.CHUNK_SIZE);
                ty = rand.Next(Chunk.CHUNK_SIZE);
            }

            float x = firstChunk.GetTile(tx, ty).Position.X;
            float y = firstChunk.GetTile(tx, ty).Position.Y;

            return new Vector2((float)tx, (float)ty);
        }

        public void GenerateWorld(int seed)
        {
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

            List<Vector2> enemyTiles = mapHandler.PlaceEnemies();
            for (int i = 0; i < enemyTiles.Count; i++)
            {
                Tile t = GetTile((int)enemyTiles[i].X, (int)enemyTiles[i].Y);
                Vector2 absolPos = t.Position;
                Chunk chunk = GetChunk((int)enemyTiles[i].X, (int)enemyTiles[i].Y);

                enemies.Add(new Entity((int)absolPos.X, (int)absolPos.Y, entityId++, chunk));
            }
            Console.WriteLine(enemyTiles.Count);
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
    }
}
