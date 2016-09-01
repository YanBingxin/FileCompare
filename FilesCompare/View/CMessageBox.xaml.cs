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
    public partial class CMessageBox : Window
    {
        public CMessageBox(string mess = "")
        {
            InitializeComponent();
            _message = mess;
        }

        private string _message;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message
        {
            get { return _message; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 弹出窗口展示信息
        /// </summary>
        /// <param name="mess"></param>
        public static void Show(string mess)
        {
            CMessageBox win = new CMessageBox(mess);
            win.ShowDialogEx();
        }
    }
}
