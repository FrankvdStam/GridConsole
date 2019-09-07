using GridConsole.Constants;
using System;

namespace GridConsole.Elements
{
    public class ButtonData
    {
        public string Text;
        public object Parameter = null;
        public Color ForegroundColor          = Color.White;
        public Color BackgroundColor          = Color.Black;
        public Color HighlightForegroundColor = Color.Black;
        public Color HighlightBackgroundColor = Color.White;
        public EnterPressedDelegate EnterPressed;
    }

    public static class ButtonFactory
    {
        public static Button Create(Func<ButtonData, ButtonData> construct)
        {
            ButtonData buttonData = new ButtonData();
            buttonData = construct(buttonData);
            Button button = new Button(buttonData);
            return button;
        }

        public static ButtonData Text(this ButtonData buttonData, string text)
        {
            buttonData.Text = text;
            return buttonData;
        }

        public static ButtonData Colors(this ButtonData buttonData, Color foregroundColor, Color backgroundColor)
        {
            buttonData.ForegroundColor = foregroundColor;
            buttonData.BackgroundColor = backgroundColor;
            return buttonData;
        }

        public static ButtonData Highlight(this ButtonData buttonData, Color foregroundColor, Color backgroundColor)
        {
            buttonData.HighlightForegroundColor = foregroundColor;
            buttonData.HighlightBackgroundColor = backgroundColor;
            return buttonData;
        }

    }

    public class Button : ABaseElement { 

        public Button(ButtonData buttonData) : base
            (
                buttonData.ForegroundColor,
                buttonData.BackgroundColor,
                buttonData.HighlightForegroundColor,
                buttonData.HighlightBackgroundColor
            )
        {
            Text                = buttonData.Text;
            Parameter           = buttonData.Text;
            EnterPressedEvent  += buttonData.EnterPressed;
              
            if(string.IsNullOrWhiteSpace(Text))
            {
                throw new Exception("Text can't be null.");
            }
        }

        public Button(string text) : base(Color.White, Color.Black, Color.Black, Color.White)
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

        public override void EnterPressed()
        {
            OnEnterPressed(Parameter);
        }

        public object Parameter = null;
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
