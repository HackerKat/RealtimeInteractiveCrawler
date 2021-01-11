using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace RealtimeInteractiveCrawler
{
    class Player : Npc
    {
        public const float PLAYER_MOVE_SPEED = 4f;
        public const float PLAYER_MOVE_SPEED_ACCELERATION = 0.2f;


        private float positionX;
        private float positionY;



        public Player(World world) : base(world)
        {
            rect = new RectangleShape(new Vector2f(Tile.TILE_SIZE * 1.5f, Tile.TILE_SIZE * 2.8f));
            rect.Origin = new Vector2f(rect.Size.X * 0.5f, 0);

            rectDirection = new RectangleShape(new Vector2f(50, 3));
            rectDirection.FillColor = Color.Red;
            rectDirection.Position = new Vector2f(0, rect.Size.Y * 0.5f - 1);
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

            positionX = Position.X;
            positionY = Position.Y;
        }

        public override void DrawNPC(RenderTarget target, RenderStates states)
        {
            target.Draw(rectDirection, states);
        }

        private void UpdateMovement()
        {
            bool movingUp = Keyboard.IsKeyPressed(Keyboard.Key.W) || Keyboard.IsKeyPressed(Keyboard.Key.Up);
            bool movingDown = Keyboard.IsKeyPressed(Keyboard.Key.S) || Keyboard.IsKeyPressed(Keyboard.Key.Down);
            bool movingLeft = Keyboard.IsKeyPressed(Keyboard.Key.A) || Keyboard.IsKeyPressed(Keyboard.Key.Left);
            bool movingRight = Keyboard.IsKeyPressed(Keyboard.Key.D) || Keyboard.IsKeyPressed(Keyboard.Key.Right);

            bool movingOnX = movingLeft || movingRight;

            if (movingOnX)
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
            }
            else
            {
                movement = new Vector2f();
            }
        }


    }
}
