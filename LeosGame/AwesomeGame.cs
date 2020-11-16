using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class AwesomeGame : GameLoop
    {
        public const uint DEFAULT_WIDTH = 1280;
        public const uint DEFAULT_HEIGHT = 720;
        public const string TITLE = "OBAMA CARES";
        public AwesomeGame() : base(DEFAULT_WIDTH, DEFAULT_HEIGHT, TITLE, Color.Black)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            DebugUtility.DrawPerformanceData(this, Color.White);
        }

        public override void Initialize()
        {
        }

        public override void LoadContent()
        {
            DebugUtility.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
