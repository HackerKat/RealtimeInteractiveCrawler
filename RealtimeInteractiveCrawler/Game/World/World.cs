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

        public int Seed;

        Tile[,] tiles;

        public World(int seed)
        {
            tiles = new Tile[WORLD_WIDTH, WORLD_HEIGHT];

            Seed = seed;
        }

        public void GenerateWorld()
        {
            AwesomeGame.Rand = Seed >= 0 ? new Random(Seed) : new Random((int)DateTime.Now.Ticks);
            //// Ground
            //int[] xCords = { 3, 46 };
            //int[] yCords = { 18, 32 };
            //for (int x = xCords[0]; x <= xCords[1]; x++)
            //    for (int y = yCords[0]; y <= yCords[1]; y++)
            //        SetTile(TileType.GROUND, x, y);
            //// Grass
            //xCords = new int[]{ 3, 46 };
            //yCords = new int[] { 17, 17 };
            //for (int x = xCords[0]; x <= xCords[1]; x++)
            //    for (int y = yCords[0]; y <= yCords[1]; y++)
            //        SetTile(TileType.GRASS, x, y);

            //// Random Walls
            //xCords = new int[] { 3, 4 };
            //yCords = new int[] { 1, 17 };
            //for (int x = xCords[0]; x <= xCords[1]; x++)
            //    for (int y = yCords[0]; y <= yCords[1]; y++)
            //        SetTile(TileType.GROUND, x, y);
            //xCords = new int[] { 45, 46 };
            //yCords = new int[] { 1, 17 };
            //for (int x = xCords[0]; x <= xCords[1]; x++)
            //    for (int y = yCords[0]; y <= yCords[1]; y++)
            //        SetTile(TileType.GROUND, x, y);
            //xCords = new int[] { 3, 46 };
            //yCords = new int[] { 1, 2 };
            //for (int x = xCords[0]; x <= xCords[1]; x++)
            //    for (int y = yCords[0]; y <= yCords[1]; y++)
            //        SetTile(TileType.GROUND, x, y);


            // Generate Dungeon
            MapHandler mapHandler = new MapHandler(WORLD_WIDTH, WORLD_HEIGHT, 30);
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
