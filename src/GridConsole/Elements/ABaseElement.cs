using GridConsole.Constants;

namespace GridConsole.Elements
{
    public delegate void EnterPressedDelegate(ABaseElement element);

    public abstract class ABaseElement
    {
        protected ABaseElement(
            Color foregroundColor,
            Color backgroundColor,
            Color highlightForegroundColor,
            Color highlightBackgroundColor
        )
        {
            this.ForegroundColor = foregroundColor;
            this.BackgroundColor = backgroundColor;
            this.HighlightForegroundColor = highlightForegroundColor;
            this.HighlightBackgroundColor = highlightBackgroundColor;
        }

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract bool CanBeSelected { get; }
        public bool IsSelected { get; set; } = false;
        
        public event EnterPressedDelegate EnterPressedEvent;

        public void EnterPressed()
        {
            EnterPressedEvent?.Invoke(this);
        }

        public abstract void Draw(IConsole console, int x, int y);

        public Color ForegroundColor          { get; set; }
        public Color BackgroundColor          { get; set; }  
        public Color HighlightForegroundColor { get; set; }
        public Color HighlightBackgroundColor { get; set; }

    }
}
