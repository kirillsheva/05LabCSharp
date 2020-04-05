using System;
using System.Diagnostics;
namespace TaskManager.Models
{
    internal class Module
    {
        #region Fields
        private readonly ProcessModule _module;
        #endregion

        #region Properties
        public string Name => _module.ModuleName;
        public string Filepath => _module.FileName;

        internal Module(ProcessModule module)
        {
            _module = module;
        }

        #endregion
    }
}
