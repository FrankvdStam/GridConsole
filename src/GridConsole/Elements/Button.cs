using GridConsole.Constants;
using System;

namespace GridConsole.Elements
{
    public class Button : IBaseElement { 

        public Button(ElementData elementData)
        {
            Text                     = elementData.Text;
            Parameter                = elementData.Parameter;
            OnEnterPressed          += elementData.OnEnterPressed;
            ForegroundColor          = elementData.ForegroundColor;
            BackgroundColor          = elementData.BackgroundColor;
            HighlightForegroundColor = elementData.HighlightForegroundColor;
            HighlightBackgroundColor = elementData.HighlightBackgroundColor;
            RowSpan                  = elementData.RowSpan;
            ColumnSpan               = elementData.ColumnSpan;

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
