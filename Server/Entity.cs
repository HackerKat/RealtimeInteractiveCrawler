using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace Server
{
    public class Entity
    {
        public Vector2 Velocity
        {
            get;
            set;
        } = new Vector2();

        public float Orientation
        {
            get;
            set;
        } = 0;

        public float Rotation
        {
            get;
            set;
        } = 0;

        public Vector2 Position
        {
            get;
            set;
        } = new Vector2();

        public const float MAX_SPEED = 20f;
        public const float MAX_ROTATION = 7f;
        public Vector2 OrientVector
        {
            get
            {
                return new Vector2((float)Math.Cos(Orientation), (float)Math.Sin(Orientation));
            }
        }

        public SteeringBehaviour Behaviour
        {
            get;
            set;
        } = new NullBehaviour();

        public int Id
        {
            get;
            private set;
        }

        public const float AGGRO_RADIUS = 100f;
        public const int SIZE = 54;
        private World world;
        private Rectangle rect;

        public Entity(int x, int y, int id)
        {
            Position = new Vector2(x, y);
            this.Id = id;

            world = Server.world;

            rect = new Rectangle(x, y, SIZE, SIZE);
        }

        public void Update(GameTime gameTime)
        {
            SteeringOutput steering = Behaviour.GetSteering();
            UpdatePos(steering, gameTime);
        }

        public void UpdatePos(SteeringOutput steering, GameTime gameTime)
        {
            UpdatePhysicsWall();
            Position = new Vector2(Position.X + (steering.velocity.X * gameTime.deltaTime),
                                   Position.Y + (steering.velocity.Y * gameTime.deltaTime));
            Orientation += steering.rotation * gameTime.deltaTime;

            rect = new Rectangle((int)Position.X, (int)Position.Y, SIZE, SIZE);
        }

        public float getNewOrientation(float currOrientation, Vector2 velocity)
        {
            if (velocity.Length() > 0)
            {
                return (float)Math.Atan2(-velocity.X, velocity.Y);
            }
            return currOrientation;
        }

        public bool CheckIfSeekPlayer(Player target)
        {
            return (Position - target.Position).Length() <= AGGRO_RADIUS;
        }

        private void UpdatePhysicsWall()
        {
            int entityTileX = (int)((Position.X - SIZE * 0.5f + SIZE * 0.5f) / Tile.TILE_SIZE);
            int entityTileY = (int)((Position.Y + SIZE * 0.5f) / Tile.TILE_SIZE);

            //int entityTileX = (int)(Position.X - rect. / Tile.TILE_SIZE);
            //int entityTileY = (int)(Position.Y / Tile.TILE_SIZE);

            Tile[] walls = new Tile[]
            {
                // Left
                Server.world.GetTile(entityTileX - 1, entityTileY),
                Server.world.GetTile(entityTileX - 1, entityTileY - 1),
                Server.world.GetTile(entityTileX - 1, entityTileY + 1),
                // Right
                Server.world.GetTile(entityTileX + 1, entityTileY),
                Server.world.GetTile(entityTileX + 1, entityTileY - 1),
                Server.world.GetTile(entityTileX + 1, entityTileY + 1),
                // Top
                Server.world.GetTile(entityTileX, entityTileY - 1),
                Server.world.GetTile(entityTileX - 1, entityTileY - 1),
                Server.world.GetTile(entityTileX + 1, entityTileY - 1),
                // Down
                Server.world.GetTile(entityTileX, entityTileY + 1),
                Server.world.GetTile(entityTileX - 1, entityTileY + 1),
                Server.world.GetTile(entityTileX + 1, entityTileY + 1),
            };

            foreach (var tile in walls)
            {
                if (tile == null || tile.type == TileType.GROUND) continue;

                Rectangle tileRect = new Rectangle((int)tile.Position.X, (int)tile.Position.Y, Tile.TILE_SIZE, Tile.TILE_SIZE);

                if (rect.IntersectsWith(tileRect))
                {
                    Vector2 offset = new Vector2(rect.Left - tileRect.Left, rect.Top - tileRect.Top);
                    offset.X /= Math.Abs(offset.X);
                    offset.Y /= Math.Abs(offset.Y);

                    //float speed = Math.Abs(movement.X);

                    // Left walls
                    if (offset.X > 0)
                    {
                        // Sends the player one tile away
                        Position = new Vector2(tileRect.Left + rect.Width * 0.25f * 0.5f, Position.Y);
                        Velocity = new Vector2(0, Velocity.Y);
                    }
                    // Right walls
                    else if (offset.X < 0)
                    {
                        Position = new Vector2(tileRect.Left - rect.Width * 0.25f * 0.5f, Position.Y);
                        Velocity = new Vector2(0, Velocity.Y);
                        //Position = new Vector2f(Position.X - 2, Position.Y);
                    }
                    //// Top walls
                    else if (offset.Y > 0)
                    {
                        Position = new Vector2(Position.X, (tileRect.Top + tileRect.Height) - rect.Height * 0.25f * 0.5f);
                        Velocity = new Vector2(Velocity.X, 0);
                        //Position = new Vector2f(Position.X, Position.Y + 2);
                    }
                    // Down walls
                    else if (offset.Y < 0)
                    {
                        Position = new Vector2(Position.X, tileRect.Top + rect.Height * 0.25f);
                        Velocity = new Vector2(Velocity.X, 0);
                        //Position = new Vector2f(Position.X, Position.Y - 2);
                    }

                    //OnWallCollided();

                }
            }
        }
    }
}
