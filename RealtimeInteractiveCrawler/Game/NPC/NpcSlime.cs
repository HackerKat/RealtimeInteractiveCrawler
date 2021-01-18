using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class NpcSlime : Npc
    {
        const float TIME_WAIT_JUMP = 1f;
        float waitTimer;

        public NpcSlime() : base()
        {
            SpriteSheet = Content.SpriteEnemy;

            // Size of rectangle
            Rect = new RectangleShape(new Vector2f(SpriteSheet.SubWidth * 0.8f, SpriteSheet.SubHeight * 0.8f));
            // Center of rectangle
            Rect.Origin = new Vector2f(Rect.Size.X * 0.5f, 0);
            Rect.FillColor = new Color(0, 255, 0, 127);

            Rect.Texture = SpriteSheet.Texture;
            Rect.TextureRect = SpriteSheet.GetTextureRect(0, 0);
        }

        public override void DrawNPC(RenderTarget target, RenderStates states)
        {
        }

        public override void OnKill()
        {
            Spawn();
        }

        public override void OnWallCollided()
        {
            Direction *= -1;
            velocity = new Vector2f(-velocity.X * 0.8f, velocity.Y);
        }

        public override void UpdateNPC()
        {

            if (waitTimer >= TIME_WAIT_JUMP)
            {
                velocity = new Vector2f(Direction * AwesomeGame.Rand.Next(1, 10), -AwesomeGame.Rand.Next(6, 9));
                waitTimer = 0f;
            }
            else
            {
                waitTimer += GameLoop.GameTime.deltaTime;
                velocity.X = 0f;
            }

            Rect.TextureRect = SpriteSheet.GetTextureRect(0, 0);

            Rect.TextureRect = SpriteSheet.GetTextureRect(0, 1);

        }
    }
}
