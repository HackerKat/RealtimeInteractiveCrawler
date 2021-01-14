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

        public static Texture TextGround; // Ground
        public static Texture TexGrass; // Grass

        private static Texture texPlayer; // Player
        public static Texture TexPlay1; // Enemy
        public static Font Font; //font

        public static Texture TexPlayer { get => texPlayer;}

        public static void Load()
        {
            TextGround = new Texture(CONTENT_DIR + "TerrariaImages\\" + "Tiles_0.png");
            TexGrass = new Texture(CONTENT_DIR + "TerrariaImages\\" + "Tiles_1.png");

            texPlayer = new Texture(CONTENT_DIR + "Character\\" + "human_base.png");
            TexPlay1 = new Texture(CONTENT_DIR + "NPC\\" + "slime.png");

            Font = new Font(CONTENT_DIR + "Fonts\\arial\\arial.ttf");
        }
    }
}
