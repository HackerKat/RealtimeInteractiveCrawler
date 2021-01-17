using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class UIInventory : UIWindow
    {
        public List<UIInventoryCell> cells = new List<UIInventoryCell>();

        public UIInventory()
        {
            IsVisibleTitleBar = false;
            //BodyColor = Color.Transparent;

            int cellCount = 3;

            for (int i = 0; i < cellCount; i++)
            {
                AddCell();
            }

            Size = new Vector2i((int)Content.SpriteInventory.Texture.Size.X * cellCount, (int)Content.SpriteInventory.Texture.Size.Y);
        }

        public void AddCell()
        {
            var cell = new UIInventoryCell(this);
            cell.Position = new Vector2i(cells.Count * cell.Width, 0);
            cells.Add(cell);
            Children.Add(cell);
        }
    }
}
