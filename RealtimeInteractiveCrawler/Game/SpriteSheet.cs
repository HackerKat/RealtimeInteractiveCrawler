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
        int subW, subH; // width and height of sprite
        int borderSize; // border between sprite elements in one picture

        public int SubWidth { get { return subW; } }
        public int SubHeight { get { return subH; } }

        // a and b size of one sprite OR amount of sprite along one axis

        /// <summary>
        ///
        /// </summary>
        /// <param name="a">Amount of fragments on X axis or width of one fragment in px</param>
        /// <param name="b">Amount of fragments on Y axis or height of one fragment in px</param>
        /// <param name="borderSize">Space between sprites</param>
        /// <param name="texW">Width of texture in px</param>
        /// <param name="texH">Height of texture in px</param>
        public SpriteSheet(int a, int b, int borderSize, int texW = 0, int texH = 0)
        {
            if (borderSize > 0)
                this.borderSize = borderSize + 1;
            else
                this.borderSize = 0;

            if(texW != 0 && texH != 0)
            {
                subW = (int)Math.Ceiling((float)texW / a);
                subH = (int)Math.Ceiling((float)texH / b);
            }
            else
            {
                subW = a;
                subH = b;
            }
        }

        public IntRect GetTextureRect(int i, int j)
        {
            int x = i * subW + i * borderSize;
            int y = j * subH + j * borderSize;
            return new IntRect(x, y, subW, subH);
        }

    }
}
