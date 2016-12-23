using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NV.DRF.UICommon
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public string PTitle
        {
            get { return (string)GetValue(PTitleProperty); }
            set { SetValue(PTitleProperty, value); }
        }
        public static readonly DependencyProperty PTitleProperty =
            DependencyProperty.Register("PTitle", typeof(string), typeof(MessageWindow), new PropertyMetadata("提示"));

        public string MessageText
        {
            get { return (string)GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }

        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(string), typeof(MessageWindow), new PropertyMetadata(""));

        public MessageWindow(string message)
        {
            InitializeComponent();
            this.DataContext = this;
            MessageText = message;
        }

        /// <summary>
        /// 返回值
        /// </summary>
        private MessageBoxResult result = MessageBoxResult.None;
        /// <summary>
        /// 获取或设置返回值
        /// </summary>
        public MessageBoxResult Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
                btn = (e as ExecutedRoutedEventArgs).Parameter as Button;
            NativeMethods.CatchExRun(delegate
            {
                Result = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), btn.Tag.ToString(), false);
            });
            this.Close();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }

    public static class CMessageBox
    {
        [SecurityCritical]
        public static MessageBoxResult Show(string messageBoxText)
        {
            MessageWindow win = new MessageWindow(messageBoxText);
            win.btnOK.Visibility = Visibility.Visible;
            win.ShowDialogEx();
            return MessageBoxResult.None;
        }

        [SecurityCritical]
        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            MessageWindow win = new MessageWindow(messageBoxText);
            win.PTitle = caption;
            win.btnOK.Visibility = Visibility.Visible;
            win.ShowDialogEx();
            return MessageBoxResult.None;
        }

        [SecurityCritical]
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            MessageWindow win = new MessageWindow(messageBoxText);
            win.PTitle = caption;
            switch (Convert.ToInt32(button))
            {
                case 0:
                    win.btnOK.Visibility = Visibility.Visible;
                    break;
                case 1:
                    win.btnOK.Visibility = Visibility.Visible;
                    win.btnCancel.Visibility = Visibility.Visible;
                    break;
                case 3:
                    win.btnYes.Visibility = Visibility.Visible;
                    win.btnNo.Visibility = Visibility.Visible;
                    win.btnCancel.Visibility = Visibility.Visible;
                    break;
                case 4:
                    win.btnYes.Visibility = Visibility.Visible;
                    win.btnNo.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            win.ShowDialogEx();
            return win.Result;
        }
    }
}
