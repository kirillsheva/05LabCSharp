using System;
using System.Windows;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Windows
{
    public partial class ShowThreadsWindow : Window
    {
        public ShowThreadsWindow(ref Processes proc)
        {
            InitializeComponent();
 
            DataContext = new ThreadViewModel(ref proc); ;
           
        }
    }
}
