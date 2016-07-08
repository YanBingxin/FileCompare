using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.Win32;

namespace FilesCompare.Common
{
    public class TextBoxHelper
    {
        #region 附加属性 IsClearButton
        /// <summary>
        /// 附加属性，是否带清空按钮
        /// </summary>
        public static readonly DependencyProperty IsClearButtonProperty =
            DependencyProperty.RegisterAttached("IsClearButton", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, ClearText));


        public static bool GetIsClearButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsClearButtonProperty);
        }

        public static void SetIsClearButton(DependencyObject obj, bool value)
        {
            obj.SetValue(IsClearButtonProperty, value);
        }

        #endregion

        #region 回调函数和清空输入框内容的实现
        /// <summary>
        /// 回调函数若附加属性IsClearButton值为True则挂载清空TextBox内容的函数
        /// </summary>
        /// <param name="d">属性所属依赖对象</param>
        /// <param name="e">属性改变事件参数</param>
        private static void ClearText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.Controls.Button btn = d as System.Windows.Controls.Button;
            if (d != null && e.OldValue != e.NewValue)
            {
                if (btn.Content.ToString() == "Open")
                {
                    btn.Click -= OpenPathClicked;
                    if ((bool)e.NewValue)
                    {
                        btn.Click += OpenPathClicked;
                    }
                }
                else
                {
                    btn.Click -= ClearTextClicked;
                    if ((bool)e.NewValue)
                    {
                        btn.Click += ClearTextClicked;
                    }
                }
            }
        }
        public static string LastPath = string.Empty;
        /// <summary>
        /// 打开文件操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OpenPathClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn != null)
            {
                var parent = VisualTreeHelper.GetParent(btn);
                while (!(parent is System.Windows.Controls.TextBox))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                System.Windows.Controls.TextBox txt = parent as System.Windows.Controls.TextBox;
                if (txt != null)
                {
                    FolderBrowserDialog dlg = new FolderBrowserDialog();
                    dlg.SelectedPath = LastPath;
                    dlg.Description = "请选择要比对的文件系统路径:";
                    if (dlg.ShowDialog() == DialogResult.OK)
                        txt.Text = dlg.SelectedPath;

                    LastPath = dlg.SelectedPath;
                }
            }

        }

        /// <summary>
        /// 清空应用该附加属性的父TextBox内容函数
        /// </summary>
        /// <param name="sender">发送对象</param>
        /// <param name="e">路由事件参数</param>
        public static void ClearTextClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            if (btn != null)
            {
                var parent = VisualTreeHelper.GetParent(btn);
                while (!(parent is System.Windows.Controls.TextBox))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                System.Windows.Controls.TextBox txt = parent as System.Windows.Controls.TextBox;
                if (txt != null)
                {
                    txt.Clear();
                }
            }
        }

        #endregion
    }
}
