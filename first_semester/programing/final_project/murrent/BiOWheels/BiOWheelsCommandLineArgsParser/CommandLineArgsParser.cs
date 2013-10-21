﻿// *******************************************************
// * <copyright file="CommandLineArgsParser.cs" company="MDMCoWorks">
// * Copyright (c) Mario Murrent. All rights reserved.
// * </copyright>
// * <summary>
// *
// * </summary>
// * <author>Mario Murrent</author>
// *******************************************************/
namespace BiOWheelsCommandLineArgsParser
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class CommandLineArgsParser : ICommandLineArgsParser
    {
        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private int optind;

        /// <summary>
        /// 
        /// </summary>
        private string nextarg = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private string optarg = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Optarg
        {
            get
            {
                return this.optarg;
            }
        }

        /// <summary>
        /// </summary>
        public int Optind
        {
            get
            {
                return this.optind;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the commandline args
        /// </summary>
        /// <param name="args">
        /// commandline args
        /// </param>
        /// <param name="options">
        /// accepted commandline arguments
        /// </param>
        /// <returns>
        /// </returns>
        public IList<char> Parse(string[] args, string options)
        {
            IList<char> includedArgs = new List<char>();

            char c;
            while ((c = this.Getopt(args.Length, args, options)) != '\0')
            {
                includedArgs.Add(c);
            }

            return includedArgs;
        }

        /// <summary>
        /// Get an valid option
        /// </summary>
        /// <param name="argc">
        /// </param>
        /// <param name="argv">
        /// </param>
        /// <param name="options">
        /// </param>
        /// <returns>
        /// </returns>
        protected char Getopt(int argc, string[] argv, string options)
        {
            this.optarg = string.Empty;

            if (argc < 0)
            {
                return '?';
            }

            if (this.optind == 0)
            {
                this.nextarg = string.Empty;
            }

            if (this.nextarg.Length == 0)
            {
                if (this.optind >= argc || argv[this.optind][0] != '-' || argv[this.optind].Length < 2)
                {
                    // no more options
                    this.optarg = string.Empty;
                    if (this.optind < argc)
                    {
                        this.optarg = argv[this.optind]; // return leftover arg
                    }

                    return '\0';
                }

                if (argv[this.optind] == "--")
                {
                    // 'end of options' flag
                    this.optind++;
                    this.optarg = string.Empty;
                    if (this.optind < argc)
                    {
                        this.optarg = argv[this.optind];
                    }

                    return '\0';
                }

                this.nextarg = string.Empty;
                if (this.optind < argc)
                {
                    this.nextarg = argv[this.optind];
                    this.nextarg = this.nextarg.Substring(1); // skip past -
                }

                this.optind++;
            }

            char c = nextarg[0]; // get option char
            this.nextarg = this.nextarg.Substring(1); // skip past option char
            int index = options.IndexOf(c); // check if this is valid option char

            if (index == -1 || c == ':')
            {
                return '?';
            }

            index++;
            if ((index < options.Length) && (options[index] == ':'))
            {
                // option takes an arg
                if (this.nextarg.Length > 0)
                {
                    this.optarg = this.nextarg;
                    this.nextarg = string.Empty;
                }
                else if (this.optind < argc)
                {
                    this.optarg = argv[this.optind];
                    this.optind++;
                }
                else
                {
                    return '?';
                }
            }

            return c;
        }

        #endregion
    }
}