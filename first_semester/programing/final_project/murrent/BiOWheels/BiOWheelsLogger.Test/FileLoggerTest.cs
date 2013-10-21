﻿// *******************************************************
// * <copyright file="FileLoggerTest.cs" company="MDMCoWorks">
// * Copyright (c) Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/

using System.Threading;

namespace BiOWheelsLogger.Test
{
    using System;
    using System.IO;
    using System.Linq;

    using NUnit.Framework;

    /// <summary>
    /// Test class for the <see cref="FileLogger"/> class of the BiOWheelsLogger module
    /// </summary>
    [TestFixture]
    public class FileLoggerTest
    {
        /// <summary>
        /// File size in MB
        /// </summary>
        private const double FileSize = 0.5;

        /// <summary>
        /// The file logger used for testing
        /// </summary>
        private FileLogger logger;

        /// <summary>
        /// Set up test environment
        /// </summary>
        [SetUp]
        public void Init()
        {
            this.logger = new FileLogger();
            this.logger.SetIsEnabled<FileLogger>(true);
            this.logger.SetFileSize<FileLogger>(FileSize);
            this.logger.Init();
        }

        /// <summary>
        /// Test method for testing the Log method of the <see cref="FileLogger"/> class
        /// </summary>
        [TestCase]
        public void LogTest()
        {
            for (int i = 0; i < 25000; i++)
            {
                this.logger.Log("test logging" + i, MessageType.DEBUG);
            }

            Assert.IsTrue(Directory.Exists("log"));
            Assert.IsNotEmpty(Directory.GetFiles("log"));

            string[] files = Directory.GetFiles("log");

            for (int i = 0; i < files.Count(); i++)
            {
                Stream actualFileStream = new FileStream(files[i], FileMode.Open);
                double length = Math.Round((actualFileStream.Length / 1024f) / 1024f, 1, MidpointRounding.AwayFromZero);

                Assert.IsTrue(length <= FileSize);
            }
        }

        /// <summary>
        /// Test method for testing the Log method of the <see cref="FileLogger"/> class
        /// </summary>
        [TestCase]
        public void LogTestWithDifferentCalls()
        {
            for (int i = 0; i < 25000; i++)
            {
                this.logger.Log("test logging" + i, MessageType.DEBUG);

                Random rnd = new Random();
                Thread.Sleep(rnd.Next(1, 600));
            }

            Assert.IsTrue(Directory.Exists("log"));
            Assert.IsNotEmpty(Directory.GetFiles("log"));

            string[] files = Directory.GetFiles("log");

            for (int i = 0; i < files.Count(); i++)
            {
                Stream actualFileStream = new FileStream(files[i], FileMode.Open);
                double length = Math.Round((actualFileStream.Length / 1024f) / 1024f, 1, MidpointRounding.AwayFromZero);

                Assert.IsTrue(length <= FileSize);
            }
        }
    }
}