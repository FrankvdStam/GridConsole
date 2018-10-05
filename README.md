# GridConsole
Tiny lightweight library to add some simple GUI functionality to .NET console apps.


Example usage:

    class Program
    {
        static void Main(string[] args)
        {
            IConsole console = new DotNetConsole();
            Grid grid = new Grid(console, 4, 4);

            grid[0,0] = new Button("0,0");
            grid[0,1] = new Button("0,1");
            grid[2,0] = new Button("2,0");
            grid[1,1] = new Button("1,1");
            grid[1,2] = new Button("1,2");
            grid[2,1] = new Button("2,1");
            grid[2,2] = new Button("2,2");
            grid[1,0] = new Text("Text asdf");
            
            foreach (var element in grid.EnumerateElements())
            {
                if (element != null)
                {
                    element.EnterPressedEvent += ButtonClick;
                }
            }

            while (true)
            {
                grid.Render();
                grid.HandleInput();
            }
        }

        static void ButtonClick(ABaseElement element)
        {
            if (element is Button button)
            {
                System.Diagnostics.Debug.WriteLine($"Button {button.Text} press event!");
            }
        }
    }
