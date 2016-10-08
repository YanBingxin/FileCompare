using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace FilesCompare.Common
{
    /// <summary>
    /// ID:通用方法
    /// Describe:通用方法类
    /// Author:ybx
    /// Date:2016-4-22 10:16:19
    /// </summary>
    public static class CommonMethod
    {
        #region 获取数据源同名属性属性值，赋值到目标数据源
        /// <summary>
        /// 获取数据源同名属性属性值，赋值到目标数据源
        /// </summary>
        /// <param name="target">赋值对象</param>
        /// <param name="source">取值对象</param>
        /// <param name="ignores">用户指定忽略的属性名</param>
        /// <returns></returns>
        public static object GetValueByPropertyName(object target, object source, string[] ignores = null)
        {
            //目标为空，抛出
            if (target == null)
            {
                throw new ArgumentNullException("参数：" + target + "为空引发了异常");
            }
            //数据源为空，抛出
            if (source == null)
            {
                throw new ArgumentNullException("参数：" + source + "为空引发了异常");
            }
            //获取赋值对象属性
            foreach (PropertyInfo pro in target.GetType().GetProperties())
            {
                //获取取值对象属性
                foreach (PropertyInfo property in source.GetType().GetProperties())
                {
                    //忽略用户指定属性
                    if (ignores != null && ignores.Contains(pro.Name))
                    {
                        continue;
                    }
                    //取出相同属性名且不被忽略的属性值
                    if (pro.Name == property.Name && property.GetValue(source, null) != null)
                    {
                        object temp = Convert.ChangeType(property.GetValue(source, null), pro.PropertyType);//转换属性值类型
                        pro.SetValue(target, temp, null);//赋值
                    }
                }
            }
            //返回赋值对象
            return target;
        }

        #endregion

        #region 字符串、日期扩展ToExtString()
        /// <summary>
        /// DateTime类扩展ToExtString方法,返回无地域格式yyyy-MM-dd
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToExtString(this DateTime dateTime, string format = "yyyy-MM-dd")
        {
            return dateTime.ToString(format, DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// String类扩展ToExtString方法，成功转换日期，返回无地域格式yyyy-MM-dd，否则返回空
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToExtString(this string str, bool isThrowEx = false)
        {
            DateTime temp;
            if (DateTime.TryParse(str, out temp))
            {
                return temp.ToExtString();
            }
            if (isThrowEx)
            {
                throw new ArgumentException("参数：" + str + "不合法。");
            }
            return string.Empty;
        }
        #endregion

        #region 异常处理，日志记录
        /// <summary>
        /// 城镇土地使用税异常模式处理方法
        /// </summary>
        /// <param name="ac">Action</param>
        /// <param name="errorMessage">预定义异常提示信息</param>
        /// <param name="isShowExMessage">是否弹出提示框</param>
        public static void CZTDSYSRunAction(Action ac, string errorMessage = "", bool isShowExMessage = true)
        {
            try
            {
                ac.Invoke();
            }
            catch (Exception ex)
            {
                //记录异常日志和信息
                //Logger.Output(ex, "城镇土地使用税异常。");
                //弹出提示框提示异常信息和详细信息
                if (isShowExMessage) { }
                //CMessageBox.ShowInfoMessageWithDetailMsg(string.IsNullOrEmpty(errorMessage) ? "请联系管理员：" + ex.Message : errorMessage + "\r\n" + ex.Message, ex.ToString());
            }
        }
        #endregion

        #region 为列表附加序号属性
        /// <summary>
        /// 为泛型列表附加序号属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">目标列表</param>
        /// <returns>附加序号属性后的列表</returns>
        public static List<T> AttachNumber<T>(this List<T> list) where T : DependencyObject
        {
            int i = 1;
            foreach (var item in list)
            {
                Attach.SetNumber(item, i);
                i++;
            }
            return list;
        }
        /// <summary>
        /// 为泛型列表附加序号属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">目标列表</param>
        /// <returns>附加Xh属性后的列表</returns>
        public static List<T> OrderNumber<T>(this List<T> list) where T : IAttachNumber
        {
            int i = 1;
            foreach (var item in list)
            {
                item.Number = i;
                i++;
            }
            return list;
        }
        #endregion

        #region 在DataGrid中将目标滚动为第一行
        /// <summary>
        /// 将SelectedItem滚动为第一行
        /// </summary>
        /// <param name="dataGrid">目标DagaGrid</param>
        /// <param name="selectedItem">选中项</param>
        public static void SetSelectedItemFirstRow(object dataGrid, object selectedItem)
        {
            //若目标datagrid为空，抛出异常
            if (dataGrid == null)
            {
                throw new ArgumentNullException("目标无" + dataGrid + "无法转换为DataGrid");
            }
            //获取目标DataGrid，为空则抛出异常
            System.Windows.Controls.DataGrid dg = dataGrid as System.Windows.Controls.DataGrid;
            if (dg == null)
            {
                throw new ArgumentNullException("目标无" + dataGrid + "无法转换为DataGrid");
            }
            //数据源为空则返回
            if (dg.Items == null || dg.Items.Count < 1)
            {
                return;
            }

            //首先滚动为末行
            dg.SelectedItem = dg.Items[dg.Items.Count - 1];
            dg.CurrentColumn = dg.Columns[0];
            dg.ScrollIntoView(dg.SelectedItem, dg.CurrentColumn);

            //获取焦点，滚动为目标行
            dg.Focus();
            dg.SelectedItem = selectedItem;
            dg.CurrentColumn = dg.Columns[0];
            dg.ScrollIntoView(dg.SelectedItem, dg.CurrentColumn);
        }
        #endregion

        #region 扩展ShowDialog()、获取当前激活窗口
        /// <summary>
        /// 扩展ShowDialogEx
        /// </summary>
        /// <param name="win">基类</param>
        /// <param name="showInTaskbar">任务栏是否显示图标：true-显示，false-不显示</param>
        /// <returns></returns>
        public static bool? ShowDialogEx(this Window win, bool showInTaskbar = false)
        {
            win.ShowInTaskbar = showInTaskbar;//是否显示在任务栏
            win.Owner = GetActiveWindowEx();//获取owner窗口
            return win.ShowDialog();
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetActiveWindow();
        /// <summary>
        /// 获取活动Window
        /// </summary>
        /// <param name="accurate">是否精确查找；当为true时，将仅采用windows api获取活动Window;当为false时，先用api获取，获取不到取Application.Current.Windows中活动或可见的某一个</param>
        /// <returns></returns>
        public static Window GetActiveWindowEx(bool accurate = false)
        {
            Window w = null;
            try
            {
                //该方法是获取当前Windows活动的界面，不一定是当前应用程序中的界面
                IntPtr hwnd = GetActiveWindow();
                if (hwnd != IntPtr.Zero)
                {
                    if (System.Windows.Interop.HwndSource.FromHwnd(hwnd).RootVisual is Window)
                    {
                        w = (Window)System.Windows.Interop.HwndSource.FromHwnd(hwnd).RootVisual;
                        if (w != null)
                        {
                            foreach (var itm in Application.Current.Windows)
                            {
                                if (itm.Equals(w))
                                    return w;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (accurate)
                return w;
            try
            {
                //Application.Current.Windows，记录实例化且未释放的Window，排序按照实例化的先后顺序，最后一项应是最后实例化的Window
                foreach (Window win in Application.Current.Windows)
                {
                    if (win.IsActive || win.Visibility == Visibility.Visible)
                    {
                        w = win;
                        if (win.IsActive)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return w;
        }
        #endregion

        #region 读写文件（kernel32）
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, byte[] val, string filePath);

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Write(string section, string key, string value, string path)
        {
            byte[] bytes = Encoding.Default.GetBytes(value);
            WritePrivateProfileString(section, key, bytes, path);
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        public static string Read(string section, string key, string path)
        {
            Encoding enc = Encoding.Default;
            StringBuilder temp = new StringBuilder(102400);
            int i = GetPrivateProfileString(section, key, "", temp, 102400, path);
            byte[] buff = Encoding.Default.GetBytes(temp.ToString());
            return enc.GetString(buff);
        }
        #endregion

        #region 文件浏览器打开文件、文件夹
        /// <summary>
        /// 浏览文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void ExploreFile(string filePath)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "explorer";
            //打开资源管理器
            proc.StartInfo.Arguments = @"/select," + filePath;
            //选中"notepad.exe"这个程序,即记事本
            proc.Start();
        }

        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void ExplorePath(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        #endregion

        #region 调用命令行
        /// <summary>
        /// 执行cmd命令
        /// 多命令请使用批处理命令连接符：
        /// <![CDATA[
        /// &:同时执行两个命令
        /// |:将上一个命令的输出,作为下一个命令的输入
        /// &&：当&&前的命令成功时,才执行&&后的命令
        /// ||：当||前的命令失败时,才执行||后的命令]]>
        /// 其他请百度
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="output"></param>
        public static void RunCmd(string cmd, out string output, bool isBackground = true)
        {
            cmd = cmd.Trim().TrimEnd('&') + (isBackground ? "&exit" : "");//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process p = new Process())
            {
                p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = isBackground;          //不显示程序窗口
                p.Start();//启动程序

                //向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                //获取cmd窗口的输出信息
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
            }
        }
        #endregion

        #region 关联文件类型
        /// <summary>
        /// 关联文件
        /// </summary>
        /// <param name="_FilePathString">应用程序路径</param>
        /// <param name="p_FileTypeName">文件类型</param>
        public static void SaveReg(string _FilePathString, string p_FileTypeName)
        {
            RegistryKey _RegKey = Registry.ClassesRoot.OpenSubKey("", true);              //打开注册表
            RegistryKey _VRPkey = _RegKey.OpenSubKey(p_FileTypeName, true);
            if (_VRPkey != null)
            {
                _RegKey.DeleteSubKey(p_FileTypeName, true);
            }
            _RegKey.CreateSubKey(p_FileTypeName);
            _VRPkey = _RegKey.OpenSubKey(p_FileTypeName, true);
            _VRPkey.SetValue("", "Exec");
            _VRPkey = _RegKey.OpenSubKey("Exec", true);
            if (_VRPkey != null) _RegKey.DeleteSubKeyTree("Exec");         //如果等于空就删除注册表DSKJIVR
            _RegKey.CreateSubKey("Exec");
            _VRPkey = _RegKey.OpenSubKey("Exec", true);
            _VRPkey.CreateSubKey("shell");
            _VRPkey = _VRPkey.OpenSubKey("shell", true);                      //写入必须路径
            _VRPkey.CreateSubKey("open");
            _VRPkey = _VRPkey.OpenSubKey("open", true);
            _VRPkey.CreateSubKey("command");
            _VRPkey = _VRPkey.OpenSubKey("command", true);
            string _PathString = "\"" + _FilePathString + "\" \"%1\"";
            _VRPkey.SetValue("", _PathString);                                    //写入数据
        }
        #endregion

        #region 截图
        public static void SaveElementPng(FrameworkElement fElement, string fileName)
        {
            try
            {
                using (System.IO.FileStream fs = System.IO.File.Open(fileName, System.IO.FileMode.OpenOrCreate))
                {
                    RenderTargetBitmap tbit = new RenderTargetBitmap((int)fElement.ActualWidth, (int)fElement.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
                    tbit.Render(fElement);
                    BitmapEncoder ben = new PngBitmapEncoder();
                    ben.Frames.Add(BitmapFrame.Create(tbit));
                    ben.Save(fs);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    #region 附加序号接口、附加属性类
    /// <summary>
    /// 附加序号接口
    /// </summary>
    public interface IAttachNumber
    {
        /// <summary>
        /// 序号
        /// </summary>
        int Number { set; get; }
    }
    /// <summary>
    /// ID:附加属性类
    /// Describe:附加序号
    /// Author:ybx
    /// Date:2016-4-21 15:53:29
    /// </summary>
    public class Attach
    {
        #region Number(序号)
        /// <summary>
        /// 附加属性 序号
        /// </summary>
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.RegisterAttached("Number", typeof(int), typeof(Attach), new PropertyMetadata(0));

        public static int GetNumber(DependencyObject obj)
        {
            return (int)obj.GetValue(NumberProperty);
        }

        public static void SetNumber(DependencyObject obj, int value)
        {
            obj.SetValue(NumberProperty, value);
        }
        #endregion
    }
    #endregion
}
