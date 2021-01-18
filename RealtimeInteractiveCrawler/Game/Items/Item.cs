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
        public int id;

        public Item(ItemType type, SpriteSheet spriteSheet, int posSpriteX, int posSpriteY, int id, float size = 1) : base()
        {
            TypeItem = type;
            Rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth, spriteSheet.SubHeight))
            {
                Texture = spriteSheet.Texture,
                TextureRect = spriteSheet.GetTextureRect(posSpriteX, posSpriteY),
                
            };
            Rect.Size *= size;
            IsItem = true;
            this.id = id;
        }

        public override void Update()
        {

            base.Update();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            if (isRectVisible)
                target.Draw(Rect, states);
        }

        public override void OnWallCollided()
        {
        }
    }
}
