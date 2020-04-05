using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Models;
using TaskManager.Tools;
using TaskManager.Tools.Managers;
using TaskManager.Windows;
using Thread = System.Threading.Thread;

namespace TaskManager.ViewModels
{
    internal class ProcessListViewModel: BaseViewModel, ILoaderOwner
    {
        #region Fields

        private ObservableCollection<Processes> _processes;
        private Visibility _loaderVisibility = Visibility.Hidden;
        private bool _isControlEnabled = true;
        private Processes _select;

        private Thread _workingThread;
        private readonly CancellationToken _token;
        private readonly CancellationTokenSource _tokenSource;

        #region Commands
        #region Sort
        private RelayCommand<object> _sortById;
        private RelayCommand<object> _sortByName;
        private RelayCommand<object> _sortByIsActive;
        private RelayCommand<object> _sortByCPUPercents;
        private RelayCommand<object> _sortByRAMAmount;
        private RelayCommand<object> _sortByThreadsNumber;
        private RelayCommand<object> _sortByUser;
        private RelayCommand<object> _sortByFilepath;
        private RelayCommand<object> _sortByStartingTime;
        #endregion
        private RelayCommand<object> _endTask;
        private RelayCommand<object> _openFolder;
        private RelayCommand<object> _showThreads;
        private RelayCommand<object> _showModules;
        #endregion
        #endregion

        #region Properties


        public Visibility LoaderVisibility { get; set; }

        public bool IsControlEnabled
        {
            get => _isControlEnabled;
            set
            {
                _isControlEnabled = value;
                OnPropertyChanged();
            }
        }
        public Processes Select
        {
            get => _select;
            set
            {
                _select = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Processes> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        public RelayCommand<object> EndTask
        {
            get
            {
                return _endTask ?? (_endTask = new RelayCommand<object>(
                           EndTaskImp, o => CanExecuteCommand()));
            }
        }

        public RelayCommand<object> OpenFolder
        {
            get
            {
                return _openFolder ?? (_openFolder = new RelayCommand<object>(
                           FolderImp, o => CanExecuteCommand()));
            }
        }

        public RelayCommand<object> ShowThreads
        {
            get
            {
                return _showThreads ?? (_showThreads = new RelayCommand<object>(
                           ThreadsImp, o => CanExecuteCommand()));
            }
        }

        public RelayCommand<object> ShowModules
        {
            get
            {
                return _showModules ?? (_showModules = new RelayCommand<object>(
                           ModulesImp, o => CanExecuteCommand()));
            }
        }

        public RelayCommand<object> SortId
        {
            get
            {
                return _sortById ?? (_sortById = new RelayCommand<object>(o =>
                               SortImplementation(o, 0)));
            }
        }
        public RelayCommand<object> SortName
        {
            get
            {
                return _sortByName ?? (_sortByName = new RelayCommand<object>(o =>
                           SortImplementation(o, 1)));
            }
        }
        public RelayCommand<object> SortActive
        {
            get
            {
                return _sortByIsActive ?? (_sortByIsActive = new RelayCommand<object>(o =>
                           SortImplementation(o, 2)));
            }
        }
        public RelayCommand<object> SortCpu
        {
            get
            {
                return _sortByCPUPercents ?? (_sortByCPUPercents = new RelayCommand<object>(o =>
                           SortImplementation(o, 3)));
            }
        }
        public RelayCommand<object> SortRam
        {
            get
            {
                return _sortByRAMAmount ?? (_sortByRAMAmount = new RelayCommand<object>(o =>
                           SortImplementation(o, 4)));
            }
        }
        public RelayCommand<object> SortThreads
        {
            get
            {
                return _sortByThreadsNumber ?? (_sortByThreadsNumber = new RelayCommand<object>(o =>
                           SortImplementation(o, 5)));
            }
        }
        public RelayCommand<object> SortUser
        {
            get
            {
                return _sortByUser ?? (_sortByUser = new RelayCommand<object>(o =>
                           SortImplementation(o, 6)));
            }
        }
        public RelayCommand<object> SortFilepath
        {
            get
            {
                return _sortByFilepath ?? (_sortByFilepath = new RelayCommand<object>(o =>
                           SortImplementation(o, 7)));
            }
        }
        public RelayCommand<object> SortTime
        {
            get
            {
                return _sortByStartingTime ?? (_sortByStartingTime = new RelayCommand<object>(o =>
                           SortImplementation(o, 8)));
            }
        }

        #endregion


        #endregion

        private async void EndTaskImp(object obj)
        {
        
            LoaderManager.Instance.ShowLoader();
            await Task.Run(() => { 
            if (_select.IfAvailable())
            {
                
                Select?.Process?.Kill();
                StationManager.RemoveProcess(ref _select);
                StationManager.Update();
                Select = null;
                Processes = new ObservableCollection<Processes>(StationManager.Processes);
            }
            else
            {
                MessageBox.Show("NO ACCESS");
            }
            }, _token);
            LoaderManager.Instance.HideLoader();
    }
        
        private void FolderImp(object obj)
        {
            try
            {
                Process.Start("explorer.exe",
                    _select.Filepath.Substring(0,
                        _select.Filepath.LastIndexOf("\\", StringComparison.Ordinal)));
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
        }

        private void ModulesImp(object obj)
        {
      
            try
            {
                ShowModulesWindow modulesWindow = new ShowModulesWindow(ref _select);
                modulesWindow.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
      
        }

        private void ThreadsImp(object obj)
        {
            try
            {
                ShowThreadsWindow threadsWindow = new ShowThreadsWindow(ref _select);
                threadsWindow.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Error occurred while processing threads info");
            }
   
        }

        private async void SortImplementation(object obj, int param)
        {
           
            await Task.Run(() =>
            {
              
                try
                {
                    StationManager.Param = param;
                    StationManager.Update();
                    Processes = new ObservableCollection<Processes>(StationManager.Processes);
                   
                }
                catch (Exception)
                {
                    MessageBox.Show("Error occurred while accessing process data");
                }
            }, _token);
          
        }


        internal void ThreadStop()
        {
            _tokenSource.Cancel();
            _workingThread.Join(500);
            _workingThread.Abort();
            _workingThread = null;
        }

        internal ProcessListViewModel()
        {
            LoaderManager.Instance.Initialize(this);
            LoaderManager.Instance.ShowLoader();
            _processes = new ObservableCollection<Processes>(StationManager.Processes);
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartThread();
            StationManager.StopThreads += ThreadStop;
            LoaderManager.Instance.HideLoader();

        }

        private void StartThread()
        {
            _workingThread = new Thread(ThreadProcess);
            _workingThread.Start();
        }

        private void ThreadProcess()
        {
            while (!_token.IsCancellationRequested)
            {
                int temp = -1;
                if (Select != null)
                {
                    temp = Select.Id;
                }

                StationManager.Update();
                Processes = new ObservableCollection<Processes>(StationManager.Processes);
                foreach (var p in Processes)
                {
                    if (p.Id != temp) continue;
                    Select = p;
                    break;
                }
                Thread.Sleep(1000);
                
                if (_token.IsCancellationRequested)
                    break;
            }
        }

        private bool CanExecuteCommand()
        {
            return Select != null;
        }
    }
}
