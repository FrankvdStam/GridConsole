using GridConsole.Constants;

namespace GridConsole
{
    public interface IConsole
    {
        // Set the cursor to the given coordinates.
        void Gotoxy(int x, int y);

        // Get the cursor's current coordinates.
        int Getx();

        // Get the cursor's current coordinates.
        int Gety();

        // Writes a character to the current cursor location and jumps 1 spot to the right or to a new line.
        void Write(char c);

        // Clears all characters from the terminal.
        void Clear();

        // Set the foreground color. Just return if you do not want to implement colors.
        void SetForegroundColor(Color color);

        // Set the background color. Just return if you do not want to implement colors.
        void SetBackgroundColor(Color color);

        // The width of the terminal/console
        int GetWidth();

        // The height of the terminal/console
        int GetHeight();

        // Get keypress
        Keys GetKey();
    }
}
