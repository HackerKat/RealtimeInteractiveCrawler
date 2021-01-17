using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace RealtimeInteractiveCrawler
{
    public class Enemy : Npc
    {
        public Enemy () : base()
        {
            spriteSheet = new SpriteSheet(9, 4, 0, (int)Content.TexPlay2.Size.X, (int)Content.TexPlay2.Size.Y);
            float size = 1;
            rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth * size, spriteSheet.SubHeight * size));
            rect.Origin = new Vector2f(spriteSheet.SubWidth * size * 0.5f, spriteSheet.SubWidth * size * 0.5f);
            Position = new Vector2f(x, y);
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(rect, states);
        }

        public void UpdatePos(SteeringOutput steering, GameTime gameTime)
        {
            Position = new Vector2f(Position.X + (steering.velocity.X * gameTime.deltaTime),
                                   Position.Y + (steering.velocity.Y * gameTime.deltaTime)); //velocity * gameTime + 0.5f * steering.angular ?!!! gameTime * gameTime

            orientation += steering.rotation * gameTime.deltaTime; //rotation * gameTime.deltaTime + 0.5f * steering.angular??!! gameTime.deltaTime * gameTime.deltaTime
            
        }

        public float getNewOrientation(float currOrientation, Vector2f velocity)
        {
            if(Length(velocity) > 0)
            {
                return (float)Math.Atan2(-velocity.X, velocity.Y);
            }
            return currOrientation;
        }

        public void getSteering(int targetPosX, int targetPosY)
        {
            velocity.X = targetPosX - Position.X;
            velocity.Y = targetPosY - Position.Y;

            velocity = normalize(velocity);
            velocity *= MAX_SPEED;

            orientation = getNewOrientation(orientation, velocity);

            rotation = 0;
        }

        public static Vector2f normalize(Vector2f source)
        {
            float length = Length(source);
            if (length != 0)
                return new Vector2f(source.X / length, source.Y / length);
            else
                return source;
        }

        public static float Length(Vector2f source)
        {
            return (float)Math.Sqrt((source.X * source.X) + (source.Y * source.Y));
        }
}
}
