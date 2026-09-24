// -----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using PrinterCapsViewer.Application;
using PrinterCapsViewer.ApplicationMain;
using System.Windows;

namespace PrinterCapsViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            MainWindow.Show();
        }
    }

}

