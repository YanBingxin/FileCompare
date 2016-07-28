using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FilesCompare.View
{
    /// <summary>
    /// WinWait.xaml 的交互逻辑
    /// </summary>
    public partial class WinHelp : Window
    {
        public WinHelp(string mess = "")
        {
            InitializeComponent();
            Message = mess;
            if (Message.Length > 12)
            {
                this.txtMess.FontSize = 18 - Message.Length / 8;
            }
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += bg_DoWork;
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;
            bg.RunWorkerAsync();
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this != null)
            {
                this.Close();
            }
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(100);
                this.Opacity = 1 - i * 0.1;
            }
        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                txtMess.Text = _message;
            }
        }
    }
}
