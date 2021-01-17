using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class UIInventoryCell : UIBase
    {
        public UIInventory Inventory { get; private set; }

        public UIInventoryCell(UIInventory inventory)
        {
            Inventory = inventory;

            rectShape = new RectangleShape((Vector2f)Content.SpriteInventory.Texture.Size);
            rectShape.Texture = Content.SpriteInventory.Texture;
        }

    }
}
