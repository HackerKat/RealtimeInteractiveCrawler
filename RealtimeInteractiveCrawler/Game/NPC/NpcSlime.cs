using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class NpcSlime : Npc
    {
        public NpcSlime(World world) : base(world)
        {
            // Size of rectangle
            rect = new RectangleShape(new Vector2f(Tile.TILE_SIZE * 1.5f, Tile.TILE_SIZE));
            // Center of rectangle
            rect.Origin = new Vector2f(rect.Size.X * 0.5f, 0);
            rect.FillColor = Color.Green;
        }

        public override void DrawNPC(RenderTarget target, RenderStates states)
        {
        }

        public override void OnKill()
        {
            Spawn();
        }

        public override void OnWallCollided()
        {
            Direction *= -1;
        }

        public override void UpdateNPC()
        {
            if (!isFlying)
                velocity = new Vector2f(Direction * MainClass.Rand.Next(1, 10), -MainClass.Rand.Next(6, 9));
        }
    }
}
