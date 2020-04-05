using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TaskManager.Models;
using TaskManager.Tools;

namespace TaskManager.ViewModels
{
    internal class ModuleViewModel: BaseViewModel
    {
        private ObservableCollection<Module> _modules;
        public string ProcessName{ get; }

        public ObservableCollection<Module> Modules
        {
            get => _modules;
            private set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }
        internal ModuleViewModel(ref Processes process)
        {
            Modules = new ObservableCollection<Module>();
            ObservableCollection<Module> moduleColl = new ObservableCollection<Module>();
            ProcessName = process.Name;
            foreach (ProcessModule module in process.Modules)
            {
                moduleColl.Add(new Module(module));
            }
            Modules = moduleColl;
        }
    }
}
