using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TaskManager.Models;
using TaskManager.Tools;

namespace TaskManager.ViewModels
{
    internal class ThreadViewModel : BaseViewModel
    {
        private ObservableCollection<Thread> _threads;
        public string ProcessName { get; }

        public ObservableCollection<Thread> Threads
        {
            get => _threads;
            private set
            {
                _threads= value;
                OnPropertyChanged();
            }
        }

        internal ThreadViewModel(ref Processes process)
        {
            Threads = new ObservableCollection<Thread>();
            ObservableCollection<Thread> threadColl = new ObservableCollection<Thread>();
            ProcessName = process.Name;
            foreach (ProcessThread thread in process.ThreadsCollection)
            {
                threadColl.Add(new Thread(thread));
            }
            Threads = threadColl;
        }
    }
}
