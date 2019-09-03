# GridConsole
Tiny lightweight library to add some simple GUI functionality to .NET console apps.

new in 1.1.0: 
Grid is now a base element that can be added to an existing Grid. It will act as a button, displaying it's contents after being clicked. This allows easy submenu's.
EnterPressedEvent now returns a parameter object, default null. Buttons can be passed an object in their constructor, this object will be passed in the event, again default null.


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
            
            Grid subGrid = new Grid(console, "Deploy application", grid, 1, 3);
            subGrid[0, 0] = new Button("Debug");
            subGrid[0, 1] = new Button("Keyuser");
            subGrid[0, 2] = new Button("Release");

            grid[0, 2] = subGrid;

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

        static void ButtonClick(ABaseElement element, object parameter)
        {
            if (element is Button button)
            {
                System.Diagnostics.Debug.WriteLine($"Button {button.Text} press event!");
            }
        }
    }
