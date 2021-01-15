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

        SpriteSheet spriteSheet;
        float waitTimer;

        public NpcSlime() : base()
        {
            spriteSheet = new SpriteSheet(1, 2, 0, (int)Content.TexPlay1.Size.X, (int)Content.TexPlay1.Size.Y);

            // Size of rectangle
            rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth * 0.8f, spriteSheet.SubHeight * 0.8f));
            // Center of rectangle
            rect.Origin = new Vector2f(rect.Size.X * 0.5f, 0);
            rect.FillColor = new Color(0, 255, 0, 127);

            rect.Texture = Content.TexPlay1;
            rect.TextureRect = spriteSheet.GetTextureRect(0, 0);
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

            rect.TextureRect = spriteSheet.GetTextureRect(0, 0);

            rect.TextureRect = spriteSheet.GetTextureRect(0, 1);

        }
    }
}
