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
using GridConsole.Constants;

namespace CsharpConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            Grid grid = ElementFactory.CreateGrid(i => i
                .Target(new DotNetConsole())
                .Size(2, 2)
            );

            grid.Add(0, 0, new Button("0, 0"));
            grid.Add(1, 0, new Button("1, 0"));
            grid.Add(0, 1, ElementFactory.CreateButton(i => i
                .Text("<-span->")
                .ColumnSpan(2)
            ));

            while(true)
            {
                grid.Render();
                grid.HandleInput();
            }
        }


        static void Main1(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            IConsole console = new DotNetConsole();
            
            Grid grid = ElementFactory.CreateGrid(i => i
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
                    element.OnEnterPressed += ButtonClick;
                }
            }

            while (true)
            {
                grid.Render();
                grid.HandleInput();
            }
        }


        static void Main2(string[] args)
        {
            IConsole console = new DotNetConsole();

            Grid grid = ElementFactory.CreateGrid(i => i
                .Target(console)
                .Size(4, 4)
            );
            
            grid.Add(0, 0, ElementFactory.CreateText(i => i
                    .Text("0,0")
                    .Colors(Color.DarkYellow, Color.DarkBlue)
            ));


            grid.Add(0, 1, new Button("0,1"));
            grid.Add(2, 0, new Button("2,0"));
            grid.Add(1, 1, new Button("1,1"));
            grid.Add(1, 2, new Button("1,2"));
            grid.Add(2, 1, new Button("2,1"));

            grid.Add(2, 2, ElementFactory.CreateButton(i => i
                    .Text("2.2")
                    .Colors(Color.DarkRed, Color.Blue)
                    .Highlight(Color.Green, Color.Magenta)
            ));

            grid.Add(1, 0, new Text("Text asdf"));
         
            Grid subGrid = ElementFactory.CreateGrid(i => i
                .Target(console)
                .Size(1, 4)
                .Text("Deploy application")
                .Colors(Color.Red, Color.Black)
                .Highlight(Color.Black, Color.Red)
            );

            subGrid.Add(0, 0, new Button("Debug"));
            subGrid.Add(0, 1, new Button("Keyuser"));
            subGrid.Add(0, 2, ElementFactory.CreateButton(i => i
                .Text("Release")
                .Pressed((sender, param) => { System.Diagnostics.Debug.WriteLine("Release button pressed. Param: " + (int)param); }, 12)
            ));

            grid.Add(0, 2, subGrid);

            while (true)
            {
                grid.Render();
                grid.HandleInput();
            }
        }

        static void ButtonClick(IBaseElement element, object parameter)
        {
            if (element is Button button)
            {
                System.Diagnostics.Debug.WriteLine($"Button {button.Text} press event!");
            }
        }
    }
}
