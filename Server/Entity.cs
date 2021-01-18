using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Diagnostics;

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

        public const float MAX_SPEED = 50f;
        public const float MIN_SPEED = 20f;
        public const float MAX_ROTATION = 7f;
        public const int ENEMY_LIFE = 100;

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
        public int Health
        {
            get;
            set;
        } = ENEMY_LIFE;

        public bool HasRecentlyDied
        {
            get;
            set;
        } = false;

        public const float AGGRO_RADIUS = 100f;
        public const int SIZE = 54;
        private World world;
        private RectangleF rect;
        public Chunk chunk;

        public Entity(float x, float y, int id, Chunk chunk)
        {
            Position = new Vector2(x, y);
            this.Id = id;
            this.chunk = chunk;

            world = Server.world;
            rect = new RectangleF(x, y, SIZE, SIZE);
        }

        public void Update(GameTime gameTime)
        {
            SteeringOutput steering = Behaviour.GetSteering();
            UpdatePos(steering, gameTime);
            GetClosestChunk();
        }

        public void UpdatePos(SteeringOutput steering, GameTime gameTime)
        {
            Position = new Vector2(Position.X + (steering.velocity.X * gameTime.deltaTime),
                                   Position.Y + (steering.velocity.Y * gameTime.deltaTime));
            Orientation += steering.rotation * gameTime.deltaTime;

            rect = new RectangleF(Position.X, Position.Y, SIZE, SIZE);
            UpdatePhysicsWall();
        }

        public void GetClosestChunk()
        {
            Chunk closestChunk = null;
            double smallestDist = Double.MaxValue;
            for (int i = 0; i < World.WORLD_SIZE; i++)
            {
                for (int j = 0; j < World.WORLD_SIZE; j++)
                {
                    double dist = Distance(Position, Server.world.chunks[i][j].Origin);
                    if(dist < smallestDist)
                    {
                        smallestDist = dist;
                        closestChunk = Server.world.chunks[i][j];
                    }
                }
            }
            chunk = closestChunk;
            
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

                RectangleF tileRect = new RectangleF(tile.Position.X, tile.Position.Y, Tile.TILE_SIZE, Tile.TILE_SIZE);

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
                        Position = new Vector2(Position.X + offset.X, Position.Y);
                        Velocity = new Vector2(0, Velocity.Y);
                    }
                    // Right walls
                    else if (offset.X < 0)
                    {
                        Position = new Vector2(Position.X - offset.X, Position.Y);
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

        public double Distance(Vector2 posA, Vector2 posB)
        {
            float dX = posA.X - posB.X;
            float dY = posA.Y - posB.Y;
            double distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            return distance;
        }
    }
}
