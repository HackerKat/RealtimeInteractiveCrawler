using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class Content
    {
        public const string CONTENT_DIR = "..\\Assets\\";

        public static Texture TexTile0; // Ground
        public static Texture TexTile1; // Grass

        public static Texture TexPlayer; // Player
        public static Texture TexPlay1; // Enemy
        public static Font Font; //font

        public static void Load()
        {
            TexTile0 = new Texture(CONTENT_DIR + "TerrariaImages\\" + "Tiles_0.png");
            TexTile1 = new Texture(CONTENT_DIR + "TerrariaImages\\" + "Tiles_1.png");

            TexPlayer = new Texture(CONTENT_DIR + "Character\\" + "human_base.png");
            TexPlay1 = new Texture(CONTENT_DIR + "NPC\\" + "slime.png");

            Font = new Font(CONTENT_DIR + "Fonts\\arial\\arial.ttf");
        }
    }
}
