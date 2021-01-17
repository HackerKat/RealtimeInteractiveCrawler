﻿using SFML.Graphics;
using SFML.System;

namespace RealtimeInteractiveCrawler
{
    class Enemy : Npc
    {
        public Chunk Chunk;
        public Enemy() : base()
        {
            SpriteSheet = Content.SpriteEnemy;

            // Size of rectangle
            rect = new RectangleShape(new Vector2f(SpriteSheet.SubWidth * 0.8f, SpriteSheet.SubHeight * 0.8f));
            // Center of rectangle
            rect.Origin = new Vector2f(rect.Size.X * 0.5f, 0);

            rect.Texture = SpriteSheet.Texture;
            rect.TextureRect = SpriteSheet.GetTextureRect(0, 0);
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