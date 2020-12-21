using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace RealtimeInteractiveCrawler
{
    public class InputManager
    {
        public InputManager()
        {
        }

        public bool getKeyDown(Keyboard.Key key)
        {
            return Keyboard.IsKeyPressed(key);
        }
    }
}
