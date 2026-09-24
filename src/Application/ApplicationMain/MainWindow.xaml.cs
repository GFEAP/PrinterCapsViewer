// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="GFEAP GmbH">
// Copyright (c) GFEAP GmbH, Bensheim, Germany. All rights reserved.
// Author: Michael Friedl
// Licensed under the MIT License. See LICENSE file in the project root
// for full license information.
// Project: PrinterCapsViewer
// </copyright>
// -----------------------------------------------------------------------
using PrinterCapsViewer.ApplicationMain;
using System.Windows;

namespace PrinterCapsViewer.Application
{
    public partial class MainWindow : Window 
    {   
        public MainWindow()
        {
            InitializeComponent();
            DataContextChanged += MainWindow_DataContextChanged;    
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is MainViewModel vm)
            {
                vm.SetCloseWindowsAction(this.Close);
            }
        }
    }
}
