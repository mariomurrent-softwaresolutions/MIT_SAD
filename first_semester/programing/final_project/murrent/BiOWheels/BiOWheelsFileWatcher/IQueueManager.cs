﻿namespace BiOWheelsFileWatcher
{
    /// <summary>
    /// Interface representing must implement methods
    /// </summary>
    internal interface IQueueManager
    {
        /// <summary>
        /// Start the <see cref="QueueManager"/>
        /// </summary>
        void DoWork();

        /// <summary>
        /// Add an item to the queue
        /// </summary>
        /// <param name="item">Item which will be added to the queue</param>
        void Enqueue(SyncItem item);
    }
}