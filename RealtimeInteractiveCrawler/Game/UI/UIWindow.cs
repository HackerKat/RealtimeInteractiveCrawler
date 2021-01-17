using SFML.Graphics;
using SFML.System;

namespace RealtimeInteractiveCrawler
{
    class UIWindow : UIBase
    {
        public const int TITLE_BAR_HEIGHT = 25;

        public bool IsVisibleTitleBar = true;
        public Color BodyColor = new Color(80, 90, 70, 127);
        public Color TitleColor = new Color(60, 50, 50, 127);
        public Color TitleColorOver = new Color(60, 50, 50, 255);

        private RectangleShape rectShapeTitleBar;

        public UIWindow()
        {
            rectShape = new RectangleShape(new Vector2f(400, 100));
            rectShapeTitleBar = new RectangleShape(new Vector2f(rectShape.Size.X, TITLE_BAR_HEIGHT));

            ApplyColors();
        }

        public void ApplyColors()
        {
            rectShape.FillColor = BodyColor;
            rectShapeTitleBar.FillColor = (UIManager.Over == this || UIManager.Drag == this) ? TitleColorOver : TitleColor;
        }

        public override void UpdateOver(Vector2i mousePos)
        {
            base.UpdateOver(mousePos);

            if (IsVisibleTitleBar)
            {
                var localMousePos = mousePos - GlobalPosition + GlobalOrigin;
                IsAllowedDrag = UIManager.Drag == this || rectShapeTitleBar.GetGlobalBounds().Contains(localMousePos.X, localMousePos.Y);
            }
        }

        public override void Update()
        {
            base.Update();

            ApplyColors();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);
            states.Transform *= Transform;

            if (IsVisibleTitleBar)
                target.Draw(rectShapeTitleBar, states);
        }

        public override void OnCancelDrag()
        {
            
        }
    }
}
