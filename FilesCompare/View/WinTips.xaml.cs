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
using FilesCompare.Common;

namespace FilesCompare.View
{
    /// <summary>
    /// WinWait.xaml 的交互逻辑
    /// </summary>
    public partial class WinTips : Window
    {
        public WinTips(string mess = "")
        {
            InitializeComponent();
            Message = mess;
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
                 this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Opacity = 1 - i * 0.1;
                }));
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

        /// <summary>
        /// 弹出窗口展示信息
        /// </summary>
        /// <param name="mess"></param>
        public static void Show(string mess)
        {
            WinTips win = new WinTips(mess);
            win.ShowDialogEx();
        }
    }
}
