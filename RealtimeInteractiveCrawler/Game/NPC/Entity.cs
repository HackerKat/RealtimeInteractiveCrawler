using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RealtimeInteractiveCrawler
{
    abstract class Entity : Transformable, Drawable
    {
        public Vector2f StartPosition;
        public bool IsDestroyed;
        public bool IsItem;
        public SpriteSheet SpriteSheet;

        protected RectangleShape rect;
        protected Vector2f velocity;
        protected Vector2f movement;
        protected World world;
        protected bool isRectVisible = true;

        protected Sprite sprite;
        public Entity() 
        {
            world = AwesomeGame.world;
        }

        public virtual void Update()
        {
            UpdatePhysics();
        }

        public void Spawn()
        {
            Position = StartPosition;
            velocity = new Vector2f();
        }

        public void Spawn(float xStart, float yStart)
        {
            StartPosition = new Vector2f(xStart, yStart);
            Position = StartPosition;
        }

        private void UpdatePhysics()
        {
            if (IsItem) return;

            velocity.X *= 0.99f;

            Vector2f nextPos = Position + velocity - rect.Origin;
            FloatRect playerRect = new FloatRect(nextPos, rect.Size);

            int pX = (int)((Position.X - rect.Origin.X + rect.Size.X * 0.5f) / Tile.TILE_SIZE);
            int pY = (int)((Position.Y + rect.Size.Y * 0.5f) / Tile.TILE_SIZE);

            UpdatePhysicsWall(playerRect, pX, pY);
        }

        private void UpdatePhysicsWall(FloatRect playerRect, int pX, int pY)
        {
            List<Tile> walls = new List<Tile>
            {
                // Left
                world.GetTile(pX - 1, pY),
                world.GetTile(pX - 1, pY - 1),
                world.GetTile(pX - 1, pY + 1),
                // Right
                world.GetTile(pX + 1, pY),
                world.GetTile(pX + 1, pY - 1),
                world.GetTile(pX + 1, pY + 1),
                // Top
                world.GetTile(pX, pY - 1),
                world.GetTile(pX - 1, pY - 1),
                world.GetTile(pX + 1, pY - 1),
                // Down
                world.GetTile(pX, pY),
                world.GetTile(pX - 1, pY),
                world.GetTile(pX + 1, pY),
            };

            //for (int i = 0; i < AwesomeGame.PlayerTiles.Count; i++)
            //{
            //    walls.Add(AwesomeGame.PlayerTiles[i]);
            //    AwesomeGame.PlayerTiles[i].Position = AwesomeGame.Players[i].Position;
            //}

            foreach (var tile in walls)
            {
                if (tile == null || tile.type == TileType.GROUND) continue;

                FloatRect tileRect = new FloatRect(tile.Position, new Vector2f(Tile.TILE_SIZE, Tile.TILE_SIZE));
                DebugRender.AddRectangle(tileRect, Color.Yellow);

                if (playerRect.Intersects(tileRect))
                {
                    Vector2f offset = new Vector2f(playerRect.Left - tileRect.Left, playerRect.Top - tileRect.Top);
                    offset.X /= Math.Abs(offset.X);
                    offset.Y /= Math.Abs(offset.Y);

                    float speed = Math.Abs(movement.X);

                    // Left walls
                    if (offset.X > 0)
                    {
                        // Sends the player one tile away
                        Position = new Vector2f(tileRect.Left + tileRect.Width + playerRect.Width * 0.5f, Position.Y);
                        movement.X = 0;
                        velocity.X = 0;
                    }
                    // Right walls
                    else if (offset.X < 0)
                    {
                        Position = new Vector2f(tileRect.Left - playerRect.Width * 0.5f, Position.Y);
                        movement.X = 0;
                        velocity.X = 0;
                        //Position = new Vector2f(Position.X - 2, Position.Y);
                    }
                    // Top walls
                    else if (offset.Y > 0)
                    {
                        Position = new Vector2f(Position.X, tileRect.Top + tileRect.Height + playerRect.Height * 0.5f);
                        movement.Y = 0;
                        velocity.Y = 0;
                        //Position = new Vector2f(Position.X, Position.Y + 2);
                    }
                    // Down walls
                    else if (offset.Y < 0)
                    {
                        Position = new Vector2f(Position.X, tileRect.Top - playerRect.Height * 0.5f);
                        movement.Y = 0;
                        velocity.Y = 0;
                        //Position = new Vector2f(Position.X, Position.Y - 2);
                    }

                    OnWallCollided();

                }
            }
        }


        public abstract void OnWallCollided();
        public abstract void Draw(RenderTarget target, RenderStates states);
    }
}
