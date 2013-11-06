﻿// *******************************************************
// * <copyright file="Visualizer.cs" company="MDMCoWorks">
// * Copyright (c) 2013 Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/
namespace BiOWheelsVisualizer
{
    using System;

    /// <summary>
    /// </summary>
    public class Visualizer : IVisualizer
    {
        #region Methods

        /// <inheritdoc/>
        public void GetMenu()
        {
        }

        /// <inheritdoc/>
        public void WriteLog(string entry)
        {
        }

        /// <inheritdoc/>
        public void WriteLine(string text)
        {
            Console.WriteLine("**************");
            Console.WriteLine(text);
        }

        /// <inheritdoc/>
        public void WriteText(string text)
        {
            Console.Write(text);
        }

        /// <inheritdoc/>
        public void GetHelp()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("\t -h \t\t Shows the help");
        }

        #endregion
    }
}