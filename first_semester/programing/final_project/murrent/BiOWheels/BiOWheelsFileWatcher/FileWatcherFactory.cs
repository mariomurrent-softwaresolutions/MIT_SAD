﻿// *******************************************************
// * <copyright file="FileWatcherFactory.cs" company="MDMCoWorks">
// * Copyright (c) 2013 Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/
namespace BiOWheelsFileWatcher
{
    using System.Collections.Generic;

    using BiOWheelsFileHandleWrapper;

    using BiOWheelsFileWatcher.Interfaces;

    /// <summary>
    ///  Class representing the <see cref="FileWatcherFactory"/>
    /// </summary>
    public class FileWatcherFactory
    {
        /// <summary>
        /// Creates the file system watcher.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="BiOWheelsFileSystemWatcher"/> class
        /// </returns>
        public static BiOWheelsFileSystemWatcher CreateFileSystemWatcher(string path)
        {
            return new BiOWheelsFileSystemWatcher(path);
        }

        /// <summary>
        /// Creates the file system watcher.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="recursive">
        /// if set to <c>true</c> [recursive].
        /// </param>
        /// <param name="destinationDirectories">
        /// The destination directories.
        /// </param>
        /// <param name="excludedDirectories">
        /// The excluded directories.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="BiOWheelsFileSystemWatcher"/> class
        /// </returns>
        public static BiOWheelsFileSystemWatcher CreateFileSystemWatcher(
            string path, bool recursive, List<string> destinationDirectories, List<string> excludedDirectories)
        {
            return new BiOWheelsFileSystemWatcher(path)
                {
                    IncludeSubdirectories = recursive, 
                    Destinations = destinationDirectories, 
                    ExcludedDirectories = excludedDirectories
                };
        }

        /// <summary>
        /// Creates the <see cref="FileWatcher"/>.
        /// </summary>
        /// <param name="queueManager">
        /// The queue manager.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="FileWatcher"/> class
        /// </returns>
        public static IFileWatcher CreateFileWatcher(IQueueManager queueManager)
        {
            return new FileWatcher(queueManager);
        }

        /// <summary>
        /// Creates the file comparator.
        /// </summary>
        /// <param name="blockSize">
        /// Size of the block.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="FileComparator"/> class
        /// </returns>
        public static IFileComparator CreateFileComparator(long blockSize)
        {
            return new FileComparator(blockSize);
        }

        /// <summary>
        /// Creates the file system manager.
        /// </summary>
        /// <param name="fileComparator">
        /// The file comparator.
        /// </param>
        /// <param name="directoryVolumeComparator">
        /// The directory volume comparator.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="FileSystemManager"/> class
        /// </returns>
        public static IFileSystemManager CreateFileSystemManager(
            IFileComparator fileComparator, IDirectoryVolumeComparator directoryVolumeComparator)
        {
            return new FileSystemManager(fileComparator, directoryVolumeComparator);
        }

        /// <summary>
        /// Creates the queue manager.
        /// </summary>
        /// <param name="fileSystemManager">
        /// The file system manager.
        /// </param>
        /// <param name="fileHandleWrapper">
        /// The file handle wrapper.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="QueueManager"/> class
        /// </returns>
        public static IQueueManager CreateQueueManager(
            IFileSystemManager fileSystemManager, IFileHandleWrapper fileHandleWrapper)
        {
            return new QueueManager(fileSystemManager, fileHandleWrapper);
        }

        /// <summary>
        /// Creates the directory volume comparator.
        /// </summary>
        /// <returns>An instance of the <see cref="DirectoryVolumeComparator"/> class</returns>
        public static IDirectoryVolumeComparator CreateDirectoryVolumenComparator()
        {
            return new DirectoryVolumeComparator();
        }
    }
}