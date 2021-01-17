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

        public bool ClientPlayer;

        //public static UIInventory Inventory;

        private float positionX;
        private float positionY;

        private AnimSprite animSprite;
        private SpriteSheet spriteSheet;
        private MovementType lastAnim;

        public Player() : base()
        {
            isRectVisible = true;

            spriteSheet = Content.SpritePlayer;
            animSprite = new AnimSprite(spriteSheet);
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
            if (!ClientPlayer) return;

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
                        Side side;
                        side = Direction == 1 ? Side.RIGHT : Side.LEFT;
                        CheckForItems(side);
                        break;
                    case MovementType.Up:
                        CheckForItems(Side.UP);
                        break;
                    case MovementType.Down:
                    case MovementType.Idle:
                        CheckForItems(Side.DOWN);
                        break;
                    default:
                        break;
                }
            }

            // Mouse
            //if (UIManager.Over == null && UIManager.Drag == null)
            //{
            
            Vector2i mousePos = Mouse.GetPosition(GameLoop.Window);
            mousePos = (Vector2i)GameLoop.Window.MapPixelToCoords(mousePos);

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
                    Debug.WriteLine(tile.Position);
                    int boderMax = World.WORLD_SIZE * Chunk.CHUNK_SIZE * Tile.TILE_SIZE - Tile.TILE_SIZE;
                    bool noBorders = tile.Position.X != 0 && tile.Position.Y != 0 && tile.Position.X != boderMax && tile.Position.Y != boderMax;
                    if (tile.type != TileType.ITEM && tile.type != TileType.GROUND && noBorders)
                        world.SetTile(TileType.GROUND, i, j);
                }
            }
            //}

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
                    tilesTowardsPlayer.Add(world.GetTile(pX, pY));
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY));
                    tilesTowardsPlayer.Add(world.GetTile(pX + 1, pY));
                    break;
                case Side.LEFT:
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY));
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY - 1));
                    tilesTowardsPlayer.Add(world.GetTile(pX - 1, pY + 1));
                    break;
                default:
                    break;
            }

            Tile foundItem = tilesTowardsPlayer.Find(item => item.type == TileType.ITEM);
            if (foundItem != null)
            {

                World.Items.TryGetValue(foundItem, out Item actualItem);
                if (actualItem == null) return;
                Debug.WriteLine("huray, item! " + actualItem.TypeItem.ToString());
                switch (actualItem.TypeItem)
                {
                    case Item.ItemType.HEALTH:
                        ChangeHealth(10);
                        break;
                    case Item.ItemType.ATTACK:
                        ChangeAttack(2);
                        break;
                    case Item.ItemType.DEFENSE:
                        ChangeDefense(5);
                        break;
                    case Item.ItemType.ERASER:
                        break;
                    default:
                        break;
                }
                actualItem.IsDestroyed = true;
                foundItem.type = TileType.GROUND;
            }
        }

        public void ChangeHealth(int health)
        {
            Health += health;
            Debug.WriteLine(Health + " my health");
        }
        public void ChangeAttack(int attack)
        {
            Attack += attack;
            Debug.WriteLine(Attack + " my attack");
        }
        public void ChangeDefense(int defense)
        {
            Defense += defense;
            Debug.WriteLine(Defense + " my defense");
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

        // For Network players
        public void UpdatePos(int x, int y)
        {
            // move right
            if (Position.X < x)
            {
                Direction = 1;
                animSprite.Play(MovementType.Horizontal);
            }
            // move left
            else if (Position.X > x)
            {
                Direction = -1;
                animSprite.Play(MovementType.Horizontal);
            }
            // move down
            else if (Position.Y < y)
            {
                animSprite.Play(MovementType.Down);
            }
            // move up
            else if (Position.Y > y)
            {
                animSprite.Play(MovementType.Up);
            }

            Position = new Vector2f(x, y);
        }

    }
}
