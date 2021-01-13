using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;

namespace RealtimeInteractiveCrawler
{
    class Player : Npc
    {
        public const float PLAYER_MOVE_SPEED = 4f;
        public const float PLAYER_MOVE_SPEED_ACCELERATION = 0.2f;


        private float positionX;
        private float positionY;

        AnimSprite animSprite;

        private SpriteSheet spriteSheet;
        private int currSpriteX;

        public Player(World world) : base(world)
        {
            isRectVisible = false;
            useGravity = false;

            spriteSheet = new SpriteSheet(9, 4, 0, (int)Content.TexPlayer.Size.X, (int)Content.TexPlayer.Size.Y);
            rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth, spriteSheet.SubWidth));
            rect.Origin = new Vector2f(rect.Size.X * 0.5f, 0);
            

            animSprite = new AnimSprite(Content.TexPlayer, spriteSheet);
            // TODO Color
            animSprite.color = Color.White; 

            // Idle Anim           
            AssignAnimations("idle", 2, 1);
            AssignAnimations("goHorizontal", 1, 9);
            AssignAnimations("goUp", 0, 9);
            AssignAnimations("goDown", 2, 9);

        }

        // TODO
        public void AssignAnimations(string animName, int spriteType, int animAmount, float time = 0.1f)
        {
            AnimationFrame[] animFrame = new AnimationFrame[animAmount];
            for (int i = 0; i < animAmount; i++)
            {
                animFrame[i] = new AnimationFrame(i, spriteType, time);
            }

            animSprite.AddAnimation(animName, new Animation(animFrame));
        }

        public override void OnKill()
        {
            Spawn();
        }

        public override void OnWallCollided()
        {         
        }

        public override void UpdateNPC()
        {
            rect.TextureRect = spriteSheet.GetTextureRect(currSpriteX, 1);
            UpdateMovement();

            positionX = Position.X;
            positionY = Position.Y;
        }

        public override void DrawNPC(RenderTarget target, RenderStates states)
        {
            target.Draw(animSprite, states);
        }

        private void UpdateMovement()
        {
            bool movingUp = Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.Up);
            bool movingDown = Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down);
            bool movingLeft = Keyboard.IsKeyPressed(Keyboard.Key.A) || Keyboard.IsKeyPressed(Keyboard.Key.Left);
            bool movingRight = Keyboard.IsKeyPressed(Keyboard.Key.D) || Keyboard.IsKeyPressed(Keyboard.Key.Right);

            bool movingOnX = movingLeft || movingRight;
            bool movingOnY = movingUp || movingDown;
            if (movingOnY){
                if (movingDown)
                {
                    if (movement.Y < 0)
                        movement.Y = 0;

                    movement.Y += PLAYER_MOVE_SPEED_ACCELERATION;

                    // Animation
                    animSprite.Play("goDown");
                }
                else if (movingUp)
                {
                    if (movement.Y > 0)
                        movement.Y = 0;

                    movement.Y -= PLAYER_MOVE_SPEED_ACCELERATION;

                    // Animation
                    animSprite.Play("goUp");
                }
                if (movement.Y > PLAYER_MOVE_SPEED)
                    movement.Y = PLAYER_MOVE_SPEED;
                else if (movement.Y < -PLAYER_MOVE_SPEED)
                    movement.Y = -PLAYER_MOVE_SPEED;

            }
            else if (movingOnX)
            {
                if (movingLeft)
                {
                    if (movement.X > 0)
                        movement.X = 0;

                    movement.X -= PLAYER_MOVE_SPEED_ACCELERATION;
                    Direction = -1;
                }
                else if (movingRight)
                {
                    if (movement.X < 0)
                        movement.X = 0;

                    movement.X += PLAYER_MOVE_SPEED_ACCELERATION;
                    Direction = 1;
                }

                if (movement.X > PLAYER_MOVE_SPEED)
                    movement.X = PLAYER_MOVE_SPEED;
                else if (movement.X < -PLAYER_MOVE_SPEED)
                    movement.X = -PLAYER_MOVE_SPEED;

                // Animation
                animSprite.Play("goHorizontal");
            }
            else
            {
                movement = new Vector2f();

                // Animation
                animSprite.Play("idle");
            }
        }


    }
}
