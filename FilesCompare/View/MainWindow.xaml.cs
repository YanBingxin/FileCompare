using FilesCompare.IView;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using FilesCompare.Model;
using FilesCompare.Common;
using FilesCompare.CompareHelper;
using System.Reflection;
using FilesCompare.ViewModel;

namespace FilesCompare
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IMainWindowView
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region 拖动
        /// <summary>
        /// 为了减少引用的外部dll，偷懒写在这吧。嘻嘻
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        #region 比对
        /// <summary>
        /// 双击行，比对文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = (sender as DataGrid).SelectedIndex;
            if (index < 0)
                return;
            FNode f1 = dgNew.Items[index] as FNode;
            FNode f2 = dgOld.Items[index] as FNode;

            if (string.IsNullOrEmpty(f1.FFullName) || string.IsNullOrEmpty(f2.FFullName))
            {
                return;
            }

            DiffHelper.Diff(f1, f2);
        }
        #endregion

        #region 浏览文件
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void file_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FNode node = (sender as Image).Tag as FNode;
            if (node == null)
            {
                return;
            }
            //打开文件所在目录
            if (node.IsFile == true)
            {
                CommonMethod.ExploreFile(node.FFullName);
            }
            //打开目录
            else if (node.IsFile == false)
            {
                CommonMethod.ExplorePath(node.FFullName);
            }
        }
        #endregion

        #region 垂直滚动条同步
        /// <summary>
        /// 双Datagrid滚动条同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            DataGrid dg = (sender as DataGrid).Name == "dgNew" ? dgOld : dgNew;

            if (e.VerticalChange != 0.0f || e.HorizontalChange != 0.0f)
            {
                ScrollViewer sv = null;
                Type t = dgNew.GetType();
                try
                {
                    sv = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, dg, null) as ScrollViewer;
                    if (e.VerticalChange != 0.0f)
                        sv.ScrollToVerticalOffset(e.VerticalOffset);
                    if (e.HorizontalChange != 0.0f)
                        sv.ScrollToHorizontalOffset(e.HorizontalOffset);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        #region 编辑黑白名单
        /// <summary>
        /// 点击编辑，弹出黑白名单编辑框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Click_Ignore(object sender, RoutedEventArgs e)
        {
            this.pupIgnore.IsOpen = !pupIgnore.IsOpen;
            UpConfig();
        }

        private void TextBox_Click_Prefer(object sender, RoutedEventArgs e)
        {
            this.pupPrefer.IsOpen = !pupPrefer.IsOpen;
            UpConfig();
        }

        private void UpConfig()
        {
            (this.DataContext as MainWindowViewModel).DataGridToConfig();
        }
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (sender as TextBox);

            if (e.Changes != null)
            {
                ScrollViewer sv = null;
                Type t = txt.GetType();
                try
                {
                    sv = t.InvokeMember("ScrollViewer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, txt, null) as ScrollViewer;
                    sv.ScrollToVerticalOffset(txt.ExtentHeight);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
