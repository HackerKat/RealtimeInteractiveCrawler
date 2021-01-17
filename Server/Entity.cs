using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

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
       // protected World world;

        public Entity(int x, int y, int id)
        {
            Position = new Vector2(x, y);
            this.Id = id;

            //world = AwesomeGame.world;
        }

        public void Update(GameTime gameTime)
        {
            SteeringOutput steering = Behaviour.GetSteering();
            UpdatePos(steering, gameTime);
        }

        public void UpdatePos(SteeringOutput steering, GameTime gameTime)
        {
            Position = new Vector2(Position.X + (steering.velocity.X * gameTime.deltaTime),
                                   Position.Y + (steering.velocity.Y * gameTime.deltaTime)); 
            Orientation += steering.rotation * gameTime.deltaTime; 
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

        //private void UpdatePhysicsWall(FloatRect playerRect, int pX, int pY)
        //{
        //    Tile[] walls = new Tile[]
        //    {
        //        // Left
        //        world.GetTile(pX - 1, pY),
        //        world.GetTile(pX - 1, pY - 1),
        //        world.GetTile(pX - 1, pY + 1),
        //        // Right
        //        world.GetTile(pX + 1, pY),
        //        world.GetTile(pX + 1, pY - 1),
        //        world.GetTile(pX + 1, pY + 1),
        //        // Top
        //        world.GetTile(pX, pY - 1),
        //        world.GetTile(pX - 1, pY - 1),
        //        world.GetTile(pX + 1, pY - 1),
        //        // Down
        //        world.GetTile(pX, pY),
        //        world.GetTile(pX - 1, pY),
        //        world.GetTile(pX + 1, pY),

        //    };

        //    foreach (var tile in walls)
        //    {
        //        if (tile == null || tile.type == TileType.GROUND) continue;

        //        FloatRect tileRect = new FloatRect(tile.Position, new Vector2f(Tile.TILE_SIZE, Tile.TILE_SIZE));
        //        DebugRender.AddRectangle(tileRect, Color.Yellow);

        //        if (playerRect.Intersects(tileRect))
        //        {
        //            Vector2f offset = new Vector2f(playerRect.Left - tileRect.Left, playerRect.Top - tileRect.Top);
        //            offset.X /= Math.Abs(offset.X);
        //            offset.Y /= Math.Abs(offset.Y);

        //            float speed = Math.Abs(movement.X);

        //            // Left walls
        //            if (offset.X > 0)
        //            {
        //                // Sends the player one tile away
        //                Position = new Vector2f((tileRect.Left + tileRect.Width) + playerRect.Width * 0.5f, Position.Y);
        //                movement.X = 0;
        //                velocity.X = 0;
        //            }
        //            // Right walls
        //            else if (offset.X < 0)
        //            {
        //                Position = new Vector2f(tileRect.Left - playerRect.Width * 0.5f, Position.Y);
        //                movement.X = 0;
        //                velocity.X = 0;
        //                //Position = new Vector2f(Position.X - 2, Position.Y);
        //            }
        //            // Top walls
        //            else if (offset.Y > 0)
        //            {
        //                Position = new Vector2f(Position.X, (tileRect.Top + tileRect.Height) + playerRect.Height * 0.5f);
        //                movement.Y = 0;
        //                velocity.Y = 0;
        //                //Position = new Vector2f(Position.X, Position.Y + 2);
        //            }
        //            // Down walls
        //            else if (offset.Y < 0)
        //            {
        //                Position = new Vector2f(Position.X, tileRect.Top - playerRect.Height * 0.5f);
        //                movement.Y = 0;
        //                velocity.Y = 0;
        //                //Position = new Vector2f(Position.X, Position.Y - 2);
        //            }

        //            OnWallCollided();

        //        }
        //    }
        //}
    }
}
