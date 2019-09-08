using System;
using System.Collections.Generic;
using System.Linq;
using GridConsole.Constants;
using GridConsole.Elements;

namespace GridConsole
{
    public class Grid : IBaseElement
    {
        #region constructor/Fluent interface
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

        public string Text { get; set; }
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }
        public int MarginWidth { get; private set; }
        public int MarginHeight { get; private set; }
        
        /// <summary>
        /// Resizes the grid. All previous data is thrown away
        /// </summary>
        public void Resize(int width, int height)
        {
            GridWidth = width;
            GridHeight = height;
            _elements = new IBaseElement[GridWidth, GridHeight];
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

            for (int i = 0; i < GridWidth; i++)
            {
                for (int j = 0; j < GridHeight; j++)
                {
                    _elements[i, j]?.Draw(_console, columnCoords[i], rowCoords[j]);
                }
            }

        }

        #region Add
        public void Add(int x, int y, IBaseElement element, int columnspan)
        {
            for(int i = x; i < x+columnspan; i++)
            {
                Add(i, y, element);
            }
        }

        public void Add(int x, int y, IBaseElement element)
        {
            _elements[x, y] = element;

            if(element is Grid grid)
            {
                grid._parentGrid = this;
            }
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
            for (int i = 0; i < GridWidth; i++)
            {
                if (i != 0)
                {
                    //Add the width of the previous column plus the width margin
                    width += CalculateColumnWidth(i - 1) + MarginWidth;
                }
                columnCoords[i] = width;
            }
            return columnCoords;
        }

        private int[] CalculateRowCoords()
        {
            int[] rowCoords = new int[GridHeight];
            int height = 0;
            for (int i = 0; i < GridHeight; i++)
            {
                if (i != 0)
                {
                    //Add the height of the previous row plus height margin
                    height += CalculateRowHeight(i - 1) + MarginHeight;
                }
                rowCoords[i] = height;
            }
            return rowCoords;
        }


        private int CalculateColumnWidth(int column)
        {
            int max = 0;
            for (int i = 0; i < GridHeight; i++)
            {
                if (max < _elements[column, i]?.Width)
                {
                    max = _elements[column, i].Width;
                }
            }
            return max;
        }

        private int CalculateRowHeight(int row)
        {
            int max = 0;
            for (int i = 0; i < GridWidth; i++)
            {
                if (max < _elements[i, row]?.Height)
                {
                    max = _elements[i, row].Height;
                }
            }
            return max;
        }
        #endregion
    }
}
