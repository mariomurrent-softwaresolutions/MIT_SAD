﻿// *******************************************************
// * <copyright file="QueueManager.cs" company="MDMCoWorks">
// * Copyright (c) 2013 Mario Murrent. All rights reserved.
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
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using BiOWheelsFileHandleWrapper;
    using BiOWheelsFileHandleWrapper.CustomEventArgs;

    using BiOWheelsFileWatcher.CustomEventArgs;
    using BiOWheelsFileWatcher.Enums;
    using BiOWheelsFileWatcher.Helper;
    using BiOWheelsFileWatcher.Interfaces;

    /// <summary>
    /// Class representing the <see cref="QueueManager"/> and its interaction logic
    /// </summary>
    internal class QueueManager : IQueueManager
    {
        #region Private Fields

        /// <summary>
        /// Represents an instance of the queue holding <see cref="SyncItem"/> objects
        /// </summary>
        private ConcurrentQueue<SyncItem> syncItemQueue;

        /// <summary>
        /// A value indicating whether the worker is in progress or not
        /// </summary>
        private bool isWorkerInProgress;

        /// <summary>
        ///  Represents an instance of the <see cref="FileSystemManager"/> class
        /// </summary>
        private IFileSystemManager fileSystemManager;

        /// <summary>
        /// The file handle wrapper
        /// </summary>
        private IFileHandleWrapper fileHandleWrapper;

        /// <summary>
        /// A value indicating if an item can be added to the queue or not
        /// </summary>
        private bool canDequeueItems;

        /// <summary>
        /// Object used for locking
        /// </summary>
        private object enqueueLockOject = new object();

        /// <summary>
        /// Object used for locking
        /// </summary>
        private object queueItemLockObject = new object();

        /// <summary>
        /// The actual <see cref="SyncItem"/> object
        /// </summary>
        private SyncItem actualSyncItem;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManager"/> class
        /// </summary>
        /// <param name="fileSystemManager">
        /// The file system manager
        /// </param>
        /// <param name="fileHandleWrapper">
        /// The file handle wrapper.
        /// </param>
        internal QueueManager(IFileSystemManager fileSystemManager, IFileHandleWrapper fileHandleWrapper)
        {
            this.SyncItemQueue = new ConcurrentQueue<SyncItem>();
            this.FileSystemManager = fileSystemManager;
            this.CanDequeueItems = true;
            this.FileHandleWrapper = fileHandleWrapper;
            this.FileHandleWrapper.FileHandlesFound += this.FileHandleWrapperFileHandlesFound;
            this.FileHandleWrapper.FileHandlesError += this.FileHandleWrapperFileHandlesError;
        }

        #region Delegates

        /// <summary>
        /// Delegate for the <see cref="CaughtExceptionHandler"/> event
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="data">Data from the event</param>
        public delegate void CaughtExceptionHandler(object sender, CaughtExceptionEventArgs data);

        /// <summary>
        /// Delegate for the <see cref="ItemFinalizedHandler"/> event
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="data">Data from the event</param>
        public delegate void ItemFinalizedHandler(object sender, ItemFinalizedEventArgs data);

        #endregion

        #region Event Handler

        /// <summary>
        /// Event handler for catching an exception
        /// </summary>
        public event CaughtExceptionHandler CaughtException;

        /// <summary>
        /// Event handler for catching an exception
        /// </summary>
        public event ItemFinalizedHandler ItemFinalized;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public bool CanDequeueItems
        {
            get
            {
                return this.canDequeueItems;
            }

            set
            {
                this.canDequeueItems = value;
            }
        }

        /// <summary>
        /// Gets or sets the queue
        /// </summary>
        public ConcurrentQueue<SyncItem> SyncItemQueue
        {
            get
            {
                return this.syncItemQueue;
            }

            set
            {
                this.syncItemQueue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the worker is in progress or not
        /// </summary>
        internal bool IsWorkerInProgress
        {
            get
            {
                return this.isWorkerInProgress;
            }

            set
            {
                this.isWorkerInProgress = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="FileSystemManager"/> instance
        /// </summary>
        internal IFileSystemManager FileSystemManager
        {
            get
            {
                return this.fileSystemManager;
            }

            set
            {
                this.fileSystemManager = value;
            }
        }

        /// <summary>
        /// Gets or sets the file handle wrapper.
        /// </summary>
        /// <value>
        /// The file handle wrapper.
        /// </value>
        internal IFileHandleWrapper FileHandleWrapper
        {
            get
            {
                return this.fileHandleWrapper;
            }

            set
            {
                this.fileHandleWrapper = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual synchronize item.
        /// </summary>
        /// <value>
        /// The actual synchronize item.
        /// </value>
        internal SyncItem ActualSyncItem
        {
            get
            {
                return this.actualSyncItem;
            }

            set
            {
                this.actualSyncItem = value;
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void DoWork()
        {
            this.IsWorkerInProgress = true;

            Thread workerThread = new Thread(this.FinalizeQueue) { IsBackground = true };
            workerThread.Start();
        }

        /// <inheritdoc/>
        public void Enqueue(SyncItem item)
        {
            lock (this.enqueueLockOject)
            {
                this.CanDequeueItems = false;

                Task enqueueTask = Task.Factory.StartNew(() => this.SyncItemQueue.Enqueue(item));
                enqueueTask.Wait();

                this.CanDequeueItems = true;
            }
        }

        #region Event Methods

        /// <summary>
        /// Called when an exception is caught.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="data">
        /// The <see cref="CaughtExceptionEventArgs"/> instance containing the event data.
        /// </param>
        protected void OnCaughtException(object sender, CaughtExceptionEventArgs data)
        {
            if (this.CaughtException != null)
            {
                this.CaughtException(this, data);
            }
        }

        /// <summary>
        /// Called when an item is finalized.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="data">
        /// The <see cref="ItemFinalizedEventArgs"/> instance containing the event data.
        /// </param>
        protected void OnItemFinalized(object sender, ItemFinalizedEventArgs data)
        {
            if (this.ItemFinalized != null)
            {
                this.ItemFinalized(this, data);
            }
        }

        /// <summary>
        /// Occurs when an error occurred
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="data">
        /// The <see cref="FileHandlesErrorEventArgs"/> instance containing the event data.
        /// </param>
        protected void FileHandleWrapperFileHandlesError(object sender, FileHandlesErrorEventArgs data)
        {
            // TODO: Error handling
        }

        /// <summary>
        /// Occurs when the file handle wrapper has finished
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="data">
        /// The <see cref="FileHandlesEventArgs"/> instance containing the event data.
        /// </param>
        protected void FileHandleWrapperFileHandlesFound(object sender, FileHandlesEventArgs data)
        {
            bool hasHandlesOpen = data.HasFileHandles;

            if (!hasHandlesOpen)
            {
                this.FinalizeQueueItem(this.ActualSyncItem);
            }
        }

        #endregion

        /// <summary>
        /// Checks the open handles for a file.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void CheckOpenHandles(SyncItem item)
        {
            this.ActualSyncItem = item;

            if (item.FullQualifiedSourceFileName.IsDirectory())
            {
                this.FinalizeQueueItem(this.ActualSyncItem);
            }
            else
            {
                this.FileHandleWrapper.FindHandlesForFile(item.FullQualifiedSourceFileName);
            }
        }

        /// <summary>
        /// Finalizes the queue item.
        /// </summary>
        /// <param name="syncItem">
        /// The synchronize item.
        /// </param>
        private void FinalizeQueueItem(SyncItem syncItem)
        {
            try
            {
                switch (syncItem.FileAction)
                {
                    case FileAction.DELETE:
                        this.fileSystemManager.Delete(syncItem);
                        break;

                    case FileAction.COPY:
                        this.FileSystemManager.Copy(syncItem);
                        break;

                    case FileAction.RENAME:
                        this.FileSystemManager.Rename(syncItem);
                        break;
                }
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                this.OnCaughtException(
                    this, 
                    new CaughtExceptionEventArgs(
                        unauthorizedAccessException.GetType(), unauthorizedAccessException.Message));
            }
            catch (ArgumentException argumentException)
            {
                this.OnCaughtException(
                    this, new CaughtExceptionEventArgs(argumentException.GetType(), argumentException.Message));
            }
            catch (PathTooLongException pathTooLongException)
            {
                this.OnCaughtException(
                    this, new CaughtExceptionEventArgs(pathTooLongException.GetType(), pathTooLongException.Message));
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                this.OnCaughtException(
                    this, 
                    new CaughtExceptionEventArgs(
                        directoryNotFoundException.GetType(), directoryNotFoundException.Message));
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                this.OnCaughtException(
                    this, new CaughtExceptionEventArgs(fileNotFoundException.GetType(), fileNotFoundException.Message));
            }
            catch (IOException systemIOException)
            {
                syncItem.Retries++;
                this.Enqueue(syncItem);

                this.OnCaughtException(
                    this, new CaughtExceptionEventArgs(systemIOException.GetType(), systemIOException.Message));
            }
            catch (NotSupportedException notSupportedException)
            {
                this.OnCaughtException(
                    this, new CaughtExceptionEventArgs(notSupportedException.GetType(), notSupportedException.Message));
            }
            catch (AggregateException aggregateException)
            {
                this.OnCaughtException(
                    this, new CaughtExceptionEventArgs(aggregateException.GetType(), aggregateException.Message));
            }
        }

        /// <summary>
        /// Finalize Queue
        /// </summary>
        private void FinalizeQueue()
        {
            while (this.IsWorkerInProgress)
            {
                Thread.Sleep(100);

                if (this.CanDequeueItems)
                {
                    SyncItem item;

                    if (this.SyncItemQueue.TryDequeue(out item))
                    {
                        lock (this.queueItemLockObject)
                        {
                            this.FinalizeQueueItem(item);

                            // TODO: IMPROVE NOT WORKING GOOD ENOUGH
                            // this.CheckOpenHandles(item);
                        }
                    }
                }
            }
        }

        #endregion
    }
}