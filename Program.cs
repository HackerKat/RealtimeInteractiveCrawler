//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SFML;
//using SFML.Graphics;
//using SFML.Window;
//using SFML.System;

//namespace RealtimeInteractiveCrawler
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            var window = new RenderWindow(new VideoMode(800, 600), "RealtimeInteractiveCrawler");
//            Texture texture = new Texture("image.png");
//            Sprite sprite = new Sprite();
//            sprite.Texture = texture;

//            window.Closed += Window_Closed;
//            while (window.IsOpen)
//            {
//                window.DispatchEvents();
//                window.Clear(Color.Cyan);
//                sprite.Position = new Vector2f(100f, 100f);
//                sprite.Scale = new Vector2f(0.1f, 0.1f);
//                window.Draw(sprite);
//                //drawing
//                window.Display();
//            }
//        }

//        private static void Window_Closed(object sender, EventArgs e)
//        {
//            RenderWindow win = (RenderWindow)sender;
//            win.Close();
//        }
//    }
//}
