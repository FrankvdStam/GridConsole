using System;
using System.Collections.Generic;
using System.Linq;
using GridConsole.Constants;
using GridConsole.Elements;

namespace GridConsole
{
    public class Grid : IBaseElement
    {
        #region constructor
        public Grid(ElementData elementData)
        {
            GridWidth       = elementData.Width;
            GridHeight      = elementData.Height;
            _elements       = new IBaseElement[GridWidth, GridHeight];
            MarginWidth     = elementData.MarginWidth;
            MarginHeight    = elementData.MarginHeight;
            Parameter       = elementData.Parameter;
            OnEnterPressed += elementData.OnEnterPressed;
            _console        = elementData.Target;
            Text            = elementData.Text;
            ForegroundColor = elementData.ForegroundColor;
            BackgroundColor = elementData.BackgroundColor;
            HighlightForegroundColor = elementData.HighlightForegroundColor;
            HighlightBackgroundColor = elementData.HighlightBackgroundColor;
            RowSpan = elementData.RowSpan;
            ColumnSpan = elementData.ColumnSpan;

            if (GridWidth <= 0 && GridHeight <= 0 && _console != null)
            {
                throw new Exception("Grid needs a width and height larger then 1 and a proper target. Try calling Size() and Target().");
            }
        }
        #endregion

        #region IBaseElement ========================================================================================================

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
        
        public object Parameter { get; set; }

        public event EnterPressedDelegate OnEnterPressed;
        public void EnterPressed()
        {
            OnEnterPressed?.Invoke(this, Parameter);
        }
                       
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
            foreach (char c in Text)
            {
                console.Write(c);
            }
        }
        #endregion
        
        #region properties/utilities
        private readonly IConsole _console;
        private IBaseElement[,] _elements;
        //Subgrid is found dynamically when selected
        private Grid _subGrid = null; 
        //Parentgrid must be passed as reference when constructed
        private Grid _parentGrid = null; 
        private bool _clearScreen = false;

        //These arrays contain the size of the columns and rows, are recalculated everytime an element is added
        private int[] _columnWidths;
        private int[] _rowHeights;

        public string Text { get; set; }
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }
        public int MarginWidth { get; private set; } = 1;
        public int MarginHeight { get; private set; }
        
        /// <summary>
        /// Resizes the grid. All previous data is thrown away
        /// </summary>
        public void Resize(int width, int height)
        {
            GridWidth = width;
            GridHeight = height;
            _elements = new IBaseElement[GridWidth, GridHeight];
            ReCalculateSizes();
        }

        ///// <summary>
        ///// Access a specific element
        ///// </summary>
        //public ABaseElement this[int x, int y]
        //{
        //    get => _elements[x, y];
        //    set => _elements[x, y] = value;
        //}

        public IBaseElement SelectedElement;

        private void NavigateToElement(IBaseElement element)
        {
            if (SelectedElement != null)
            {
                SelectedElement.IsSelected = false;
            }
            SelectedElement = element;
            SelectedElement.IsSelected = true;
        }


        /// <summary>
        /// Gets the position of an element in the grid
        /// </summary>
        private bool TryGetElementGridPosition(IBaseElement element, out int x, out int y)
        {
            x = 0;
            y = 0;
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    if (_elements[i, j] == element)
                    {
                        x = i;
                        y = j;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Enumerates all the elements in the grid.
        /// </summary>
        public IEnumerable<IBaseElement> EnumerateElements()
        {
            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    yield return _elements[i, j];
                }
            }
        }
        #endregion

        #region Render and input
        /// <summary>
        /// Clears the screen on the next render cycle
        /// </summary>
        public void ClearScreen()
        {
            _clearScreen = true;
        }

        /// <summary>
        /// Mainloop, renders and calls input on every cycle.
        /// </summary>
        public void Run()
        {
            while(true)
            {
                Render();
                HandleInput();
            }
        }

        /// <summary>
        /// Waits for keyboard input (halts the caller). Does NOT call draw again, you must do this yourself.
        /// </summary>
        public void HandleInput(Keys key = Keys.None)
        {
            if (key == Keys.None)
            {
                key = _console.GetKey();
            }

            if (_subGrid == null)
            {
                //Navigation
                if (key == Keys.Backspace && _parentGrid != null)
                {
                    _parentGrid._subGrid = null;
                    _parentGrid._clearScreen = true;
                }

                if (key == Keys.UpArrow || key == Keys.DownArrow || key == Keys.LeftArrow || key == Keys.RightArrow)
                {
                    TryNavigate(key);
                }

                if (key == Keys.Enter)
                {
                    SelectedElement?.EnterPressed();

                    if (SelectedElement is Grid subGrid)
                    {
                        _subGrid = subGrid;
                        _clearScreen = true;
                    }
                }
            }
            else
            {
                _subGrid.HandleInput(key);
            }
        }

        /// <summary>
        /// Draws the current grid state on the console
        /// </summary>
        public void Render()
        {
            if (_clearScreen)
            {
                _console.SetBackgroundColor(Color.Black);
                _clearScreen = false;
                _console.Clear();
            }

            if (_subGrid != null)
            {
                _subGrid.Render();
                return;
            }

            int[] columnCoords = CalculateColumnCoords();
            int[] rowCoords = CalculateRowCoords();

            //Elements can exists in the array multiple times if they have a row or column span.
            //Since we draw right to left and top to bottom, it's safe to keep track of which elements we've drawn to only draw them once.
            List<IBaseElement> drawnElements = new List<IBaseElement>();

            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    if(_elements[i, j] != null && !drawnElements.Contains(_elements[i, j]))
                    {                        
                        _elements[i, j].Draw(_console, columnCoords[i], rowCoords[j]);
                        drawnElements.Add(_elements[i, j]);
                    }
                }
            }

        }
        #endregion

        #region Add
        public void Add(int x, int y, IBaseElement element)
        {
            _elements[x, y] = element;
            //for (int _x = x; _x < x + element.ColumnSpan; _x++)
            //{
            //    for (int _y = y; _y < y + element.RowSpan; _y++)
            //    {
            //        _elements[_x, _y] = element;
            //    }
            //}

            if(element is Grid grid)
            {
                grid._parentGrid = this;
            }

            ReCalculateSizes();
        }
        #endregion

        #region Navigation ===============================================================================================================================
        private void TryNavigate(Keys key)
        {
            //No selection yet
            if (SelectedElement == null)
            {
                if (EnumerateElements().Any(i => i.CanBeSelected))
                {
                    //Navigate to the first logical element
                    NavigateToElement(EnumerateElements().First(i => i.CanBeSelected));
                }
                else
                {
                    //No elements CAN be selected. 
                    return;
                }
            }
            else
            {
                NavigateFromSelectedItem(key);
            }
        }

        private void AddDirectionToCoordsFromKey(Keys key, ref int x, ref int y)
        {
            if (key == Keys.UpArrow)
            {
                y--;
            }
            if (key == Keys.DownArrow)
            {
                y++;
            }
            if (key == Keys.LeftArrow)
            {
                x--;
            }
            if (key == Keys.RightArrow)
            {
                x++;
            }
        }

        private void NavigateFromSelectedItem(Keys key)
        {
            if (!TryGetElementGridPosition(SelectedElement, out int x, out int y))
            {
                throw new Exception("Can't obtain coordinates of the current element.");
            }
            

            //These are grid locations, not actual x,y coords for characters on the screen. Thats why this
            //already takes care of element size
            AddDirectionToCoordsFromKey(key, ref x, ref y);

            int tries = 10;
            

            while (!TryNavigateToGridCoordinates(x, y))
                //while (!TryNavigateToGridCoordinates(elementX + directionX, elementY + directionY))
            {
                AddDirectionToCoordsFromKey(key, ref x, ref y);
                LoopCoordinates(ref x, ref y);
                tries--;
                if (tries == 0)
                {
                    return;
                }
            }

        }
        
        /// <summary>
        /// If the coordinates get out of bounds of the width and height of the grid, they automatically are looped around to the other side with this method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void LoopCoordinates(ref int x, ref int y)
        {
            if (x < 0)
            {
                x = GridWidth - 1;
            }

            if (x >= GridWidth)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = GridHeight - 1;
            }

            if (y >= GridHeight)
            {
                y = 0;
            }
        }

        private bool TryNavigateToGridCoordinates(int x, int y)
        {
            //if NOT within bounds of the grid
            //if (  !(x >= 0 && x < Width && y >= 0 && y < Height) )
            //{
                //Reverse t
                if (x < 0)
                {
                    x = GridWidth - 1;
                }

                if (x >= GridWidth)
                {
                    x = 0;
                }

                if (y < 0)
                {
                    y = GridHeight - 1;
                }

                if (y >= GridHeight)
                {
                    y = 0;
                }
            //}

            if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight && _elements[x, y] != null && _elements[x, y].CanBeSelected)
            {
                NavigateToElement(_elements[x, y]);
                return true;
            }
            return false;
        }
        #endregion
  
        #region Rasterization ===============================================================================================================================
        private int[] CalculateColumnCoords()
        {
            int[] columnCoords = new int[GridWidth];
            int width = 0;
            for (int i = 0; i < _columnWidths.Length; i++)
            {
                if (i == 0)
                {
                    columnCoords[i] = 0;
                }
                else
                {
                    width += _columnWidths[i - 1] + MarginWidth;
                    columnCoords[i] = width;
                }
            }
            return columnCoords;
        }

        private int[] CalculateRowCoords()
        {
            int[] rowCoords = new int[GridHeight];
            int height = 0;
            for (int i = 0; i < _rowHeights.Length; i++)
            {
                if (i == 0)
                {
                    rowCoords[i] = 0;
                }
                else
                {
                    height += _rowHeights[i - 1] + MarginHeight;
                    rowCoords[i] = height;
                }
            }
            return rowCoords;
        }
                   
        private void ReCalculateSizes()
        {
            _columnWidths = CalculateColumnWidths();
            _rowHeights = CalculateRowHeights();
        }

        private int[] CalculateColumnWidths()
        {
            int[] widths = new int[GridWidth];

            //First step: calculate the max with of each column and store these
            for (int x = 0; x < GridWidth; x++)
            {
                int max = 0;
                for (int y = 0; y < GridHeight; y++)
                {
                    if (_elements[x, y] != null && _elements[x, y].ColumnSpan == 1 && _elements[x, y]?.Width > max)
                    {
                        max = _elements[x, y].Width;
                    }                    
                }
                widths[x] = max;
            }

            //Second step: correct the widths for columnspans not equal to 1.
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    if (_elements[x, y] != null && _elements[x, y].ColumnSpan != 1)
                    {
                        //The element starts at x, y. It has a width, there are margins.
                        //Maybe the element fits in the alocated space. If that is the case, we don't have to correct anything.
                        //First step is to calculate the size available for the element
                        int availableSize = 0;
                        for (int i = x; i < x + _elements[x, y].ColumnSpan; i++)
                        {
                            availableSize += widths[i];
                        }
                        //Add only the internal margins, we can't draw over the margins on the outside.
                        availableSize += (_elements[x, y].ColumnSpan - 1) * MarginWidth;

                        //See if we need to adjust the size:
                        if (availableSize < _elements[x, y].Width)
                        {
                            int increment = _elements[x, y].Width - availableSize;
                            widths[x + _elements[x, y].ColumnSpan - 1] += increment;
                        }
                    }
                }
            }

            return widths;
        }

        private int[] CalculateRowHeights()
        {
            int[] heights = new int[GridHeight];

            //First step: calculate the max with of each column and store these
            for (int y = 0; y < GridHeight; y++)
            {
                int max = 0;
                for (int x = 0; x < GridWidth; x++)
                {
                    if (_elements[x, y] != null && _elements[x, y].RowSpan == 1 && _elements[x, y]?.Height > max)
                    {
                        max = _elements[x, y].Height;
                    }
                    heights[y] = max;
                }
            }

            //Second step: correct the widths for columnspans not equal to 1.
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (_elements[x, y] != null && _elements[x, y].RowSpan != 1)
                    {
                        //The element starts at x, y. It has a width, there are margins.
                        //Maybe the element fits in the alocated space. If that is the case, we don't have to correct anything.
                        //First step is to calculate the size available for the element
                        int availableSize = 0;
                        for (int i = x; i < x + _elements[x, y].RowSpan; i++)
                        {
                            availableSize += heights[i];
                        }
                        //Add only the internal margins, we can't draw over the margins on the outside.
                        availableSize += (_elements[x, y].RowSpan - 1) * MarginHeight;

                        //See if we need to adjust the size:
                        if (availableSize < _elements[x, y].Height)
                        {
                            int increment = _elements[x, y].Height - availableSize;
                            heights[x + _elements[x, y].RowSpan - 1] += increment;
                        }
                    }
                }
            }
            return heights;
        }
        #endregion
    }
}
