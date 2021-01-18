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
        public const int ATTACK = 5;

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

        public bool AllowAttack = true;

        public const float AGGRO_RADIUS = 100f;
        public const int SIZE = 24;
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
                    if (dist < smallestDist)
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
            int entityTileX = (int)Position.X / Tile.TILE_SIZE;
            int entityTileY = (int)Position.Y / Tile.TILE_SIZE;

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
                Vector2 pos = Position;
                Vector2 vel = Velocity;

                if (rect.IntersectsWith(tileRect))
                {
                    RectangleF intersection = RectangleF.Intersect(rect, tileRect);

                    //check least dominant intersection
                    if (intersection.Width < intersection.Height)
                    {
                        if (intersection.X > rect.X) // tile to the right
                        {
                            pos.X -= intersection.Width;
                            vel.X = 0;

                            rect.X -= intersection.Width;
                        }
                        else // tile to the left
                        {
                            pos.X += intersection.Width;
                            vel.X = 0;

                            rect.X += intersection.Width;
                        }
                    }
                    else
                    {
                        if (intersection.Y > rect.Y) // tile to the bottom
                        {
                            pos.Y -= intersection.Height;
                            vel.Y = 0;

                            rect.Y -= intersection.Height;
                        }
                        else // tile to the top
                        {
                            pos.Y += intersection.Height;
                            vel.Y = 0;

                            rect.Y += intersection.Height;
                        }
                    }

                    //OnWallCollided();
                }

                Position = pos;
                Velocity = vel;
            }
        }


        public double Distance(Vector2 posA, Vector2 posB)
        {
            float dX = posA.X - posB.X;
            float dY = posA.Y - posB.Y;
            double distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
            return distance;
        }

        public void Attack(Player target)
        {
            if (AllowAttack && Distance(Position, target.Position) < 16)
            {
                AllowAttack = false;
                target.Health -= ATTACK;
                //Console.WriteLine(Id + " ENEMY ATTACKED");
                Wait();
            }
        }

        public async void Wait()
        {
            await GameLoop.Wait(5000);
            AllowAttack = true;
        }
    }
}
