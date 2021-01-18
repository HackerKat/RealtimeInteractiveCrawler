using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealtimeInteractiveCrawler.Item;

namespace RealtimeInteractiveCrawler
{

    class SimpleUI : Transformable, Drawable
    {
        private RectangleShape statusBar;
        private Text text;
        private float OutlineThickness = 2;
        private Vector2f position;

        private Font font;



        public SimpleUI(Color color, Vector2f position, string statusText, Vector2f size)
        {
            this.position = position;
            statusBar = new RectangleShape(size)
            {
                FillColor = color,
                Position = position,
                OutlineThickness = OutlineThickness,
                OutlineColor = Color.White
            };
            LoadFont();
            text = CreateText(statusText, position, Color.White);
            
        }

        private Text CreateText(string outText, Vector2f pos, Color col)
        {
            Text text = new Text(outText, font, 14);
            text.Position = pos;
            text.Color = col;

            return text;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;

            float windowLeft = GameLoop.Window.GetView().Center.X - GameLoop.Window.GetView().Size.X * 0.5f;
            float windowTop = GameLoop.Window.GetView().Center.Y - GameLoop.Window.GetView().Size.Y * 0.5f;

            
            target.Draw(statusBar, states);
            target.Draw(text);


            statusBar.Position = new Vector2f(windowLeft + position.X + 70, windowTop + position.Y);
            text.Position = new Vector2f(windowLeft + position.X, windowTop + position.Y);
        }

        // Debug
        private void LoadFont()
        {
            font = new Font("./fonts/arial.ttf");
        }

        public void ChangeStatus(int amount)
        {
            statusBar.Size = new Vector2f(statusBar.Size.X + amount, statusBar.Size.Y);
        }


    }
}
