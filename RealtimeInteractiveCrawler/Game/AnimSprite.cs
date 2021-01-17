using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{
    public class AnimationFrame
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

    public class Animation
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

    public class AnimSprite : Transformable, Drawable
    {
        public enum MovementType
        {
            Idle,
            Horizontal,
            Up,
            Down
        }

        public float Speed = 0.05f;
        RectangleShape rectShape;
        SpriteSheet spriteSheet;

        // Probably to replace
        SortedDictionary<MovementType, Animation> animations = new SortedDictionary<MovementType, Animation>();
        MovementType currAnimType;

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
            float size = 1;
            rectShape = new RectangleShape(new Vector2f(spriteSheet.SubWidth * size, spriteSheet.SubHeight * size))
            {
                Origin = new Vector2f(spriteSheet.SubWidth * size * 0.5f, spriteSheet.SubHeight * size * 0.5f),
                Texture = texture
            };
        }

        public void AddAnimation(MovementType type, Animation animation)
        {


            animations[type] = animation;
            currAnim = animation;
            currAnimType = type;
        }

        public void Play(MovementType type)
        {
            if (currAnimType == type)
                return;

            currAnim = animations[type];
            currAnimType = type;
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
