﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealtimeInteractiveCrawler.AnimSprite;

namespace RealtimeInteractiveCrawler
{
    abstract class Npc : Entity
    {      
        public int Health;
        public int Attack;
        public int Defense;


        public int Direction
        {
            set
            {
                int dir = value >= 0 ? 1 : -1;
                Scale = new Vector2f(dir, 1);
            }
            get
            {
                int dir = Scale.X >= 0 ? 1 : -1;
                return dir;
            }
        }

        public Npc() : base()
        {
        }



        public override void Update()
        {
            UpdateNPC();
            base.Update();

            Position += movement + velocity;

            // Fell down outside the window
            if (Position.Y > GameLoop.Window.Size.Y)
                OnKill();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            if (isRectVisible)
                target.Draw(rect, states);

            DrawNPC(target, states);
        }


        public void AssignAnimations(AnimSprite animSprite, MovementType animType, int spriteType, int animAmount, float time = 0.1f)
        {
            AnimationFrame[] animFrame = new AnimationFrame[animAmount];
            for (int i = 0; i < animAmount; i++)
            {
                animFrame[i] = new AnimationFrame(i, spriteType, time);
            }

            animSprite.AddAnimation(animType, new Animation(animFrame));
        }

        public abstract void OnKill();
        public abstract void UpdateNPC();
        public abstract void DrawNPC(RenderTarget target, RenderStates states);
    }
}
