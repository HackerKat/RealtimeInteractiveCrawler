using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    class AnimationFrame
    {
        public int i, j;
        public float time;

        public AnimationFrame(int i, int j, float time)
        {
            this.i = i;
            this.j = j;
            this.time = time;
        }
    }

    class Animation
    {
        AnimationFrame[] frames;
        float timer;
        AnimationFrame currFrame;
        int currFrameIndex;

        public Animation(params AnimationFrame[] frames)
        {
            this.frames = frames;
            Reset();
        }

        public void Reset()
        {
            timer = 0f;
            currFrameIndex = 0;
            currFrame = frames[currFrameIndex];
        }

        public void NextFrame()
        {
            timer = 0f;
            currFrameIndex++;

            if (currFrameIndex == frames.Length)
                currFrameIndex = 0;

            currFrame = frames[currFrameIndex];
        }

        public AnimationFrame Getframe(float speed)
        {
            timer += speed;

            if (timer >= currFrame.time)
                NextFrame();

            return currFrame;
        }

    }

    class AnimSprite : Transformable, Drawable
    {
        public float Speed = 0.05f;
        RectangleShape rectShape;
        SpriteSheet spriteSheet;

        // Probably to replace
        SortedDictionary<string, Animation> animations = new SortedDictionary<string, Animation>();
        string currAnimName;

        Animation currAnim;

        public Color color
        {
            set { rectShape.FillColor = value; }
            get { return rectShape.FillColor; }
        }

        public RectangleShape RectShape { get => rectShape;}

        public AnimSprite(Texture texture, SpriteSheet spriteSheet)
        {
            this.spriteSheet = spriteSheet;
            float size = 1.5f;
            rectShape = new RectangleShape(new Vector2f(spriteSheet.SubWidth * size, spriteSheet.SubHeight * size));
            rectShape.Origin = new Vector2f(spriteSheet.SubWidth * size * 0.5f, spriteSheet.SubHeight * size * 0.5f);
            rectShape.Texture = texture;
        }

        public void AddAnimation(string name, Animation animation)
        {
            animations[name] = animation;
            currAnim = animation;
            currAnimName = name;
        }

        public void Play(string name)
        {
            if (currAnimName == name)
                return;

            currAnim = animations[name];
            currAnimName = name;
            currAnim.Reset();
        }

        public IntRect GetTextureRect()
        {
            var currFrame = currAnim.Getframe(Speed);
            return spriteSheet.GetTextureRect(currFrame.i, currFrame.j);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            rectShape.TextureRect = GetTextureRect();

            states.Transform *= Transform;
            target.Draw(rectShape, states);
        }
    }
}
