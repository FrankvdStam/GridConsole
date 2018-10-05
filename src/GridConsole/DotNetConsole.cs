using System;
using GridConsole.Constants;

namespace GridConsole
{
    public class DotNetConsole : IConsole
    {
        public DotNetConsole()
        {
            Console.CursorVisible = false;
        }

        public void Clear()
        {
            Console.Clear();
        }

        public int GetHeight()
        {
            return Console.BufferHeight;
        }

        public int GetWidth()
        {
            return Console.BufferWidth;
        }

        public int Getx()
        {
            return Console.CursorLeft;
        }

        public int Gety()
        {
            return Console.CursorTop;
        }

        public void Gotoxy(int x, int y)
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;
        }

        public void SetBackgroundColor(Color color)
        {
            //Cast color enum to ConsoleColor enum (portability)
            Console.BackgroundColor = (ConsoleColor)(int)color;
        }

        public void SetForegroundColor(Color color)
        {
            //Cast color enum to ConsoleColor enum (portability)
            Console.ForegroundColor = (ConsoleColor)(int)color;
        }

        public Keys GetKey()
        {
            Keys k = Keys.Unknown;
            ConsoleKeyInfo info = Console.ReadKey(true);
            if (info.Key == ConsoleKey.LeftArrow)
            {
                k = Keys.LeftArrow;
            }
            if (info.Key == ConsoleKey.RightArrow)
            {
                k = Keys.RightArrow;
            }
            if (info.Key == ConsoleKey.UpArrow)
            {
                k = Keys.UpArrow;
            }
            if (info.Key == ConsoleKey.DownArrow)
            {
                k = Keys.DownArrow;
            }
            if (info.Key == ConsoleKey.Escape)
            {
                k = Keys.Escape;
            }
            if (info.Key == ConsoleKey.Enter)
            {
                k = Keys.Enter;
            }
            return k;
        }

        public void Write(char c)
        {
            Console.Write(c);
        }
    }
}
