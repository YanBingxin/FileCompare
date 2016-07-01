using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilesCompare.IView
{
    public interface IMainWindowView
    {
        object DataContext { set; get; }
        double Width { set; get; }
        double Height { get; set; }
        void Close();
        void Show();
        //void DoWork(Action action);
    }
}
