﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GridConsole;
using GridConsole.Elements;

namespace CsharpConsole
{
    class Program
    {
        static void Main1(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            Grid gasd = GridFactory.Create(i => i.Size(1, 1).Target(new DotNetConsole()));

            IConsole console = new DotNetConsole();
            
            Grid grid = GridFactory.Create(i => i
                .Target(console)
                .Size(1, 4)
            );

            grid.Add(0, 0, new Text("Publish channel:"));
            grid.Add(0, 1, new Button("Debug"));
            grid.Add(0, 2, new Button("Keyuser"));
            grid.Add(0, 3, new Button("Release"));

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


        static void Main(string[] args)
        {
            IConsole console = new DotNetConsole();

            Grid grid = GridFactory.Create(i => i
                .Target(console)
                .Size(4, 4)
            );

            grid.Add(0, 0, new Button("0,0"));
            grid.Add(0, 1, new Button("0,1"));
            grid.Add(2, 0, new Button("2,0"));
            grid.Add(1, 1, new Button("1,1"));
            grid.Add(1, 2, new Button("1,2"));
            grid.Add(2, 1, new Button("2,1"));
            grid.Add(2, 2, new Button("2,2"));
            grid.Add(1, 0, new Text("Text asdf"));
         
            Grid subGrid = GridFactory.Create(i => i
                .Target(console)
                .Size(1, 4)
                //.Parent(grid)
                .Text("Deploy application")
            );

            subGrid.Add(0, 0, new Button("Debug"));
            subGrid.Add(0, 1, new Button("Keyuser"));
            subGrid.Add(0, 2, new Button("Release"));

            //grid[0, 2] = subGrid;
            grid.Add(0, 2, subGrid);

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
}
