using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace RealtimeInteractiveCrawler
{
    class Player : Transformable, Drawable
    {
        public const float PLAYER_MOVE_SPEED = 4f;
        public const float PLAYER_MOVE_SPEED_ACCELERATION = 0.2f;

        public Vector2f StartPosition;

        private RectangleShape rect;
        private RectangleShape rectDirection;
        Vector2f velocity;
        Vector2f movement;
        private World world;

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


        public Player(World world)
        {
            rect = new RectangleShape(new Vector2f(Tile.TILE_SIZE * 1.5f, Tile.TILE_SIZE * 2.8f));
            rect.Origin = new Vector2f(rect.Size.X * 0.5f, 0);

            rectDirection = new RectangleShape(new Vector2f(50, 3));
            rectDirection.FillColor = Color.Red;
            rectDirection.Position = new Vector2f(0, rect.Size.Y * 0.5f - 1);

            this.world = world;
        }

        public void Spawn(float xStart, float yStart)
        {
            StartPosition = new Vector2f(xStart, yStart);
            Position = StartPosition;
        }

        public void Spawn()
        {
            Position = StartPosition;
            velocity = new Vector2f();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(rect, states);
            target.Draw(rectDirection, states);
        }

        public void Update()
        {
            UpdateMovement();
            UpdatePhysics();
            

            Position += movement + velocity;

            if (Position.Y > GameLoop.Window.Size.Y)
                Spawn();
        }

        private void UpdateMovement()
        {
            bool movingUp = Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.Up);
            bool movingDown = Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down);
            bool movingLeft = Keyboard.IsKeyPressed(Keyboard.Key.A) || Keyboard.IsKeyPressed(Keyboard.Key.Left);
            bool movingRight = Keyboard.IsKeyPressed(Keyboard.Key.D) || Keyboard.IsKeyPressed(Keyboard.Key.Right);

            bool movingOnX = movingLeft || movingRight;

            if (movingOnX)
            {
                if (movingLeft)
                {
                    if (movement.X > 0)
                        movement.X = 0;

                    movement.X -= PLAYER_MOVE_SPEED_ACCELERATION;
                    Direction = -1;
                }
                else if (movingRight)
                {
                    if (movement.X < 0)
                        movement.X = 0;

                    movement.X += PLAYER_MOVE_SPEED_ACCELERATION;
                    Direction = 1;
                }

                if (movement.X > PLAYER_MOVE_SPEED)
                    movement.X = PLAYER_MOVE_SPEED;
                else if (movement.X < -PLAYER_MOVE_SPEED)
                    movement.X = -PLAYER_MOVE_SPEED;
            }
            else
            {
                movement = new Vector2f();
            }
        }

        private void UpdatePhysics()
        {
            bool isFalling = true;

            velocity += new Vector2f(0, 0.15f);

            Vector2f nextPos = Position + velocity - rect.Origin;
            FloatRect playerRect = new FloatRect(nextPos, rect.Size);

            int physicsX = (int)((Position.X - rect.Origin.X + rect.Size.X * 0.5f) / Tile.TILE_SIZE);
            int physicsY = (int)((Position.Y + rect.Size.Y) / Tile.TILE_SIZE);
            Tile tile = world.GetTile(physicsX, physicsY);

            if(tile != null)
            {
                
                FloatRect tileRect = new FloatRect(tile.Position, new Vector2f(Tile.TILE_SIZE, Tile.TILE_SIZE));

                DebugRender.AddRectangle(tileRect, Color.Red);
                // Collision
                isFalling = !playerRect.Intersects(tileRect);
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
                        movement.X = ((tileRect.Left + tileRect.Width) - playerRect.Left);
                        velocity.X = 0;
                    }
                    // Right walls
                    else if (offset.X < 0)
                    {
                        movement.X = (tileRect.Left - (playerRect.Left + playerRect.Width));
                        velocity.X = 0;
                    }

                }
            }
        }
    }
}
