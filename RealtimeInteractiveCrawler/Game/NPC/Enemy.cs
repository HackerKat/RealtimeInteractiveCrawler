using SFML.Graphics;
using SFML.System;

namespace RealtimeInteractiveCrawler
{
    class Enemy : Npc
    {
        public const int ENEMY_LIFE = 100;
        public Chunk Chunk;
        public int id;
        public Enemy() : base()
        {
            Health = ENEMY_LIFE;
            SpriteSheet = Content.SpriteEnemy;

            // Size of rectangle
            Rect = new RectangleShape(new Vector2f(SpriteSheet.SubWidth * 0.8f, SpriteSheet.SubHeight * 0.8f));
            // Center of rectangle
            Rect.Origin = new Vector2f(Rect.Size.X * 0.5f, 0);

            Rect.Texture = SpriteSheet.Texture;
            Rect.TextureRect = SpriteSheet.GetTextureRect(0, 0);
        }
        public override void DrawNPC(RenderTarget target, RenderStates states)
        {
            
        }

        public override void OnKill()
        {
            
        }

        public override void OnWallCollided()
        {
            
        }

        public override void UpdateNPC()
        {
            
        }

    }
}