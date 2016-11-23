using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace FilesCompare.Common
{
    /// <summary>
    /// ID:命令实例方法
    /// Describe:将命令实例到是否可执行方法
    /// Author:ybx
    /// Date:2016-11-21 15:37:06
    /// </summary>
    public class RelayCommand : System.Windows.Input.ICommand
    {
        private Action _act;
        private Func<bool> _func;

        public RelayCommand(Action act)
        {
            this._act = act;
        }
        public RelayCommand(Action act, Func<bool> func)
        {
            this._act = act;
            this._func = func;
        }

        public bool CanExecute(object parameter)
        {
            if (this._func != null)
                return this._func();
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this._act != null)
                this._act();
        }
    }
    public class RelayCommand<T> : System.Windows.Input.ICommand
    {
        private Action _act;
        private Func<bool> _func;
        private Action<T> _actT;
        private Func<T, bool> _funcT;

        public RelayCommand(Action<T> actT)
        {
            this._actT = actT;
        }
        public RelayCommand(Action<T> actT, Func<bool> func)
        {
            this._actT = actT;
            this._func = func;
        }
        public RelayCommand(Action act, Func<T, bool> funcT)
        {
            this._act = act;
            this._funcT = funcT;
        }
        public RelayCommand(Action<T> actT, Func<T, bool> funcT)
        {
            this._actT = actT;
            this._funcT = funcT;
        }

        public bool CanExecute(object parameter)
        {
            if (this._func != null)
                return this._func();
            if (this._funcT != null)
                return this._funcT((T)parameter);
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this._act != null)
                this._act();
            if (this._actT != null)
                this._actT((T)parameter);
        }
    }
}
