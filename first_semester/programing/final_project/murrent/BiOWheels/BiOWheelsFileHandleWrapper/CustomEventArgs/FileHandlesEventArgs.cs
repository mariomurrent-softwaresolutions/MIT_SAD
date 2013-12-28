﻿// *******************************************************
// * <copyright file="FileHandlesEventArgs.cs" company="MDMCoWorks">
// * Copyright (c) 2013 Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/
namespace BiOWheelsFileHandleWrapper.CustomEventArgs
{
    using System;

    /// <summary>
    /// The <see ref="FileHandlesEventArgs"/> class and its interaction logic 
    /// </summary>
    public class FileHandlesEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandlesEventArgs"/> class.
        /// </summary>
        /// <param name="hasFileHandles">
        /// The file handle count.
        /// </param>
        public FileHandlesEventArgs(bool hasFileHandles)
        {
            this.HasFileHandles = hasFileHandles;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a file has a file count or not.
        /// </summary>
        /// <value>
        /// The file handle count.
        /// </value>
        public bool HasFileHandles { get; set; }

        /// <summary>
        /// Gets or sets the full qualified filename.
        /// </summary>
        /// <value>
        /// The full qualified filename.
        /// </value>
        public string FullQualifiedFilename { get; set; }
    }
}