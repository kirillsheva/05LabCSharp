﻿using System;
using System.Windows;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Windows
{
  
    public partial class ShowModulesWindow : Window
    {
        public ShowModulesWindow(ref Processes proc)
        {
            InitializeComponent();
            DataContext = new ModuleViewModel(ref proc);
        }
    }
}
