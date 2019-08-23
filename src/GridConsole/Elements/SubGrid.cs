using System;
using System.Collections.Generic;
using System.Text;
using GridConsole.Constants;

namespace GridConsole.Elements
{
    class SubGrid : ABaseElement
    {
        public SubGrid(
            string text,
            Grid parent,
            Grid child,
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
            if (parent == null || Child == null)
            {
                throw new Exception("SubGrid needs a valid child and parent reference to properly allow navigation.");
            }

            Text = text;
            Parent = parent;
            Child = child;
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
            foreach (char c in Text)
            {
                console.Write(c);
            }
        }

        public Grid Parent { get; set; }
        public Grid Child { get; set; }
        public string Text { get; set; }
        public override int Width => Text.Length;
        public override int Height => 1;

        public override bool CanBeSelected => true;

        public override string ToString()
        {
            return "SubGrid " + Text;
        }
    }
}
