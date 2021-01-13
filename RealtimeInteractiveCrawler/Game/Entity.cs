using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace RealtimeInteractiveCrawler
{
    public class Entity
    {
        protected Sprite sprite;
        public Entity(Sprite sprite) 
        {
            this.sprite = sprite;
        }
    }
}
