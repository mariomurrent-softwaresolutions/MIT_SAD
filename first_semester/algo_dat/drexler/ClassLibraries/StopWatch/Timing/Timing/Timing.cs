﻿//-----------------------------------------------------------------------
// <copyright file="Timing.cs" company="MD Development">
//     Copyright (c) MD Development. All rights reserved.
// </copyright>
// <author>Michael Drexler</author>
//-----------------------------------------------------------------------
namespace Timing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Timing
    {
        private TimeSpan startTime;
        private TimeSpan duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Timing"/> class
        /// </summary>
        public Timing()
        {
            this.startTime = new TimeSpan(0);
            this.duration = new TimeSpan(0);
        }

        /// <summary>
        /// Stop cpu time
        /// </summary>
        public void StopTime()
        {
            this.duration = Process.GetCurrentProcess().Threads[0].UserProcessorTime.Subtract(this.startTime);
        }

        /// <summary>
        /// Start cpu time
        /// </summary>
        public void StartTime()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            this.startTime = Process.GetCurrentProcess().Threads[0].UserProcessorTime;
        }

        /// <summary>
        /// Get the duration of the running process on the cpu
        /// </summary>
        /// <returns></returns>
        public TimeSpan Result()
        {
            return this.duration;
        }
    }
}
