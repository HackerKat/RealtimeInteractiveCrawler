using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    public class SpriteSheet
    {
        int borderSize; // border between sprite elements in one picture

        public int SubWidth { get; private set; }
        public int SubHeight { get; private set; }
        public int SubCountX { get; private set; }
        public int SubCountY { get; private set; }

        public Texture Texture;

        /// <summary>
        ///
        /// </summary>
        /// <param name="a">Amount of fragments on X axis or width of one fragment in px</param>
        /// <param name="b">Amount of fragments on Y axis or height of one fragment in px</param>
        /// <param name="abIsCount">a and b is the amount of sprites in x and y direction</param>
        /// <param name="borderSize">Space between sprites</param>
        /// <param name="texture"></param>
        /// <param name="isSmooth"></param>
        public SpriteSheet(int a, int b, bool abIsCount, int borderSize, Texture texture, bool isSmooth = false)
        {
            Texture = texture;
            texture.Smooth = isSmooth; // evil thing, try not to use

            if (borderSize > 0)
                this.borderSize = borderSize + 1;
            else
                this.borderSize = 0;

            if (abIsCount)
            {
                SubWidth = (int)Math.Ceiling((float)texture.Size.X / a);
                SubHeight = (int)Math.Ceiling((float)texture.Size.Y / b);
                SubCountX = a;
                SubCountY = b;
            }
            else
            {
                SubWidth = a;
                SubHeight = b;
                SubCountX = (int)Math.Ceiling((float)texture.Size.X / a);
                SubCountY = (int)Math.Ceiling((float)texture.Size.Y / b);
            }
        }

        public void Dispose()
        {
            Texture.Dispose();
            Texture = null;
        }

        public IntRect GetTextureRect(int i, int j)
        {
            int x = i * SubWidth + i * borderSize;
            int y = j * SubHeight + j * borderSize;
            return new IntRect(x, y, SubWidth, SubHeight);
        }

    }
}
