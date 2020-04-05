using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace TaskManager.Models
{
    public class Processes
    {
        #region Fields
        private readonly Process _process;
        private readonly PerformanceCounter _counter;
 
        #endregion

        #region Properties
        public Process Process => _process;

        public int Id => _process.Id;

        public string Name => _process.ProcessName;

        public bool IsActive => _process.Responding;

        public float Cpu => _counter.NextValue() / Environment.ProcessorCount;

        // ReSharper disable once PossibleLossOfFraction
        public float Ram => _process.PrivateMemorySize64 / 1024;

        public int Threads => _process.Threads.Count;

        public ProcessModuleCollection Modules => _process.Modules;

        public ProcessThreadCollection ThreadsCollection => _process.Threads;

        internal Processes(Process process)
        {
            _process = process;
            _counter = new PerformanceCounter("Process", "% Processor Time", "_Total");
            _counter.NextValue();
        }

        public string Time
        {
            get
            {
                try
                {
                    return _process.StartTime.ToString(" dd/MM/yyyy HH:mm:ss");
                }
                catch (Exception)
                {
                    return "Access denied";
                }
            }
        }

        public string Filepath
        {
            get
            {
                try
                {

                    return _process.MainModule?.FileName;
                }
                catch (Exception)
                {
                    return "Denied";
                }

            }
        }

        public string User
        {
            get
            {
                try
                {
                    OpenProcessToken(_process.Handle, 8, out var processHandle);
                    var wi = new WindowsIdentity(processHandle);
                    var user = wi.Name;
                    return user;
                }
                catch
                {
                    return "Unknown user";
                }
            }
        }
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
        #endregion

        public bool IfAvailable()
        {
            return Time != "Access denied";
        }
    }
}
