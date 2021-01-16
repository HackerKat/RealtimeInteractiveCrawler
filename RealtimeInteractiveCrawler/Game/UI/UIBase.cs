using SFML.Graphics;
using SFML.System;
using System;

namespace RealtimeInteractiveCrawler
{
    abstract class UIBase : Transformable, Drawable
    {
        public new Vector2i Position
        {
            get { return (Vector2i)base.Position; }
            set { base.Position = (Vector2f)value; }
        }

        public new Vector2i Origin
        {
            get { return (Vector2i)base.Origin; }
            set { base.Origin = (Vector2f)value; }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {

        }
    }
}
