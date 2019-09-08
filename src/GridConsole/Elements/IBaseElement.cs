using System;
using GridConsole.Constants;

namespace GridConsole.Elements
{
    public delegate void EnterPressedDelegate(IBaseElement element, object parameter);

    public interface IBaseElement
    {
        int RowSpan { get; }
        int ColumnSpan { get; }
        int Width { get; }
        int Height { get; }
        bool CanBeSelected { get; }
        bool IsSelected { get; set; }
                
        Color ForegroundColor { get; set; }
        Color BackgroundColor { get; set; }
        Color HighlightForegroundColor { get; set; }
        Color HighlightBackgroundColor { get; set; }


        event EnterPressedDelegate OnEnterPressed;
        void EnterPressed();
        void Draw(IConsole console, int x, int y);
    }
}
