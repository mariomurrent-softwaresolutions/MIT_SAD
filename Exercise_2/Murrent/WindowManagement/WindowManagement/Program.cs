﻿// *******************************************************
// * <copyright file="Program.cs" company="MDMCoWorks">
// * Copyright (c) Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/
namespace WindowManagement
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the Application which contains the entry point of the application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// WindowManager holds all windows
        /// </summary>
        private static readonly WindowManager WindowManager = new WindowManager();

        /// <summary>
        /// Entry point for the program
        /// </summary>
        private static void Main()
        {
            Console.CursorVisible = false;

            WindowManager.Windows = new List<Window>
                {
                    new WindowWithSigns { Content = "Window 1", ForegroundColor = ConsoleColor.Yellow, BackgroundColor = ConsoleColor.DarkBlue, Title = "Window Blue/Yellow", Left = 0, Top = 0, Width = 10, Height = 10 },
                    new WindowWithSigns { Content = "Window 2", ForegroundColor = ConsoleColor.Cyan, BackgroundColor = ConsoleColor.Red, Title = "Window Cyan/Red", Left = 0, Top = 0, Width = 15, Height = 10 },
                    new WindowWithTextContent { Content = "Window 3", ForegroundColor = ConsoleColor.DarkRed, BackgroundColor = ConsoleColor.Green, Title = "Window Red/Green", Left = 2, Top = 2, Width = 15, Height = 15 },
                    new WindowWithTextContent { Content = "Window 4", ForegroundColor = ConsoleColor.DarkMagenta, BackgroundColor = ConsoleColor.DarkGray, Title = "Window Magenta/Gray", Left = 1, Top = 1, Width = 20, Height = 20 }
                };

            WindowManager.DrawAll();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        WindowManager.DrawNext();
                        break;
                    case ConsoleKey.DownArrow:
                        WindowManager.DrawPrevious();
                        break;

                    case ConsoleKey.X:
                        Environment.Exit(0);
                        break;

                    default:
                        WindowManager.DrawAll();

                        // Console.WriteLine("Supported keys: 'ArrowUp' for next window and 'ArrowDown' for previous window and 'x' to close the application");
                        break;
                }
            }
        }
    }
}