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

    public class Text : ABaseElement
    {
        public Text(TextData textData) : base(textData.ForegroundColor, textData.BackgroundColor, Color.Black, Color.Black)
        {
            Text_ = textData.Text;
        }

        public Text(string text) : base(Color.White, Color.Black, Color.Black, Color.Black)
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
