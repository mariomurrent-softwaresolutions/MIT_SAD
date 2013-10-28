﻿// *******************************************************
// * <copyright file="FileWatcher.cs" company="MDMCoWorks">
// * Copyright (c) Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BiOWheelsFileWatcher.Test")]

namespace BiOWheelsFileWatcher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using CustomEventArgs;
    using CustomExceptions;

    /// <summary>
    /// Class representing the <see cref="FileWatcher"/> and its interaction logic
    /// </summary>
    public class FileWatcher : IFileWatcher
    {
        #region Private Fields

        /// <summary>
        /// Manager for the queue
        /// </summary>
        private readonly IQueueManager queueManager;

        /// <summary>
        /// List of mappings representing <see cref="DirectoryMapping"/>
        /// </summary>
        private IEnumerable<DirectoryMapping> mappings;

        /// <summary>
        /// Field representing the status of the <see cref="FileWatcher"/>
        /// </summary>
        private bool isWorkerInProgress;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher"/> class
        /// </summary>
        public FileWatcher()
        {
            this.Mappings = new List<DirectoryMapping>();
            this.queueManager = new QueueManager();
        }

        #region Delegates

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void CaughtExceptionHandler(object sender, CaughtExceptionEventArgs data);

        /// <summary>
        /// Delegate for the <see cref="ProgressUpdateHandler"/> event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void ProgressUpdateHandler(object sender, UpdateProgressEventArgs data);

        #endregion

        #region Event Handler
        /// <summary>
        /// Event handler for catching an exception
        /// </summary>
        public event CaughtExceptionHandler CaughtException;

        /// <summary>
        /// Event handler for updating the progress
        /// </summary>
        public event ProgressUpdateHandler ProgressUpdate;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="QueueManager"/>
        /// </summary>
        internal QueueManager QueueManager
        {
            get
            {
                return this.queueManager as QueueManager;
            }
        }

        /// <summary>
        /// Gets or sets the list of mappings
        /// </summary>
        internal IEnumerable<DirectoryMapping> Mappings
        {
            get
            {
                return this.mappings;
            }

            set
            {
                this.mappings = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="FileWatcher"/> is in progress or not
        /// </summary>
        internal bool IsWorkerInProgress
        {
            get
            {
                return this.isWorkerInProgress;
            }

            private set
            {
                this.isWorkerInProgress = value;
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void Init()
        {
            this.IsWorkerInProgress = true;

            foreach (DirectoryMapping mapping in this.Mappings)
            {
                Thread backgroundWatcherThread = new Thread(this.WatchDirectory);
                backgroundWatcherThread.Start(mapping);
            }

            this.QueueManager.DoWork();
        }

        /// <inheritdoc/>
        public void SetSourceDirectories(IEnumerable<DirectoryMapping> directoryMappings)
        {
            this.mappings = directoryMappings;
        }

        #region Event Methods

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void FileSystemWatcherChanged(object sender, FileSystemEventArgs e)
        {
            BiOWheelsFileSystemWatcher watcher = this.GetFileSystemWatcher(sender);

            if (watcher != null)
            {
                this.AddQueueItem(watcher.Destinations, e.FullPath, FileAction.DIFF);
                this.OnProgressUpdate(this, new UpdateProgressEventArgs("File --" + e.Name + "-- has changed."));
                this.OnProgressUpdate(
                    this, new UpdateProgressEventArgs("Added job to queue for comparing --" + e.Name + "--"));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void FileSystemWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            BiOWheelsFileSystemWatcher watcher = this.GetFileSystemWatcher(sender);

            if (watcher != null)
            {
                this.AddQueueItem(watcher.Destinations, e.FullPath, FileAction.DELETE);
                this.OnProgressUpdate(this, new UpdateProgressEventArgs("File --" + e.Name + "-- has been deleted."));
                this.OnProgressUpdate(
                    this, new UpdateProgressEventArgs("Added job to queue for deleting --" + e.Name + "--"));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void FileSystemWatcherCreated(object sender, FileSystemEventArgs e)
        {
            BiOWheelsFileSystemWatcher watcher = this.GetFileSystemWatcher(sender);

            if (watcher != null)
            {
                this.AddQueueItem(watcher.Destinations, e.FullPath, FileAction.CREATE);
                this.OnProgressUpdate(this, new UpdateProgressEventArgs("File --" + e.Name + "-- has been created."));
                this.OnProgressUpdate(
                    this, new UpdateProgressEventArgs("Added job to queue for copying --" + e.Name + "--"));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void FileSystemWatcherRenamed(object sender, RenamedEventArgs e)
        {
            BiOWheelsFileSystemWatcher watcher = this.GetFileSystemWatcher(sender);

            if (watcher != null)
            {
                this.AddQueueItem(watcher.Destinations, e.FullPath, FileAction.COPY);
                this.OnProgressUpdate(
                    this, new UpdateProgressEventArgs("File --" + e.OldName + " has been renamed to --" + e.Name));
                this.OnProgressUpdate(
                    this,
                    new UpdateProgressEventArgs(
                        "Added job to queue for renaming --" + e.OldName + "-- to --" + e.Name + "--"));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void FileSystemWatcherError(object sender, ErrorEventArgs e)
        {
            BiOWheelsFileSystemWatcher watcher = this.GetFileSystemWatcher(sender);

            if (watcher != null)
            {
                this.OnCaughtException(
                    this, new CaughtExceptionEventArgs(e.GetException().GetType(), e.GetException().Message));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void FileSystemWatcherDisposed(object sender, EventArgs e)
        {
            BiOWheelsFileSystemWatcher watcher = this.GetFileSystemWatcher(sender);

            if (watcher != null)
            {
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="data">
        /// </param>
        protected void OnProgressUpdate(object sender, UpdateProgressEventArgs data)
        {
            if (this.ProgressUpdate != null)
            {
                this.ProgressUpdate(this, data);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="data">
        /// </param>
        protected void OnCaughtException(object sender, CaughtExceptionEventArgs data)
        {
            if (this.CaughtException != null)
            {
                this.CaughtException(this, data);
            }
        }

        #endregion

        /// <summary>
        /// Method for watching a specific directory - will be executed in a new thread
        /// </summary>
        /// <param name="mappingInfo">
        /// Object containing the <see cref="DirectoryMapping"/> information
        /// </param>
        private void WatchDirectory(object mappingInfo)
        {
            if (mappingInfo != null)
            {
                if (mappingInfo.GetType() == typeof(DirectoryMapping))
                {
                    try
                    {
                        BiOWheelsFileSystemWatcher fileSystemWatcher =
                            new BiOWheelsFileSystemWatcher(((DirectoryMapping)mappingInfo).SorceDirectory)
                                {
                                    IncludeSubdirectories = ((DirectoryMapping)mappingInfo).Recursive, 
                                    Destinations = ((DirectoryMapping)mappingInfo).DestinationDirectories
                                };
                        fileSystemWatcher.Changed += this.FileSystemWatcherChanged;
                        fileSystemWatcher.Created += this.FileSystemWatcherCreated;
                        fileSystemWatcher.Deleted += this.FileSystemWatcherDeleted;
                        fileSystemWatcher.Error += this.FileSystemWatcherError;
                        fileSystemWatcher.Renamed += this.FileSystemWatcherRenamed;
                        fileSystemWatcher.Disposed += this.FileSystemWatcherDisposed;
                        fileSystemWatcher.EnableRaisingEvents = true;
                    }
                    catch (PathTooLongException pathTooLongException)
                    {
                        this.OnCaughtException(
                            this, 
                            new CaughtExceptionEventArgs(pathTooLongException.GetType(), pathTooLongException.Message));
                    }
                    catch (ArgumentException argumentException)
                    {
                        this.OnCaughtException(
                            this, new CaughtExceptionEventArgs(argumentException.GetType(), argumentException.Message));
                    }
                }
                else
                {
                    this.OnCaughtException(
                        this, 
                        new CaughtExceptionEventArgs(typeof(MappingInvalidException), "Mapping information is invalid"));
                }
            }
            else
            {
                this.OnCaughtException(
                    this, new CaughtExceptionEventArgs(typeof(MappingNullException), "Mapping information is null"));
            }
        }

        /// <summary>
        /// Get the <see cref="BiOWheelsFileSystemWatcher"/> from the object
        /// </summary>
        /// <param name="sender">
        /// Object from the event
        /// </param>
        /// <returns>
        /// The instance of the <see cref="BiOWheelsFileSystemWatcher"/>
        /// </returns>
        private BiOWheelsFileSystemWatcher GetFileSystemWatcher(object sender)
        {
            BiOWheelsFileSystemWatcher watcher = sender as BiOWheelsFileSystemWatcher;

            if (watcher != null)
            {
                return watcher;
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="destinations">
        /// </param>
        /// <param name="filename">
        /// </param>
        /// <param name="fileAction">
        /// </param>
        private void AddQueueItem(IEnumerable<string> destinations, string filename, FileAction fileAction)
        {
            foreach (string folder in destinations)
            {
                SyncItem item = new SyncItem(folder, filename, fileAction);

                this.QueueManager.Enqueue(item);
            }
        }

        #endregion
    }
}