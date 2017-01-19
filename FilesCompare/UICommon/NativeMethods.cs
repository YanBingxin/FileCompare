using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace NV.DRF.UICommon
{

    /// <summary>
    /// ID:通用方法类
    /// Describe:通用方法类
    /// Author:ybx
    /// Date:2016年11月24日10:12:40
    /// </summary>
    public static class NativeMethods
    {
        #region 捕获异常状态下运行
        /// <summary>
        /// 捕获异常状态下运行无返回值
        /// </summary>
        /// <param name="act"></param>
        public static void CatchExRun(Action act, bool isShowMessage = false)
        {
            try
            {
                act();
            }
            catch (Exception ex)
            {
                if (isShowMessage)
                    CMessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 捕获异常状态下有返回值运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T CatchExRun<T>(Func<T> func, bool isShowMessage = false)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                if (isShowMessage)
                    CMessageBox.Show(ex.Message);
                return default(T);
            }
        }
        #endregion

        #region 排序号
        /// <summary>
        /// 为泛型列表附加序号属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">目标列表</param>
        /// <returns>附加序号属性后的列表</returns>
        public static List<T> Sort<T>(this List<T> list, string proName)
        {
            System.Reflection.PropertyInfo proInfo = typeof(T).GetProperty(proName);
            int i = 1;
            foreach (var item in list)
            {
                proInfo.SetValue(item, i, null);
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
        public static ObservableCollection<T> Sort<T>(this ObservableCollection<T> list, string proName)
        {
            System.Reflection.PropertyInfo proInfo = typeof(T).GetProperty(proName);
            int i = 1;
            foreach (var item in list)
            {
                proInfo.SetValue(item, i, null);
                i++;
            }
            return list;
        }
        #endregion

        #region 获取数据源同名属性属性值，赋值到目标数据源
        /// <summary>
        /// 获取数据源同名属性属性值，赋值到目标数据源
        /// </summary>
        /// <param name="target">赋值对象</param>
        /// <param name="source">取值对象</param>
        /// <param name="ignores">用户指定忽略的属性名</param>
        /// <returns></returns>
        public static T CopyTo<T>(T target, object source, string[] ignores = null)
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
                    if (ignores != null && ignores.Contains(pro.Name) || !property.CanRead || !property.CanWrite)
                    {
                        continue;
                    }
                    NativeMethods.CatchExRun(delegate
                    {
                        //取出相同属性名且不被忽略的属性值
                        if (pro.Name == property.Name && property.GetValue(source, null) != null)
                        {
                            //object temp = Convert.ChangeType(property.GetValue(source, null), pro.PropertyType);//转换属性值类型
                            pro.SetValue(target, property.GetValue(source, null), null);//赋值
                        }
                    });
                }
            }
            //返回赋值对象
            return target;
        }

        #endregion

        #region 文件操作
        /// <summary>
        /// 计算文件夹大小
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;
            // 所有文件大小.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // 遍历出当前目录的所有文件夹.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }

        /// <summary>
        /// 将现有文件夹复制到新文件夹，允许覆盖,不对异常进行处理，应在调用此方法处对异常捕获处理
        /// </summary>
        /// <param name="fileclass"></param>
        /// <param name="souce">源文件夹</param>
        /// <param name="target">目标文件夹</param>
        /// <returns></returns> 
        public static bool CopyFolder(string souce, string target)
        {
            bool result = false;
            //复制文件
            foreach (var file in Directory.GetFiles(souce, "*.*"))
            {

                if (!Directory.Exists(target))
                    Directory.CreateDirectory(target);

                string newfullPath = Path.Combine(target, Path.GetFileName(file));
                File.Copy(file, newfullPath, true);
                result = true;
            }
            //复制子文件夹
            foreach (var dir in Directory.GetDirectories(souce))
            {
                CopyFolder(dir, Path.Combine(target, new DirectoryInfo(dir).Name));
            }
            return result;
        }

        public static bool DelDirectory(string path)
        {
            bool result = false;
            Directory.Delete(path, true);
            return result;
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

        #region Xml序列化
        public static T LoadFromFile<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            using (Stream stream = File.OpenRead(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream);
            }
        }

        public static void SaveToFile(object o, string fileName)
        {
            using (Stream stream = File.Create(fileName))
            {
                new XmlSerializer(o.GetType()).Serialize(stream, o);
            }
        }
        #endregion

    }

  
}
