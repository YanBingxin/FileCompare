using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FilesCompare.Common;

namespace FilesCompare.View
{
    /// <summary>
    /// ResPreview.xaml 的交互逻辑
    /// </summary>
    public partial class ResPreview : BxWindow.BxWindow
    {
        public ResPreview()
        {
            InitializeComponent();
        }

        public override double TitleHeight
        {
            get
            {
                return 30;
            }
        }

        public override double TitleRightMargin
        {
            get
            {
                return this.stpHeaderButton.ActualWidth;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string res = string.Empty;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.AddExtension = true;
                dlg.DefaultExt = ".png";
                dlg.Filter = "结果截图 (*.png)|*.png";
                dlg.FileName = "xxx.png";
                dlg.Title = "请选择导出结果文件位置：";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    res = dlg.FileName;
            }

            if (string.IsNullOrEmpty(res))
                return;
            try
            {
                CommonMethod.SaveElementPng(this.gdRes, res);
                CMessageBox.Show("保存成功。");
            }
            catch (Exception ex)
            {
                CMessageBox.Show("保存失败,原因:" + ex.Message);
            }
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
