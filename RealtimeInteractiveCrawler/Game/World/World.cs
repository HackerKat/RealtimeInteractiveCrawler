﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static RealtimeInteractiveCrawler.Item;

namespace RealtimeInteractiveCrawler
{
    // TODO Singleton
    class World : Transformable, Drawable
    {
        public const int WORLD_SIZE = 5;

        //public List<Enemy> enemies = new List<Enemy>();

        public Chunk[][] chunks;
        // 0 = health, 1 = attack, 2 = defense, 3 = eraser
        private List<SpriteSheet> itemSpriteSheets = new List<SpriteSheet>();
        //private List<Item> items = new List<Item>();
        public static Dictionary<Tile, Item> Items = new Dictionary<Tile, Item>();
        private int itemCount = 0;

        public World()
        {
            chunks = new Chunk[WORLD_SIZE][];

            for (int i = 0; i < WORLD_SIZE; i++)
            {
                chunks[i] = new Chunk[WORLD_SIZE];
            }

            itemSpriteSheets.Add(Content.SpriteHealth);
            itemSpriteSheets.Add(Content.SpriteAttack);
            itemSpriteSheets.Add(Content.SpriteDefense);
            // TODO other items
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

            // Items
            ItemType[] itemTypes = new ItemType[]
            {
                ItemType.ATTACK,
                ItemType.DEFENSE,
                ItemType.HEALTH
            };
            
            List<Vector2i> itemsAttack = mapHandler.PlaceTreasure(4);
            for (int i = 0; i < itemsAttack.Count; i++)
            {
                mapHandler.Map[itemsAttack[i].X, itemsAttack[i].Y] = TileType.ITEM;
                int nextType = AwesomeGame.Rand.Next(0, itemTypes.Length);
                SetItem(itemTypes[nextType], itemsAttack[i].X, itemsAttack[i].Y);
            }
        }

        public void SetItem(ItemType itemType, int x, int y)
        {
            SpriteSheet neededSprite = null;
            switch (itemType)
            {
                case ItemType.HEALTH:
                    neededSprite = itemSpriteSheets[0];
                    break;
                case ItemType.ATTACK:
                    neededSprite = itemSpriteSheets[1];
                    break;
                case ItemType.DEFENSE:
                    neededSprite = itemSpriteSheets[2];
                    break;
                case ItemType.ERASER:
                    neededSprite = itemSpriteSheets[3];
                    break;
                default:
                    break;

            }

            Tile tile = GetTile(x, y);
            tile.type = TileType.ITEM;

            Item item = new Item(itemType, neededSprite, 0, 0, itemCount, 0.125f);
            itemCount++;
            Items[tile] = item;

            item.Spawn(tile.Position.X, tile.Position.Y);
        }

        public void SetTile(TileType tileType, int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            Vector2i tilePos = GetTilePosFromChunk(x, y);
            Tile[] neighbours = new Tile[4];

            neighbours[0] = GetTile(x, y - 1); // up
            neighbours[1] = GetTile(x, y + 1); // down
            neighbours[2] = GetTile(x - 1, y); // left 
            neighbours[3] = GetTile(x + 1, y); // right

            chunk.SetTile(tileType, tilePos.X, tilePos.Y, neighbours);
        }

        public Tile GetTile(int x, int y)
        {
            Chunk chunk = GetChunk(x, y);

            if (chunk == null)
                return null;

            Vector2i tilePos = GetTilePosFromChunk(x, y);

            return chunk.GetTile(tilePos.X, tilePos.Y);
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
                    chunks[X][Y] = new Chunk(new Vector2i(X, Y));

                return chunks[X][Y];
            }
            catch (Exception) { return null; }

        }

        public Vector2i GetTilePosFromChunk(int x, int y)
        {
            int X = x / Chunk.CHUNK_SIZE;
            int Y = y / Chunk.CHUNK_SIZE;

            return new Vector2i(x - X * Chunk.CHUNK_SIZE, y - Y * Chunk.CHUNK_SIZE);
        }

        public void Update()
        {
            // Items
            Tile keyWhereToRemove = null;
            foreach (var item in Items)
            {
                if (item.Value.IsDestroyed)
                    keyWhereToRemove = item.Key;
                else
                    item.Value.Update();
            }
            if (keyWhereToRemove != null)
                Items.Remove(keyWhereToRemove);
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

            foreach (var item in Items.Values)
            {
                double distanceToPlayer = AwesomeGame.Distance(item.Position, AwesomeGame.Player.Position);
                if (distanceToPlayer > Tile.TILE_SIZE * Chunk.CHUNK_SIZE * 0.5f)
                    continue;
                target.Draw(item);
            }
        }
    }
}
