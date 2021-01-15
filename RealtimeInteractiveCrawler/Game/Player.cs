using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;
using static RealtimeInteractiveCrawler.AnimSprite;

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

        public Player(World world) : base(world)
        {
            isRectVisible = true;
            useGravity = false;

            spriteSheet = new SpriteSheet(9, 4, 0, (int)Content.TexPlayer.Size.X, (int)Content.TexPlayer.Size.Y);
            animSprite = new AnimSprite(Content.TexPlayer, spriteSheet);
            animSprite.color = Color.Red;
            rect = animSprite.RectShape;
            rect = animSprite.RectShape;
            //rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth * size, spriteSheet.SubHeight * size));
            // Center of rectangle
            //rect.Origin = new Vector2f(spriteSheet.SubWidth * size * 0.5f, spriteSheet.SubWidth * size * 0.5f);
       
            AssignAnimations(animSprite, MovementType.Idle, 2, 1);
            AssignAnimations(animSprite, MovementType.Horizontal, 1, 9);
            AssignAnimations(animSprite, MovementType.Up, 0, 9);
            AssignAnimations(animSprite, MovementType.Down, 2, 9);

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
            UpdateMovement();

            // For server
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
                    animSprite.Play(MovementType.Down);
                }
                else if (movingUp)
                {
                    if (movement.Y > 0)
                        movement.Y = 0;

                    movement.Y -= PLAYER_MOVE_SPEED_ACCELERATION;

                    // Animation
                    animSprite.Play(MovementType.Up);
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
                animSprite.Play(MovementType.Horizontal);
            }
            else
            {
                movement = new Vector2f();

                // Animation
                animSprite.Play(MovementType.Idle);
            }
        }


    }
}
