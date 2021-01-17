using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RealtimeInteractiveCrawler
{
    public abstract class GameLoop
    {
        public const int FPS = 60;
        public const float TIME_TILL_UPDATE = 1f / FPS;
        public bool hasFocus = true;

        public static RenderWindow Window
        {
            get;
            protected set;
        }

        public static GameTime GameTime
        {
            get;
            protected set;
        }

        public Color WindowClearColor
        {
            get;
            protected set;
        }

        public View GameView
        {
            get;
            protected set;
        }

        protected GameLoop(uint width, uint height, string title, Color windowColor)
        {
            WindowClearColor = windowColor;
            Window = new RenderWindow(new VideoMode(width, height), title, Styles.Titlebar | Styles.Close);
            GameView = new View(new Vector2f(350f, 300f), new Vector2f(width, height));  // TODO set to player pos

            GameTime = new GameTime();
            Window.Closed += WindowClosed;
            Window.Resized += WindowResized;
            Window.GainedFocus += WindowFocus;
            Window.LostFocus += WindowUnfocus;
        }

        public void Run()
        {
            LoadContent();
            Initialize();

            float totalTimeBeforeUpdate = 0f;
            float previousTimeElapsed = 0f;
            float deltaTime = 0f;
            float totalTimeElapsed = 0f;

            Clock clock = new Clock();

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                totalTimeElapsed = clock.ElapsedTime.AsSeconds();
                deltaTime = totalTimeElapsed - previousTimeElapsed;
                previousTimeElapsed = totalTimeElapsed;
                totalTimeBeforeUpdate += deltaTime;

                if(totalTimeBeforeUpdate >= TIME_TILL_UPDATE)
                {
                    GameTime.Update(totalTimeBeforeUpdate, totalTimeElapsed);
                    totalTimeBeforeUpdate = 0f;
                    Update(GameTime);

                    Window.Clear(WindowClearColor);
                    Window.SetView(GameView);
                    Draw(GameTime);
                    Window.Display();
                   
                }
            }
        }

        public static async Task<bool> Wait(int milliSeconds)
        {
            await Task.Delay(milliSeconds);
            return true;
        }

        public abstract void LoadContent();
        public abstract void Initialize();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
        private void WindowClosed(object sender, EventArgs e)
        {
            Window.Close();
        }
        private void WindowResized(object sender, SizeEventArgs e)
        {
            Window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }
        private void WindowFocus(object sender, EventArgs e)
        {
            Window.SetActive(true);
            hasFocus = true;
        }
        private void WindowUnfocus(object sender, EventArgs e)
        {
            Window.SetActive(false);
            hasFocus = false;
        }
    }
}
