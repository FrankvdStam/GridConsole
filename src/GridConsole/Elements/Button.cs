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
        public EnterPressedDelegate OnEnterPressed;
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

        public static ButtonData Pressed(this ButtonData buttonData, object parameter, EnterPressedDelegate enterPressed)
        {
            buttonData.OnEnterPressed = enterPressed;
            buttonData.Parameter = parameter;
            return buttonData;
        }
    }

    public class Button : IBaseElement { 

        public Button(ButtonData buttonData)
        {
            Text                     = buttonData.Text;
            Parameter                = buttonData.Parameter;
            OnEnterPressed          += buttonData.OnEnterPressed;
            ForegroundColor          = buttonData.ForegroundColor;
            BackgroundColor          = buttonData.BackgroundColor;
            HighlightForegroundColor = buttonData.HighlightForegroundColor;
            HighlightBackgroundColor = buttonData.HighlightBackgroundColor;


            if (string.IsNullOrWhiteSpace(Text))
            {
                throw new Exception("Text can't be null.");
            }
        }
        
        public Button(string text)
        {
            Text = text;
        }

        #region IBaseElement ==============================================================================================================================================
        public int RowSpan { get; private set; } = 1;
        public int ColumnSpan { get; private set; } = 1;
        public int Width => Text.Length;
        public int Height => 1;
        public bool CanBeSelected => true;

        public bool IsSelected { get; set; } 
        public Color ForegroundColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color HighlightForegroundColor { get; set; } = Color.Black;
        public Color HighlightBackgroundColor { get; set; } = Color.White;

        public event EnterPressedDelegate OnEnterPressed;

        public void Draw(IConsole console, int x, int y)
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

        public void EnterPressed()
        {
            OnEnterPressed?.Invoke(this, Parameter);
        }
        #endregion        

        public object Parameter = null;
        public string Text { get; set; }

        public override string ToString()
        {
            return "Button " + Text;
        }
    }
}
