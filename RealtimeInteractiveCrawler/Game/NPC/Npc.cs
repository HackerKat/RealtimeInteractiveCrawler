using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    abstract class Npc : Transformable, Drawable
    {
        public Vector2f StartPosition;

        protected RectangleShape rect;
        protected RectangleShape rectDirection;
        protected Vector2f velocity;
        protected Vector2f movement;
        protected World world;
        protected bool isFlying = true;

        public int Direction
        {
            set
            {
                int dir = value >= 0 ? 1 : -1;
                Scale = new Vector2f(dir, 1);
            }
            get
            {
                int dir = Scale.X >= 0 ? 1 : -1;
                return dir;
            }
        }

        public Npc(World world)
        {
            this.world = world;
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

        public void Update()
        {
            UpdateNPC();
            UpdatePhysics();

            Position += movement + velocity;
            // Fell down outside the window
            if (Position.Y > GameLoop.Window.Size.Y)
                OnKill();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(rect, states);

            DrawNPC(target, states);
        }

        private void UpdatePhysics()
        {
            bool isFalling = true;

            velocity.X *= 0.99f;
            // Gravity
            velocity.Y += 0.25f;

            Vector2f nextPos = Position + velocity - rect.Origin;
            FloatRect playerRect = new FloatRect(nextPos, rect.Size);

            int physicsX = (int)((Position.X - rect.Origin.X + rect.Size.X * 0.5f) / Tile.TILE_SIZE);
            int physicsY = (int)((Position.Y + rect.Size.Y) / Tile.TILE_SIZE);
            Tile tile = world.GetTile(physicsX, physicsY);

            if (tile != null)
            {

                FloatRect tileRect = new FloatRect(tile.Position, new Vector2f(Tile.TILE_SIZE, Tile.TILE_SIZE));

                DebugRender.AddRectangle(tileRect, Color.Red);
                // Collision
                isFalling = !playerRect.Intersects(tileRect);
                isFlying = isFalling;
            }
            if (!isFalling)
            {
                velocity.Y = 0;
            }

            UpdatePhysicsWall(playerRect, physicsX, physicsY);
        }

        private void UpdatePhysicsWall(FloatRect playerRect, int physicsX, int physicsY)
        {
            Tile[] walls = new Tile[]
            {
                world.GetTile(physicsX - 1, physicsY - 1),
                world.GetTile(physicsX - 1, physicsY - 2),
                world.GetTile(physicsX - 1, physicsY - 3),
                world.GetTile(physicsX + 1, physicsY - 1),
                world.GetTile(physicsX + 1, physicsY - 2),
                world.GetTile(physicsX + 1, physicsY - 3),
            };

            foreach (var tile in walls)
            {
                if (tile == null) continue;

                FloatRect tileRect = new FloatRect(tile.Position, new Vector2f(Tile.TILE_SIZE, Tile.TILE_SIZE));
                DebugRender.AddRectangle(tileRect, Color.Yellow);

                if (playerRect.Intersects(tileRect))
                {
                    Vector2f offset = new Vector2f(playerRect.Left - tileRect.Left, 0);
                    offset.X /= Math.Abs(offset.X);

                    float speed = Math.Abs(movement.X);

                    // Left walls
                    if (offset.X > 0)
                    {
                        Position = new Vector2f((tileRect.Left + tileRect.Width) + playerRect.Width * 0.5f, Position.Y);
                        movement.X = 0;
                        velocity.X = 0;
                    }
                    // Right walls
                    else if (offset.X < 0)
                    {
                        Position = new Vector2f(tileRect.Left - playerRect.Width * 0.5f, Position.Y);
                        movement.X = 0;
                        velocity.X = 0;
                    }

                    OnWallCollided();

                }
            }
        }

        public abstract void OnKill();
        public abstract void OnWallCollided();
        public abstract void UpdateNPC();
        public abstract void DrawNPC(RenderTarget target, RenderStates states);
    }
}
