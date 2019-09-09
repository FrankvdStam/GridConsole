using GridConsole.Constants;
using GridConsole.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace GridConsole
{
    //public interface IElementData
    //{
    //    int RowSpan { get; set; }
    //    int ColumnSpan { get; set; }
    //
    //    string Text { get; set; }
    //    object Parameter { get; set; }
    //
    //    Color ForegroundColor { get; set; }
    //    Color BackgroundColor { get; set; }
    //    Color HighlightForegroundColor { get; set; }
    //    Color HighlightBackgroundColor { get; set; }
    //
    //    EnterPressedDelegate OnEnterPressed { get; set; }
    //}

    public class ElementData
    {
        public int Width;
        public int Height;

        public int MarginWidth = 1;
        public int MarginHeight;

        public int RowSpan = 1;
        public int ColumnSpan = 1;

        public string Text;
        public object Parameter;

        public Color ForegroundColor = Color.White;
        public Color BackgroundColor = Color.Black;
        public Color HighlightForegroundColor = Color.Black;
        public Color HighlightBackgroundColor = Color.White;

        public EnterPressedDelegate OnEnterPressed;
        public IConsole Target;

    }

    public static class ElementFactory
    {
        public static Grid CreateGrid(Func<ElementData, ElementData> construct)
        {
            ElementData elementData = new ElementData();
            elementData = construct(elementData);
            return new Grid(elementData);
        }

        /// <summary>
        /// Create a new button element
        /// </summary>
        public static Button CreateButton(Func<ElementData, ElementData> construct)
        {
            ElementData elementData = new ElementData();
            elementData = construct(elementData);
            return new Button(elementData);
        }

        /// <summary>
        /// Create a new text element
        /// </summary>
        public static Text CreateText(Func<ElementData, ElementData> construct)
        {
            ElementData elementData = new ElementData();
            elementData = construct(elementData);
            return new Text(elementData);
        }

        /// <summary>
        /// Sets the text
        /// </summary>
        public static ElementData Text(this ElementData elementData, string text)
        {
            elementData.Text = text;
            return elementData;
        }

        /// <summary>
        /// Sets the colors
        /// </summary>
        public static ElementData Colors(this ElementData elementData, Color foregroundColor, Color backgroundColor)
        {
            elementData.ForegroundColor = foregroundColor;
            elementData.BackgroundColor = backgroundColor;
            return elementData;
        }

        /// <summary>
        /// Sets the highlight colors
        /// </summary>
        public static ElementData Highlight(this ElementData elementData, Color foregroundColor, Color backgroundColor)
        {
            elementData.HighlightForegroundColor = foregroundColor;
            elementData.HighlightBackgroundColor = backgroundColor;
            return elementData;
        }

        /// <summary>
        /// Sets the OnEnterPressed event and parameter for classes that support it
        /// </summary>
        public static ElementData Pressed(this ElementData elementData, EnterPressedDelegate enterPressed, object parameter)
        {
            elementData.Parameter = parameter;
            elementData.OnEnterPressed = enterPressed;
            return elementData;
        }

        /// <summary>
        /// Sets the console target for Grid classes
        /// </summary>
        public static ElementData Target(this ElementData elementData, IConsole target)
        {
            elementData.Target = target;
            return elementData;
        }

        /// <summary>
        /// Sets the width and height of the Grid class, other elements automatically figure out their size
        /// </summary>
        public static ElementData Size(this ElementData elementData, int width, int height)
        {
            elementData.Width = width;
            elementData.Height = height;
            return elementData;
        }

        /// <summary>
        /// Sets the margin size of the Grid class, not supported on other classes
        /// </summary>
        public static ElementData Margin(this ElementData elementData, int width, int height)
        {
            elementData.MarginWidth = width;
            elementData.MarginHeight = height;
            return elementData;
        }

        public static ElementData ColumnSpan(this ElementData elementData, int columnSpan)
        {
            elementData.ColumnSpan = columnSpan;
            return elementData;
        }

        public static ElementData RowSpan(this ElementData elementData, int rowSpan)
        {
            elementData.RowSpan = rowSpan;
            return elementData;
        }
    }
}
