﻿// *******************************************************
// * <copyright file="CaughtExceptionEventArgs.cs" company="MDMCoWorks">
// * Copyright (c) 2013 Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/
namespace BiOWheelsFileWatcher.CustomEventArgs
{
    using System;

    /// <summary>
    ///  Class representing the <see cref="CaughtExceptionEventArgs"/>
    /// </summary>
    public class CaughtExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaughtExceptionEventArgs"/> class
        /// </summary>
        /// <param name="exceptionType">
        /// Type of the exception.
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message.
        /// </param>
        public CaughtExceptionEventArgs(Type exceptionType, string exceptionMessage)
        {
            this.ExceptionType = exceptionType;
            this.ExceptionMessage = exceptionMessage;
        }

        /// <summary>
        /// Gets or sets the type of the exception
        /// </summary>
        public Type ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the message of the exception
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets or sets the custom exception text
        /// </summary>
        public string CustomExceptionText { get; set; }

        /// <summary>
        /// Gets the formatted exception
        /// </summary>
        /// <returns>The formatted exception</returns>
        public string GetFormattedException()
        {
            return this.CustomExceptionText + " -- exception: " + ExceptionType + " -- message: " + this.ExceptionMessage;
        }
    }
}