using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace RealtimeInteractiveCrawler
{
    // TODO Singleton
    class World : Transformable, Drawable
    {
        public const int WORLD_WIDTH = 300;
        public const int WORLD_HEIGHT = 100;

        Tile[,] tiles;

        public World()
        {
            tiles = new Tile[WORLD_WIDTH, WORLD_HEIGHT];
        }

        public void GenerateWorld(int seed)
        {

            int size = WORLD_SIZE * Chunk.CHUNK_SIZE;
            MapHandler mapHandler = new MapHandler(size, size, 30, seed);
            for (int i = 0; i < 6; i++)
            {
                mapHandler.DoSimulationStep();
            }

            for (int x = 0; x < mapHandler.MapWidth; x++)
                for (int y = 0; y < mapHandler.MapHeight; y++)
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

        public Tile GetTile(int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            if (chunk == null)
                return null;

            Vector2i tilePos = GetTilePosFromChunk(x, y);

        }

        public void SetTile(TileType type, int i, int j)
        {
            Tile[] neighbours = new Tile[4];
            neighbours[0] = GetTile(i, j - 1); // up
            neighbours[1] = GetTile(i, j + 1); // down
            neighbours[2] = GetTile(i - 1, j); // left 
            neighbours[3] = GetTile(i + 1, j); // right

            if (type != TileType.NONE)
            {
                var tile = new Tile(type, neighbours);
                tile.Position = new Vector2f(i * Tile.TILE_SIZE, j * Tile.TILE_SIZE) + Position;
                tiles[i, j] = tile;
            }
            else
            {
                tiles[i, j] = null;
                if (neighbours[0] != null) neighbours[0].DownTile = null;
                if (neighbours[1] != null) neighbours[1].UpTile = null;
                if (neighbours[2] != null) neighbours[2].RightTile = null;
                if (neighbours[3] != null) neighbours[3].LeftTile = null;
            }
        }

        public Tile GerTileByWorldPos(float x, float y)
        {
            int i = (int)(x / Tile.TILE_SIZE);
            int j = (int)(y / Tile.TILE_SIZE);
            return GetTile(i, j);
        }

        public Tile GerTileByWorldPos(Vector2f pos)
        {
            return GerTileByWorldPos(pos.X, pos.Y);
        }
        public Tile GerTileByWorldPos(Vector2i pos)
        {
            return GerTileByWorldPos(pos.X, pos.Y);
        }

        public Tile GetTile(int i, int j)
        {
            if (i >= 0 && j >= 0 && i < WORLD_WIDTH && j < WORLD_HEIGHT)
                return tiles[i, j];
            else
                return null;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            for (int i = 0; i < GameLoop.Window.Size.X / Tile.TILE_SIZE + 1; i++)
            {
                for (int j = 0; j < GameLoop.Window.Size.Y / Tile.TILE_SIZE + 1 / 2; j++)
                {
                    if (tiles[i, j] != null)
                        target.Draw(tiles[i, j]);
                }
            }
        }
    }
}
