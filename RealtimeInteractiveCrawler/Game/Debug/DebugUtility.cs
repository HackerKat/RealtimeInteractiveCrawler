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

        public static Font ConsoleFont = Content.Font;

        public static void DrawPerformanceData(Color fontColor)
        {
            if (ConsoleFont == null)
                return;

            string totalTimeElapsed = GameLoop.GameTime.TotealTimeElapsed.ToString("0.000");
            string deltaTime = GameLoop.GameTime.DeltaTime.ToString("0.00000");
            float fps = 1f / GameLoop.GameTime.DeltaTime;
            string fpsStr = fps.ToString("0.00");

            GameLoop.Window.Draw(CreateText(totalTimeElapsed, 2200f, 8f, fontColor));
            GameLoop.Window.Draw(CreateText(deltaTime, 2200f, 28f, fontColor));
            GameLoop.Window.Draw(CreateText(fpsStr, 2200f, 48f, fontColor));

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
