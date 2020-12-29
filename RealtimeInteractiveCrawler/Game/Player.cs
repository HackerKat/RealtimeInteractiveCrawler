using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RealtimeInteractiveCrawler
{
    public class Player : Entity
    {
        private float movementSpeed = 5f;
        private Sprite sprite;
        

        public Player(Sprite sprite) : base(sprite)
        {
            this.sprite = sprite;
        }

        public void Update(InputManager inputManager, GameTime gameTime)
        {
            if (inputManager.getKeyDown(Keyboard.Key.W) || inputManager.getKeyDown(Keyboard.Key.Up))
            {
                sprite.Position += new Vector2f(0f, -1f) * movementSpeed * gameTime.DeltaTime;
            }
            if (inputManager.getKeyDown(Keyboard.Key.A) || inputManager.getKeyDown(Keyboard.Key.Left))
            {
                sprite.Position += new Vector2f(-1f, 0f) * movementSpeed * gameTime.DeltaTime;
            }
            if (inputManager.getKeyDown(Keyboard.Key.S) || inputManager.getKeyDown(Keyboard.Key.Down))
            {
                sprite.Position += new Vector2f(0f, 1f) * movementSpeed * gameTime.DeltaTime;
            }
            if (inputManager.getKeyDown(Keyboard.Key.D) || inputManager.getKeyDown(Keyboard.Key.Right))
            {
                sprite.Position += new Vector2f(1f, 0f) * movementSpeed * gameTime.DeltaTime;
            }
        }
    }
}
