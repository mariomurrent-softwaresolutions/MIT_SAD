﻿#region File Header
// <copyright file="UserTraining.cs" company="">
// Copyright (c) 2015 Mario Murrent. All rights reserved.
// </copyright>
// <summary>
// </summary>
// <author>Mario Murrent</author>
#endregion
namespace PassSecure.Models
{
    #region Usings

    using System.Collections.Generic;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    /// </summary>
    public class UserTraining
    {
        public UserTraining()
        {
            Trainings = new List<TrainingEntry>();
        }

        /// <summary>
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        public List<TrainingEntry> Trainings { get; set; }

        /// <summary>
        /// </summary>
        public void Analyze()
        {
            Parallel.ForEach(Trainings, (entry, state) => entry.Analyze());
        }
    }
}
