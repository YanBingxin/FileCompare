using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class WinWait : Window
    {
        public WinWait()
        {
            InitializeComponent();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
