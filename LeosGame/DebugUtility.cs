using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    public static class DebugUtility
    {
        public const string CONSOLE_FONT_PATH = "./fonts/arial/arial.ttf";
        public static Font ConsoleFont;

        public static void LoadContent()
        {
            ConsoleFont = new Font(CONSOLE_FONT_PATH);
        }

        public static void DrawPerformanceData(GameLoop gameLoop, Color fontColor)
        {
            if (ConsoleFont == null)
                return;

            string totalTimeElapsed = gameLoop.GameTime.TotealTimeElapsed.ToString("0.000");
            string deltaTime = gameLoop.GameTime.DeltaTime.ToString("0.00000");
            float fps = 1f / gameLoop.GameTime.DeltaTime;
            string fpsStr = fps.ToString("0.00");

            gameLoop.Window.Draw(CreateText(totalTimeElapsed, 4f, 8f, fontColor));
            gameLoop.Window.Draw(CreateText(deltaTime, 4f, 28f, fontColor));
            gameLoop.Window.Draw(CreateText(fpsStr, 4f, 48f, fontColor));

        }

        private static Text CreateText(string outText, float x, float y, Color col)
        {
            Text text = new Text(outText, ConsoleFont, 14);
            text.Position = new Vector2f(x, y);
            text.Color = col;

            return text;
        }

    }
}
