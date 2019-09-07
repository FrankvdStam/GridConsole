using System;
using System.Collections.Generic;
using System.Linq;
using GridConsole.Constants;
using GridConsole.Elements;

namespace GridConsole
{
    public static class GridFactory
    {
        public static Grid Size(this Grid grid, int width, int height)
        {
            return new Grid(copyGrid: grid, gridWidth: width, gridHeight: height, factorySize: true);
        }

        public static Grid Margin(this Grid grid, int width, int height)
        {
            return new Grid(copyGrid: grid, marginWidth: width, marginHeight: height, factoryMargin: true);
        }

        public static Grid Target(this Grid grid, IConsole console)
        {
            return new Grid(copyGrid: grid, console: console, factoryTarget: true);
        }

        public static Grid Parent(this Grid grid, Grid parentGrid)
        {
            return new Grid(copyGrid: grid, parent: parentGrid, factoryParent: true);
        }

        public static Grid Text(this Grid grid, string text)
        {
            return new Grid(copyGrid: grid, text: text, factoryText: true);
        }
    }
    
    public class Grid : ABaseElement
    {
        #region constructor/Fluent interface
        public Grid(
                Grid copyGrid = null,
                IConsole console = null,
                Grid parent = null,
                string text = null,
                int gridWidth = 1,
                int gridHeight = 1,
                int marginWidth = 1,
                int marginHeight = 0,
                bool factorySize = false,
                bool factoryMargin = false,
                bool factoryTarget = false,
                bool factoryParent = false,
                bool factoryText = false
            ) : base(Color.White, Color.Black, Color.Black, Color.White)
        {
            //Always copy if possible
            if (copyGrid != null)
            {
                GridWidth       = copyGrid.GridWidth;
                GridHeight      = copyGrid.GridHeight;
                _elements = new ABaseElement[GridWidth, GridHeight];
                MarginWidth     = copyGrid.MarginWidth;
                MarginHeight    = copyGrid.MarginHeight;
                _elements       = copyGrid._elements;
                _parentGrid     = copyGrid._parentGrid;
                _console        = copyGrid._console;
            }

            //Cherrypick with properties to overwrite

            if (factorySize)
            {
                GridWidth = gridWidth;
                GridHeight = gridHeight;
                _elements = new ABaseElement[GridWidth, GridHeight];
            }

            if(factoryMargin)
            {
                MarginWidth = marginWidth;
                MarginHeight = marginHeight;
            }

            if(factoryTarget)
            {
                _console = console;
            }

            if(factoryParent)
            {
                _parentGrid = parent;
            }

            if (factoryText)
            {
                Text = text;
            }         
        }

        public static Grid Create()
        {
            return new Grid();
        }    
        
        public Grid Verify()
        {
            if (GridWidth != 0 && GridHeight != 0 && _console != null)
            {
                return this;
            }
            else
            {
                throw new Exception("Grid verification failed. Make sure construction is correct.");
            }
        }

        
        //public Grid() : base(Color.White, Color.Black, Color.Black, Color.White) { }
        //
        //public Grid(Grid g) : base(Color.White, Color.Black, Color.Black, Color.White)
        //{
        //    GridWidth = g.GridWidth;
        //    GridHeight = g.GridHeight;
        //    MarginWidth = g.MarginWidth;
        //    MarginHeight = g.MarginHeight;
        //    _elements = g._elements;
        //    _console = g._console;
        //}

        

        //public Grid(IConsole console, string text, Grid parent, int gridWidth, int gridHeight, int marginWidth = 1, int marginHeight = 0) : base(Color.White, Color.Black, Color.Black, Color.White)
        //{
        //    if (gridWidth == 0 || gridHeight == 0)
        //    {
        //        throw new Exception("Width and height need to be at least 1 in order to accomodate elements.");
        //    }
        //
        //    Text = text;
        //    GridWidth = gridWidth;
        //    GridHeight = gridHeight;
        //    MarginWidth = marginWidth;
        //    MarginHeight = marginHeight;
        //    _elements = new ABaseElement[GridWidth, GridHeight];
        //    _parentGrid = parent;
        //    this._console = console;
        //}
        #endregion

        #region Base element ========================================================================================================
        public string Text { get; set; }
        public override int Width => Text.Length;
        public override int Height => 1;
        public override bool CanBeSelected => true;
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
        #endregion
        
        #region properties/utilities
        private readonly IConsole _console;
        private ABaseElement[,] _elements;
        private Grid _subGrid = null;
        private Grid _parentGrid = null;
        private bool _clearScreen = false;

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
            _elements = new ABaseElement[GridWidth, GridHeight];
        }

        /// <summary>
        /// Access a specific element
        /// </summary>
        public ABaseElement this[int x, int y]
        {
            get => _elements[x, y];
            set => _elements[x, y] = value;
        }

        public ABaseElement SelectedElement;

        private void NavigateToElement(ABaseElement element)
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
        private bool TryGetElementGridPosition(ABaseElement element, out int x, out int y)
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
        /// Enumerates all the elements in the grid. Allows linq queries to be executed easily on the multidimensional grid.
        /// </summary>
        public IEnumerable<ABaseElement> EnumerateElements()
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
        /// Awaits and handles keyboard. Does NOT call draw again, you must do this yourself.
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
                if (key == Keys.Backspace)
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
