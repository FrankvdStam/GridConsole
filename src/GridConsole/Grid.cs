using System;
using System.Collections.Generic;
using System.Linq;
using GridConsole.Constants;
using GridConsole.Elements;

namespace GridConsole
{
    public class Grid
    {
        public Grid(IConsole console, int width, int height, int marginWidth = 1, int marginHeight = 0) 
        {
            Width = width;
            Height = height;
            MarginWidth = marginWidth;
            MarginHeight = marginHeight;
            _elements = new ABaseElement[Width,Height];
            this._console = console;
        }
        
        #region properties/utilities
        private readonly IConsole _console;
        private ABaseElement[,] _elements;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int MarginWidth { get; private set; }
        public int MarginHeight { get; private set; }
        
        /// <summary>
        /// Resizes the grid. All previous data is thrown away
        /// </summary>
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            _elements = new ABaseElement[Width, Height];
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
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
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
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    yield return _elements[i, j];
                }
            }
        }
        #endregion

        /// <summary>
        /// Awaits and handles keyboard. Does NOT call draw again, you must do this yourself.
        /// </summary>
        public void HandleInput()
        {
            Keys key = _console.GetKey();

            //Navigation
            if (key == Keys.UpArrow || key == Keys.DownArrow || key == Keys.LeftArrow || key == Keys.RightArrow)
            {
                TryNavigate(key);
            }

            if (key == Keys.Enter)
            {
                SelectedElement?.EnterPressed();
            }
        }

        /// <summary>
        /// Draws the current grid state on the console
        /// </summary>
        public void Render()
        {
            int columnWidth = 0;
            int rowHeight = 0;

            int x = 0;
            int y = 0;

            int[] columnCoords = CalculateColumnCoords();
            int[] rowCoords = CalculateRowCoords();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
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
                x = Width - 1;
            }

            if (x >= Width)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = Height - 1;
            }

            if (y >= Height)
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
                    x = Width - 1;
                }

                if (x >= Width)
                {
                    x = 0;
                }

                if (y < 0)
                {
                    y = Height - 1;
                }

                if (y >= Height)
                {
                    y = 0;
                }
            //}

            if (x >= 0 && x < Width && y >= 0 && y < Height && _elements[x, y] != null && _elements[x, y].CanBeSelected)
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
            int[] columnCoords = new int[Width];
            int width = 0;
            for (int i = 0; i < Width; i++)
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
            int[] rowCoords = new int[Height];
            int height = 0;
            for (int i = 0; i < Height; i++)
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
            for (int i = 0; i < Height; i++)
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
            for (int i = 0; i < Height; i++)
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
