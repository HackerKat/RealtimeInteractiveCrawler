using SFML.Graphics;
using SFML.System;
using System;

namespace RealtimeInteractiveCrawler
{
    class Item : Entity
    {
        public enum ItemType
        {
            NONE,
            HEALTH,
            ATTACK,
            DEFENSE,
            ERASER
        }

        public ItemType TypeItem;

        public Item(ItemType type, SpriteSheet spriteSheet, int posSpriteX, int posSpriteY, float size = 1) : base()
        {
            TypeItem = type;
            rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth, spriteSheet.SubHeight))
            {
                Texture = spriteSheet.Texture,
                TextureRect = spriteSheet.GetTextureRect(posSpriteX, posSpriteY),
                
            };
            rect.Size *= size;
            IsItem = true;
        }

        public override void Update()
        {

            base.Update();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            if (isRectVisible)
                target.Draw(rect, states);
        }

        public override void OnWallCollided()
        {
        }
    }
}
