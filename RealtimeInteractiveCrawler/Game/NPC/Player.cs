﻿using SFML.Graphics;
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

        private AnimSprite animSprite;
        private SpriteSheet spriteSheet;
        private MovementType lastAnim;

        public Player() : base()
        {
            isRectVisible = true;

            spriteSheet = new SpriteSheet(9, 4, 0, (int)Content.TexPlayer.Size.X, (int)Content.TexPlayer.Size.Y);
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
            Vector2i mousePos = Mouse.GetPosition(GameLoop.Window);
            //Debug.WriteLine(mousePos.X + " mouse x");
            Tile tile = world.GetTile(mousePos.X, mousePos.Y);
            //Chunk chunkChunk = world.GetChunk(mousePos.X, mousePos.Y);
            //if (chunkChunk != null)
            //    Debug.WriteLine(chunkChunk.chunkPos.X + " mopuse pos chunk");
            if (tile != null)
            {
                Debug.WriteLine(tile.Position.X + " tile x");
                Vector2f mouseMan = new Vector2f(mousePos.X, mousePos.Y);
                FloatRect tileRect = tile.GetFloatRect();
                DebugRender.AddRectangle(tileRect, Color.Green);
                // Debug
                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    int i = (int)(mousePos.X / Tile.TILE_SIZE);
                    int j = (int)(mousePos.Y / Tile.TILE_SIZE);
                    world.SetTile(TileType.GROUND, i, j);
                }
            }

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