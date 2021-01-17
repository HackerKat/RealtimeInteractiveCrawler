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

        public Entity(int x, int y, int id)
        {
            Position = new Vector2(x, y);
            this.Id = id;
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
    }
}
