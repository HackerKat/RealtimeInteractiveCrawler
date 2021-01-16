using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealtimeInteractiveCrawler.AnimSprite;

namespace RealtimeInteractiveCrawler
{
    class NetworkPlayer : Transformable, Drawable
    {
        AnimSprite animSprite;

        private SpriteSheet spriteSheet;
        private RectangleShape rect;

        public NetworkPlayer(int x, int y)
        {
            spriteSheet = new SpriteSheet(9, 4, true, 0, Content.TexPlayer);
            //animSprite = new AnimSprite(Content.TexPlayer, spriteSheet);
            
           
            float size = 1;
            rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth * size, spriteSheet.SubHeight * size));
            // Center of rectangle
            rect.Origin = new Vector2f(spriteSheet.SubWidth * size * 0.5f, spriteSheet.SubWidth * size * 0.5f);
            rect.FillColor = Color.Blue;
            // Idle Anim         
            /*
            AssignAnimations(animSprite, MovementType.Idle, 2, 1);
            AssignAnimations(animSprite, MovementType.Horizontal, 1, 9);
            AssignAnimations(animSprite, MovementType.Up, 0, 9);
            AssignAnimations(animSprite, MovementType.Down, 2, 9);
            */

            Position = new Vector2f(x, y);
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            target.Draw(rect, states);
        }

        public void UpdatePos(int x, int y)
        {
            Position = new Vector2f(x, y);
        }
    }
}
