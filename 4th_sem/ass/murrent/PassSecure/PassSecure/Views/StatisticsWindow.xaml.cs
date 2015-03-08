﻿#region File Header
// <copyright file="StatisticsWindow.xaml.cs" company="">
// Copyright (c) 2015 Mario Murrent. All rights reserved.
// </copyright>
// <summary>
// </summary>
// <author>Mario Murrent</author>
#endregion
namespace PassSecure.Views
{
    #region Usings

    using System.Windows;

    using PassSecure.Data;
    using PassSecure.Service;

    #endregion

    /// <summary>
    /// Interaction logic for StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        /// <summary>
        /// </summary>
        private readonly DataStore dataStore = SimpleContainer.Resolve<DataStore>();

        /// <summary>
        /// </summary>
        public StatisticsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void OkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
