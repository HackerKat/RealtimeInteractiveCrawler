using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static RealtimeInteractiveCrawler.AnimSprite;

namespace RealtimeInteractiveCrawler
{
    public enum Side
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    class Player : Npc
    {
        public const float PLAYER_MOVE_SPEED = 4f;
        public const float PLAYER_MOVE_SPEED_ACCELERATION = 0.2f;

        private float positionX;
        private float positionY;

        private AnimSprite animSprite;
        private SpriteSheet spriteSheet;
        private MovementType lastAnim;

        public Player() : base()
        {
            isRectVisible = true;

            spriteSheet = new SpriteSheet(9, 4, true, 0, Content.TexPlayer);
            animSprite = new AnimSprite(Content.TexPlayer, spriteSheet);
            //animSprite.color = Color.Red;
            rect = animSprite.RectShape;
            rect = animSprite.RectShape;
            //rect = new RectangleShape(new Vector2f(spriteSheet.SubWidth * size, spriteSheet.SubHeight * size));
            // Center of rectangle
            //rect.Origin = new Vector2f(spriteSheet.SubWidth * size * 0.5f, spriteSheet.SubWidth * size * 0.5f);

            AssignAnimations(animSprite, MovementType.Idle, 2, 1);
            AssignAnimations(animSprite, MovementType.Horizontal, 1, 9);
            AssignAnimations(animSprite, MovementType.Up, 0, 9);
            AssignAnimations(animSprite, MovementType.Down, 2, 9);
            animSprite.Play(MovementType.Idle);
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
            UpdateInteractions();


            // For server
            positionX = Position.X;
            positionY = Position.Y;
        }

        private void UpdateInteractions()
        {
            // Space
            bool space = Keyboard.IsKeyPressed(Keyboard.Key.Space);
            if (space)
            {
                switch (lastAnim)
                {
                    case MovementType.Horizontal:
                        Debug.WriteLine("Horizontal");
                        Side side;
                        side = Direction == 1 ? Side.RIGHT : Side.LEFT;
                        CheckForItems(side);
                        break;
                    case MovementType.Up:
                        Debug.WriteLine("UP");
                        CheckForItems(Side.UP);
                        break;
                    case MovementType.Down:
                    case MovementType.Idle:
                        Debug.WriteLine("Down");
                        CheckForItems(Side.DOWN);
                        break;
                    default:
                        break;
                }
            }

            // Mouse
            Vector2i mousePos = Mouse.GetPosition(GameLoop.Window);
            Tile tile = world.GetTile(mousePos.X / Tile.TILE_SIZE, mousePos.Y / Tile.TILE_SIZE);
            if (tile != null)
            {
                FloatRect tileRect = tile.GetFloatRect();
                DebugRender.AddRectangle(tileRect, Color.Green);
                // Left Mouse to Erase
                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    int i = (mousePos.X) / Tile.TILE_SIZE;
                    int j = (mousePos.Y) / Tile.TILE_SIZE;
                    if (world.GetTile(i, j).type != TileType.ITEM)
                        world.SetTile(TileType.GROUND, i, j);
                }
            }
        }

        private void CheckForItems(Side side)
        {
            int pX = (int)((Position.X - rect.Origin.X + rect.Size.X * 0.5f) / Tile.TILE_SIZE);
            int pY = (int)((Position.Y + rect.Size.Y * 0.5f) / Tile.TILE_SIZE);
            List<Tile> tilesTowardsPlayer = new List<Tile>();
            switch (side)
            {
                case Side.UP:
                    tilesTowardsPlayer.Add(world.GetTile(pX, pY - 2));
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY - 2));
                    tilesTowardsPlayer.Add(world.GetTile(pX + 1, pY - 2));
                    tilesTowardsPlayer.Add(world.GetTile(pX, pY - 1));
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY - 1));
                    tilesTowardsPlayer.Add(world.GetTile(pX + 1, pY - 1));
                    break;
                case Side.RIGHT:
                    tilesTowardsPlayer.Add(world.GetTile(pX + 1, pY));
                    tilesTowardsPlayer.Add(world.GetTile(pX + 1, pY - 1));
                    tilesTowardsPlayer.Add(world.GetTile(pX + 1, pY + 1));
                    break;
                case Side.DOWN:
                    tilesTowardsPlayer.Add(world.GetTile(pX, pY + 1));
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY + 1));
                    tilesTowardsPlayer.Add(world.GetTile(pX + 1, pY + 1));
                    break;
                case Side.LEFT:
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY));
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY - 1));
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY + 1));
                    break;
                default:
                    break;
            }

            if (tilesTowardsPlayer.Exists(item => item.type == TileType.ITEM))
            {
                Debug.WriteLine("huray, item!");
            }
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

            if (movingOnY)
            {
                if (movingDown)
                {
                    if (movement.Y < 0)
                        movement.Y = 0;

                    movement.Y += PLAYER_MOVE_SPEED_ACCELERATION;

                    // Animation
                    animSprite.Play(MovementType.Down);
                    lastAnim = MovementType.Down;
                }
                else if (movingUp)
                {
                    if (movement.Y > 0)
                        movement.Y = 0;

                    movement.Y -= PLAYER_MOVE_SPEED_ACCELERATION;

                    // Animation
                    animSprite.Play(MovementType.Up);
                    lastAnim = MovementType.Up;
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
                lastAnim = MovementType.Horizontal;
            }
            // Standing / Idle
            else
            {
                movement = new Vector2f();

                // Animation
                animSprite.animations[lastAnim].Reset();
                //animSprite.Play(MovementType.Idle);
            }
        }


    }
}
