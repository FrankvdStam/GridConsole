using GridConsole.Constants;

namespace GridConsole.Elements
{
    public class Text : ABaseElement
    {
        public Text(
            string text,
            Color foregroundColor = Color.White,
            Color backgroundColor = Color.Black
        ) : base(
            foregroundColor,
            backgroundColor,
            //Text can't be highlighted
            Color.Black,
            Color.Black
        )
        {
            Text_ = text;
        }

        public override int Width => Text_.Length;

        public override int Height => 1;

        public override bool CanBeSelected => false;

        public string Text_ { get; set; }

        public override void Draw(IConsole console, int x, int y)
        {
            console.SetForegroundColor(ForegroundColor);
            console.SetBackgroundColor(BackgroundColor);
            console.Gotoxy(x, y);
            foreach (char c in Text_)
            {
                console.Write(c);
            }
        }
    }
}
