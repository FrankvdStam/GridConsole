using GridConsole.Constants;

namespace GridConsole.Elements
{
    public class Button : ABaseElement { 
        public Button(
            string text,
            Color foregroundColor = Color.White,
            Color backgroundColor = Color.Black,
            Color highlightForegroundColor = Color.Black,
            Color highlightBackgroundColor = Color.White
        ) : base(
            foregroundColor,
            backgroundColor,
            highlightForegroundColor,
            highlightBackgroundColor
        )
        {
            Text = text;
        }

        public override void Draw(IConsole console, int x, int y)
        {
            if (IsSelected)
            {
                console.SetForegroundColor(HighlightForegroundColor);
                console.SetBackgroundColor(HighlightBackgroundColor);
            }
            else
            {
                console.SetForegroundColor(ForegroundColor);
                console.SetBackgroundColor(BackgroundColor);
            }
            console.Gotoxy(x, y);
            foreach(char c in Text)
            {
                console.Write(c);
            }
        }
        
        public string Text { get; set; }
        public override int Width => Text.Length;
        public override int Height => 1;

        public override bool CanBeSelected => true;

        public override string ToString()
        {
            return "Button " + Text;
        }
    }
}
