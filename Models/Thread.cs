using System;
using System.Diagnostics;

namespace TaskManager.Models
{
    internal class Thread
    {
        #region Fields

        private readonly ProcessThread _thread;

        #endregion

        #region Properties

        public int Id => _thread.Id;

        public ThreadState State => _thread.ThreadState;

        public DateTime StartingTime => _thread.StartTime;

        internal Thread(ProcessThread thread)
        {
            _thread = thread;
        }

        #endregion

    }
}
