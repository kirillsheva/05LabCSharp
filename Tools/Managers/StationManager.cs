using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using TaskManager.Models;

namespace TaskManager.Tools
{
    internal static class StationManager
    {
        #region Fields
        public static event Action StopThreads;
        private static List<Processes> _processes;
        #endregion

        #region Properties
        internal static List<Processes> Processes => _processes;
        internal static int Param { get; set; }
        #endregion

        internal static void Init()
        {
            Param = 1;
            _processes = new List<Processes>();
            Update();
        }


        internal static void Sort()
        {            
            switch (Param)
            {
                case 0:
                    _processes = (from u in _processes
                                      orderby u.Id
                                      select u).ToList();
                    break;

                case 1:
                    _processes = (from u in _processes
                                      orderby u.Name
                                      select u).ToList();
                    break;
                case 2:
                    _processes = (from u in _processes
                                      orderby u.IsActive
                                      select u).ToList();
                    break;
                case 3:
                    _processes = (from u in _processes
                                      orderby u.Cpu descending 
                                      select u).ToList();
                    break;
                case 4:
                    _processes = (from u in _processes
                                      orderby u.Ram descending 
                                      select u).ToList();
                    break;
                case 5:
                    _processes = (from u in _processes
                                      orderby u.Threads descending 
                                      select u).ToList();
                    break;
                case 6:
                    _processes = (from u in _processes
                                      orderby u.User descending 
                                      select u).ToList();
                    break;
                case 7:
                    _processes= (from u in _processes
                                      orderby u.Filepath
                                      select u).ToList();
                    break;
                default:
                    _processes = (from u in _processes
                                      orderby u.Time descending 
                                      select u).ToList();
                    break;
            }
        }

        internal static void RemoveProcess(ref Processes p)
        {
            _processes.Remove(p);
        }

        internal static void Update()
        {
            AddProcesses();
            Sort();
        }
        private static void AddProcesses()
        {
            foreach (var item in Process.GetProcesses())
            {
                if (item == null) continue;
                if(!SameProcess(item.Id))
                    _processes.Add(new Processes(item));
            }
        }

        private static bool SameProcess(int processId)
        {
            return _processes.Any(item => processId == item.Id);
        }

        internal static void CloseApp()
        {
            StopThreads?.Invoke();
        }
    }
}
