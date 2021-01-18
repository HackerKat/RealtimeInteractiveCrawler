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

        public static SpriteSheet SpriteGround { get; set; } // Ground
        public static SpriteSheet SpriteGrass { get; set; } // Grass
        public static SpriteSheet SpriteEnemy { get; set; } // Enemy
        public static Font Font { get; set; } //font

        public static SpriteSheet SpritePlayer { get; set; } // Player
        public static SpriteSheet SpriteHealth { get; set; } // Health
        public static SpriteSheet SpriteAttack { get; set; } // Attack
        public static SpriteSheet SpriteDefense { get; set; } // Defense

        public static SpriteSheet SpriteInventory { get; set; } // Inventory


        public static void Load()
        {
            SpriteGround = new SpriteSheet(Tile.TILE_SIZE, Tile.TILE_SIZE, false, 1, new Texture(CONTENT_DIR + "TerrariaImages\\" + "Tiles_0.png"));
            SpriteGrass = new SpriteSheet(Tile.TILE_SIZE, Tile.TILE_SIZE, false, 1, new Texture(CONTENT_DIR + "TerrariaImages\\" + "Tiles_1.png"));

            
            SpritePlayer = new SpriteSheet(9, 4, true, 0, new Texture(CONTENT_DIR + "Character\\" + "human_base.png"));

            
            SpriteHealth = new SpriteSheet(128, 128, false, 0, new Texture(CONTENT_DIR + "Items\\" + "Health.png"));
            SpriteAttack = new SpriteSheet(128, 128, false, 0, new Texture(CONTENT_DIR + "Items\\" + "Attack.png"));
            SpriteDefense = new SpriteSheet(128, 128, false, 0, new Texture(CONTENT_DIR + "Items\\" + "Defense.png"));

            SpriteInventory = new SpriteSheet(128, 128, false, 0, new Texture(CONTENT_DIR + "UI\\" + "Ladder.png"));
           
            SpriteEnemy = new SpriteSheet(24, 24, false, 0, new Texture(CONTENT_DIR + "NPC\\" + "HalflingFighter2.png"));

            Font = new Font(CONTENT_DIR + "Fonts\\arial\\arial.ttf");
        }
    }
}
