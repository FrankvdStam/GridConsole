using GridConsole.Constants;
using System;

namespace GridConsole.Elements
{

    public class TextData
    {
        public string Text;
        public Color ForegroundColor = Color.White;
        public Color BackgroundColor = Color.Black;
    }

    public static class TextFactory
    {
        public static Text Create(Func<TextData, TextData> construct)
        {
            TextData textData = new TextData();
            textData = construct(textData);
            Text text = new Text(textData);
            return text;
        }

        public static TextData Text(this TextData textData, string text)
        {
            textData.Text = text;
            return textData;
        }

        public static TextData Colors(this TextData textData, Color foregroundColor, Color backgroundColor)
        {
            textData.ForegroundColor = foregroundColor;
            textData.BackgroundColor = backgroundColor;
            return textData;
        }
    }

    public class Text : IBaseElement
    {
        public Text(TextData textData)
        {
            Text_ = textData.Text;
            ForegroundColor = textData.ForegroundColor;
            BackgroundColor = textData.BackgroundColor;
        }

        public Text(string text)
        {
            Text_ = text;
        }

        #region IBaseElement ==============================================================================================================================================
        public int RowSpan { get; private set; } = 1;
        public int ColumnSpan { get; private set; } = 1;
        public int Width => Text_.Length;
        public int Height => 1;
        public bool IsSelected { get; set; }
        public Color ForegroundColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Black;

        //Text can't be selected, these are irrelevant
        public bool CanBeSelected => false;
        public Color HighlightForegroundColor { get; set; } = Color.Black;
        public Color HighlightBackgroundColor { get; set; } = Color.White;
               
        public void Draw(IConsole console, int x, int y)
        {
            console.SetForegroundColor(ForegroundColor);
            console.SetBackgroundColor(BackgroundColor);
            console.Gotoxy(x, y);
            foreach (char c in Text_)
            {
                console.Write(c);
            }
        }

        //Text can't be pressed
        public event EnterPressedDelegate OnEnterPressed;
        public void EnterPressed()
        {
            throw new NotImplementedException();
        }
        #endregion

        public string Text_ { get; set; }
    }
}
