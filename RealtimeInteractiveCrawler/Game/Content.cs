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

        public static Texture TexGround; // Ground
        public static Texture TexGrass; // Grass

        public static Texture Enemy1; // Enemy
        public static Texture Enemy2; // Enemy
        public static Font Font; //font

        public static Texture TexPlayer { get; set; } // Player
        public static Texture TexHealth { get; set; } // Health
        public static Texture TexAttack { get; set; } // Attack
        public static Texture TexDefense { get; set; } // Defense

        public static void Load()
        {
            TexGround = new Texture(CONTENT_DIR + "TerrariaImages\\" + "Tiles_0.png");
            TexGrass = new Texture(CONTENT_DIR + "TerrariaImages\\" + "Tiles_1.png");

            TexPlayer = new Texture(CONTENT_DIR + "Character\\" + "human_base.png");

            TexHealth = new Texture(CONTENT_DIR + "Items\\" + "Health.png");
            TexAttack = new Texture(CONTENT_DIR + "Items\\" + "Attack.png");
            TexDefense = new Texture(CONTENT_DIR + "Items\\" + "Defense.png");


            Enemy1 = new Texture(CONTENT_DIR + "NPC\\" + "slime.png");
            Enemy2 = new Texture(CONTENT_DIR + "NPC\\" + "HalflingFighter.png");

            Font = new Font(CONTENT_DIR + "Fonts\\arial\\arial.ttf");
        }
    }
}
