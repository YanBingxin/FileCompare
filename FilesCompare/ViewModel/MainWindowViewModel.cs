using FilesCompare.IView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FilesCompare.IViewModel;
using System.Windows.Input;
using FilesCompare.Common;
using FilesCompare.Model;
using System.Windows;
using System.Threading;
using FilesCompare.CompareHelper;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace FilesCompare.ViewModel
{
    public class MainWindowViewModel : NotifyObject, IMainWindowViewModel
    {
        #region 命令
        /// <summary>
        /// 分析命令
        /// </summary>
        public ICommand DescryptCommand { get; set; }
        /// <summary>
        /// 最小化命令
        /// </summary>
        public ICommand MinCommand { get; set; }
        /// <summary>
        /// 最大化命令
        /// </summary>
        public ICommand MaxCommand { get; set; }
        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand { get; set; }
        /// <summary>
        /// 拖动命令
        /// </summary>
        public ICommand DragMoveCommand { get; set; }
        #endregion

        #region 字段
        /// <summary>
        /// 每次解压将在temp1、2下生成该序号的文件，防止重复出错
        /// </summary>
        private static int TEMPNUMBER = 0;
        /// <summary>
        /// 同步锁对象
        /// </summary>
        private static readonly object SysObject = new object();
        /// <summary>
        /// 计时器
        /// </summary>
        DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
        #endregion

        #region 属性

        /// <summary>
        /// 1比2多出数量
        /// </summary>
        private int _more;
        /// <summary>
        /// 获取或设置1比2多出数量
        /// </summary>
        public int More
        {
            get
            {
                return _more;
            }
            set
            {
                if (_more == value)
                {
                    return;
                }
                _more = value;
                RaisePropertyChanged(() => More);
            }
        }

        /// <summary>
        /// 2比1多出数量
        /// </summary>
        private int _less;
        /// <summary>
        /// 获取或设置2比1多出数量
        /// </summary>
        public int Less
        {
            get
            {
                return _less;
            }
            set
            {
                if (_less == value)
                {
                    return;
                }
                _less = value;
                RaisePropertyChanged(() => Less);
            }
        }

        /// <summary>
        /// 1比2修改数量
        /// </summary>
        private int _changed;
        /// <summary>
        /// 获取或设置1比2修改数量
        /// </summary>
        public int Changed
        {
            get
            {
                return _changed;
            }
            set
            {
                if (_changed == value)
                {
                    return;
                }
                _changed = value;
                RaisePropertyChanged(() => Changed);
            }
        }
        /// <summary>
        /// 过滤字符串
        /// </summary>
        private string _config = string.Empty;
        /// <summary>
        /// 获取或设置过滤字符串
        /// </summary>
        public string Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
                RaisePropertyChanged("Config");

                Ignores.Clear();
                foreach (var key in _config.Split('|'))
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    Ignores.Add(key);
                }
                RaisePropertyChanged("Ignores");
            }
        }

        private ObservableCollection<string> _ignores;
        /// <summary>
        /// 过滤关键字
        /// </summary>
        public ObservableCollection<string> Ignores
        {
            get
            {
                return _ignores ?? (_ignores = new ObservableCollection<string>());
            }
            set
            {
                _ignores = value;
            }
        }

        /// <summary>
        /// 日志
        /// </summary>
        private string _logs;
        /// <summary>
        /// 获取或设置日志
        /// </summary>
        public string Logs
        {
            get
            {
                return _logs;
            }
            set
            {
                _logs = value;
                RaisePropertyChanged(() => Logs);
            }
        }

        /// <summary>
        /// 目标1
        /// </summary>
        private string _filePath1;
        /// <summary>
        /// 获取或设置目标1
        /// </summary>
        public string FilePath1
        {
            get
            {
                return _filePath1;
            }
            set
            {
                if (_filePath1 == value)
                {
                    return;
                }
                _filePath1 = value;
                RaisePropertyChanged(() => FilePath1);
            }
        }

        /// <summary>
        /// 目标2
        /// </summary>
        private string _filePath2;
        /// <summary>
        /// 获取或设置目标1
        /// </summary>
        public string FilePath2
        {
            get
            {
                return _filePath2;
            }
            set
            {
                if (_filePath2 == value)
                {
                    return;
                }
                _filePath2 = value;
                RaisePropertyChanged(() => FilePath2);
            }
        }

        /// <summary>
        /// 正在解压文件名
        /// </summary>
        private string _unZipFileName;
        /// <summary>
        /// 获取或设置正在解压文件名
        /// </summary>
        public string UnzipFileName
        {
            get
            {
                return _unZipFileName;
            }
            set
            {
                _unZipFileName = value;
                RaisePropertyChanged(() => UnzipFileName);
            }
        }

        /// <summary>
        /// 已检索MD5子文件(夹)数
        /// </summary>
        private int _numMD5;
        /// <summary>
        /// 已检索子MD5文件(夹)数
        /// </summary>
        public int NumMD5
        {
            get
            {
                return _numMD5;
            }
            set
            {
                _numMD5 = value;
                RaisePropertyChanged(() => NumMD5);
            }
        }

        /// <summary>
        /// 已比对子文件(夹)数
        /// </summary>
        private int _numDif;
        /// <summary>
        /// 已比对文件(夹)数
        /// </summary>
        public int NumDif
        {
            get
            {
                return _numDif;
            }
            set
            {
                _numDif = value;
                RaisePropertyChanged(() => NumDif);
            }
        }

        /// <summary>
        /// 总文件(夹)数
        /// </summary>
        private int _numAll;
        /// <summary>
        /// 总文件(夹)数
        /// </summary>
        public int NumAll
        {
            get
            {
                return _numAll;
            }
            set
            {
                _numAll = value;
                RaisePropertyChanged(() => NumAll);
            }
        }

        private int _time;
        /// <summary>
        /// 花费时间
        /// </summary>
        public int Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                this.RaisePropertyChanged(() => Time);
            }
        }

        private string m_btnContent;
        /// <summary>
        /// 按钮内容
        /// </summary>
        public string btnContent
        {
            get
            {
                return m_btnContent;
            }
            set
            {
                m_btnContent = value;
                this.RaisePropertyChanged(() => btnContent);
            }
        }

        private ObservableCollection<FNode> _fs1;
        /// <summary>
        /// 原始文件系统1，(未解压.jar.zip)
        /// </summary>
        public ObservableCollection<FNode> NormalFiles1
        {
            get
            {
                return _fs1 ?? (_fs1 = new ObservableCollection<FNode>());
            }
            set
            {
                _fs1 = value;
                RaisePropertyChanged(() => NormalFiles1);
            }
        }

        private ObservableCollection<FNode> _fs2;
        /// <summary>
        /// 原始文件系统2，(未解压.jar.zip)
        /// </summary>
        public ObservableCollection<FNode> NormalFiles2
        {
            get
            {
                return _fs2 ?? (_fs2 = new ObservableCollection<FNode>());
            }
            set
            {
                _fs2 = value;
                RaisePropertyChanged(() => NormalFiles2);
            }
        }

        private ObservableCollection<FNode> _fs3;
        /// <summary>
        /// 差异文件系统1，(未解压.jar.zip)
        /// </summary>
        public ObservableCollection<FNode> DifFiles1
        {
            get
            {
                return _fs3 ?? (_fs3 = new ObservableCollection<FNode>());
            }
            set
            {
                _fs3 = value;
                RaisePropertyChanged(() => DifFiles1);
            }
        }

        private ObservableCollection<FNode> _fs4;
        /// <summary>
        /// 差异文件系统2，(未解压.jar.zip)
        /// </summary>
        public ObservableCollection<FNode> DifFiles2
        {
            get
            {
                return _fs4 ?? (_fs4 = new ObservableCollection<FNode>());
            }
            set
            {
                _fs4 = value;
                RaisePropertyChanged(() => DifFiles2);
            }
        }

        private ObservableCollection<FNode> _fs5;
        /// <summary>
        /// 差异文件系统1，(仅包含.jar.zip)
        /// </summary>
        public ObservableCollection<FNode> ZipFiles1
        {
            get
            {
                return _fs5 ?? (_fs5 = new ObservableCollection<FNode>());
            }
            set
            {
                _fs5 = value;
                RaisePropertyChanged(() => ZipFiles1);
            }
        }

        private ObservableCollection<FNode> _fs6;
        /// <summary>
        /// 差异文件系统2，(仅包含.jar.zip)
        /// </summary>
        public ObservableCollection<FNode> ZipFiles2
        {
            get
            {
                return _fs6 ?? (_fs6 = new ObservableCollection<FNode>());
            }
            set
            {
                _fs6 = value;
                RaisePropertyChanged(() => ZipFiles2);
            }
        }

        private ObservableCollection<FNode> _fs7;
        /// <summary>
        /// 差异文件系统1，(仅包含.jar.zip)解压后
        /// </summary>
        public ObservableCollection<FNode> UnCompressFiles1
        {
            get
            {
                return _fs7 ?? (_fs7 = new ObservableCollection<FNode>());
            }
            set
            {
                _fs7 = value;
                RaisePropertyChanged(() => UnCompressFiles1);
            }
        }

        private ObservableCollection<FNode> _fs8;
        /// <summary>
        /// 差异文件系统2，(仅包含.jar.zip)
        /// </summary>
        public ObservableCollection<FNode> UnCompressFiles2
        {
            get
            {
                return _fs8 ?? (_fs8 = new ObservableCollection<FNode>());
            }
            set
            {
                _fs8 = value;
                RaisePropertyChanged(() => UnCompressFiles2);
            }
        }

        private ObservableCollection<FNode> _result1;
        /// <summary>
        /// 结果1
        /// </summary>
        public ObservableCollection<FNode> Result1
        {
            get
            {
                return _result1 ?? (_result1 = new ObservableCollection<FNode>());
            }
            set
            {
                _result1 = value;
                RaisePropertyChanged(() => Result1);
            }
        }
        private ObservableCollection<FNode> _result2;
        /// <summary>
        /// 结果2
        /// </summary>
        public ObservableCollection<FNode> Result2
        {
            get
            {
                return _result2 ?? (_result2 = new ObservableCollection<FNode>());
            }
            set
            {
                _result2 = value;
                RaisePropertyChanged(() => Result2);
            }
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 页面
        /// </summary>
        private IMainWindowView m_View;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="view"></param>
        public MainWindowViewModel(IMainWindowView view)
        {
            m_View = view;
            m_View.DataContext = this;
            InitData();
            InitCommand();
            LoadIgnores();
            InitTempDirectory();
        }

        /// <summary>
        /// 初始命令
        /// </summary>
        private void InitCommand()
        {
            DescryptCommand = new RelayCommand(CompareExecute, CompareCanExecute);
            MinCommand = new RelayCommand(MinWindowExecute);
            MaxCommand = new RelayCommand(MaxWindowExecute);
            CloseCommand = new RelayCommand(CloseExecute);
            DragMoveCommand = new RelayCommand(DragMoveExecute);
        }

        private void OpenPathExecute(object obj)
        {
            MessageBox.Show(obj.ToString());
        }

        /// <summary>
        /// 初始数据
        /// </summary>
        private void InitData()
        {
            btnContent = string.Format("开始分析");
            TEMPNUMBER = 0;
        }
        #endregion

        #region 方法处理
        /// <summary>
        /// 是否可以开始分析
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CompareCanExecute(object arg)
        {
            return !timer.IsEnabled;
        }
        /// <summary>
        /// 校对处理
        /// </summary>
        /// <param name="prarmeter"></param>
        private void CompareExecute(object prarmeter)
        {
            TEMPNUMBER++;
            InitDataForCompare();
            CompareFiles();
        }
        /// <summary>
        /// 初始化比较所用数据
        /// </summary>
        private void InitDataForCompare()
        {
            Log("日期:" + DateTime.Now.ToExtString());
            Log("目标1:" + FilePath1);
            Log("目标2:" + FilePath2);
            Log("分析系统准备中...");

            InitTempDirectory();

            UnzipFileName = "开始校对...";

            More = 0;
            Less = 0;
            Changed = 0;

            NormalFiles1.Clear();
            NormalFiles2.Clear();

            DifFiles1.Clear();
            DifFiles2.Clear();

            ZipFiles1.Clear();
            ZipFiles2.Clear();

            UnCompressFiles1.Clear();
            UnCompressFiles2.Clear();

            Result1.Clear();
            Result2.Clear();

            NumMD5 = 0;
            NumDif = 0;

            Log("系统数据初始化完毕。");
            Time = 0;
            timer.Tick -= Count;
            timer.Tick += Count;
            Log("计时器准备就绪。");
        }

        /// <summary>
        /// 初始临时文件夹
        /// </summary>
        private void InitTempDirectory()
        {
            try
            {
                if (!Directory.Exists("Temp1"))
                {
                    Directory.CreateDirectory("Temp1");
                }
                if (!Directory.Exists("Temp2"))
                {
                    Directory.CreateDirectory("Temp2");
                }
            }
            catch (Exception)
            {
                Log("临时文件夹创建异常。");
            }
        }

        /// <summary>
        /// 加载过滤关键字
        /// </summary>
        private void LoadIgnores()
        {
            string config = CommonMethod.Read("立思辰文件比对工具2.0--by ybx", "过滤关键词", Environment.CurrentDirectory + @"\Compare.cfg");
            FilePath1 = CommonMethod.Read("立思辰文件比对工具2.0--by ybx", "目标1", Environment.CurrentDirectory + @"\Compare.cfg");
            FilePath2 = CommonMethod.Read("立思辰文件比对工具2.0--by ybx", "目标2", Environment.CurrentDirectory + @"\Compare.cfg");

            Config = config;
            if (string.IsNullOrEmpty(config))
                return;

            Ignores.Clear();
            foreach (var key in config.Split('|'))
            {
                Ignores.Add(key);
            }
        }

        /// <summary>
        /// 检索文件目录计算MD5
        /// </summary>
        private void CompareFiles()
        {
            Log("初步分析准备中...");
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((sender, e) =>
            {
                Parallel.Invoke(new Action(() =>
                {
                    Log("目标1文件系统加载中...");
                    NormalFiles1 = LoadFiles(FilePath1);
                    Log("目标1文件系统加载完毕。");
                }), new Action(() =>
                {
                    Log("目标2文件系统加载中...");
                    NormalFiles2 = LoadFiles(FilePath2);
                    Log("目标2文件系统加载完毕。");
                }));

                Log("文件差异分析中...");
                Compare(NormalFiles1, NormalFiles2);
                Log("文件差异分析完毕。");
            }));

            bg.RunWorkerCompleted += CompareCompeleted;
            Log("初步分析准备完毕。");
            Log("开始初步分析文件...");
            bg.RunWorkerAsync();

            timer.Start();
        }

        /// <summary>
        /// 第一次比较完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompareCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("压缩文件(.jar.zip)分析系统启动中...");
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((s, a) =>
            {
                Parallel.Invoke(new Action(() =>
                {
                    Log("目标1压缩文件解压中...");
                    UnZipFiles(ZipFiles1, "Temp1");
                    Log("目标1压缩文件解压完毕。");
                    UnCompressFiles1 = LoadFiles(ZipFiles1, "Temp1");
                    Log("目标1解压文件加载完毕。");
                }), new Action(() =>
                {
                    Log("目标2压缩文件解压中...");
                    UnZipFiles(ZipFiles2, "Temp2");
                    Log("目标2压缩文件解压完毕。");
                    UnCompressFiles2 = LoadFiles(ZipFiles2, "Temp2");
                    Log("目标2解压文件加载完毕。");
                }));
                UnzipFileName = "解压完毕。";

                Log("压缩文件差异分析中...");
                UnzipFileName = "校对中...";
                Compare(UnCompressFiles1, UnCompressFiles2);
                UnzipFileName = "校对完毕。";
                Log("压缩文件差异分析完毕。");
            }));

            bg.RunWorkerCompleted += SecondCompareCompeleted;
            Log("压缩文件(.jar.zip)分析系统已就绪。");

            bg.RunWorkerAsync();
        }
        /// <summary>
        /// 第二次比较完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecondCompareCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NumDif = NumMD5;//无意义，为了使使用者安心。由于分析包含目录结构，实际校对数<=总数。(例如多出文件夹，将不比较子目录文件)
            Log("文件分析系统校对完毕。");

            //缓冲加载结果，防卡死
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((s, a) =>
            {
                for (int i = 0; i < DifFiles1.Count; i++)
                {
                    //过虑忽略文件
                    if (Ignore(DifFiles1[i]))
                        continue;

                    if (i > 0 && i % 40 == 0)
                    {
                        Thread.Sleep(500);
                    }
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Result1.Add(DifFiles1[i]);
                        Result2.Add(DifFiles2[i]);
                    }));
                }
            }));
            bg.RunWorkerCompleted += Completed;
            bg.RunWorkerAsync();

            timer.Stop();
            Log("计时器已终止。");
            Log("结果加载中...");
        }

        /// <summary>
        /// 结果加载完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            More = Result1.Where(f => f.DifTag == false).Count();
            Less = Result2.Where(f => f.DifTag == false).Count();
            Changed = Result1.Where(f => f.DifTag == true).Count();

            Log("结果加载完毕。");
            Log("\r\n");
        }

        #region 解压缩文件
        /// <summary>
        /// 解压缩.zip.jar文件
        /// </summary>
        private void UnZipFiles(ObservableCollection<FNode> collection, string location)
        {
            string rootName = location == "Temp1" ? FilePath1 : FilePath2;
            foreach (var file in collection)
            {
                try
                {
                    //过滤文件类型
                    if (Ignore(file))
                        continue;

                    lock (SysObject)
                    {
                        Log("解压：" + file.FFullName);
                        UnzipFileName = file.FFullName;
                    }
                    string folderName = file.FFullName.Replace(".zip", "").Replace(".jar", "").Replace(rootName, location + "/" + TEMPNUMBER.ToString());
                    UnCompressHelper.UnZipDir(file.FFullName, folderName, false);
                }
                catch (Exception e)
                {
                    Log(string.Format(@"
// 异常信息
// 解压时出现异常
// 文件名:{0}
// 详细信息:{1}
", file.FFullName, e.Message));
                    MessageBox.Show("解压文件失败：" + file.FFullName + e.Message + e.ToString(), "提示");
                }
            }
        }
        #endregion

        #region 过滤文件
        /// <summary>
        /// 检查是否是要过滤的文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool Ignore(FNode file)
        {
            foreach (var key in Ignores)
            {
                if (file.FName.Contains(key))
                    return true;
            }
            return false;
        }
        #endregion

        #region 加载文件
        /// <summary>
        /// 检索指定目录的所有字文件(夹)
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        private ObservableCollection<FNode> LoadFiles(string pathName)
        {
            ObservableCollection<FNode> tempList = new ObservableCollection<FNode>();
            //检索当前目录所有文件和目录节点
            try
            {
                tempList = new ObservableCollection<FNode>((from f in Directory.GetFiles(pathName)
                                                            select new FNode()
                                                            {
                                                                FFullName = f,
                                                                FName = f.Replace(pathName + "\\", ""),
                                                                IsFile = true
                                                            }).Concat(from d in Directory.GetDirectories(pathName)
                                                                      select new FNode()
                                                                      {
                                                                          FFullName = d,
                                                                          FName = d.Replace(pathName + "\\", ""),
                                                                          Child = LoadFiles(d)
                                                                      }));
            }
            catch (Exception ex)
            {
                Log(string.Format(@"
// 异常信息
// 加载文件时出现异常
// {0}
", ex.Message));
            }
            //双线程引起不同步，导致文件数总显示不对，并发引起的问题，找了半天。
            lock (SysObject)
            {
                NumMD5 += tempList.Count();
            }

            return tempList;
        }

        /// <summary>
        /// 对包含.jar.zip压缩文件的集合解压后的文件系统进行加载
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private ObservableCollection<FNode> LoadFiles(ObservableCollection<FNode> collection, string location)
        {
            string rootName = location == "Temp1" ? FilePath1 : FilePath2;
            ObservableCollection<FNode> tempList = new ObservableCollection<FNode>();
            foreach (var node in collection)
            {
                if (Ignore(node))
                    continue;

                FNode foder = new FNode();
                foder.FFullName = node.FFullName.Replace(".jar", "").Replace(".zip", "").Replace(rootName, Environment.CurrentDirectory + "/" + location + "/" + TEMPNUMBER.ToString());//解压缩节点目录全名
                foder.FName = node.FName.Replace(".jar", "").Replace(".zip", "");//解压缩节点目录名
                foder.Child = LoadFiles(foder.FFullName, node.FName);//获取子目录文件(夹)
                tempList.Add(foder);
            }
            lock (SysObject)
            {
                NumMD5 += collection.Count();
            }
            return tempList;
        }

        /// <summary>
        /// 检索jar包下所有字文件(夹)附加jar包标示
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        private ObservableCollection<FNode> LoadFiles(string pathName, string jarName)
        {
            ObservableCollection<FNode> tempList = new ObservableCollection<FNode>();
            //检索当前目录所有文件和目录节点
            try
            {
                tempList = new ObservableCollection<FNode>((from f in Directory.GetFiles(pathName)
                                                            select new FNode()
                                                            {
                                                                FFullName = f,
                                                                FName = f.Replace(pathName + "\\", ""),
                                                                JarParentName = jarName,
                                                                IsFile = true
                                                            }).Concat(from d in Directory.GetDirectories(pathName)
                                                                      select new FNode()
                                                                      {
                                                                          FFullName = d,
                                                                          FName = d.Replace(pathName + "\\", ""),
                                                                          Child = LoadFiles(d, jarName),
                                                                          JarParentName = jarName
                                                                      }));
            }
            catch (Exception ex)
            {
                Log(string.Format(@"
// 异常信息
// 加载文件时出现异常
// {0}
", ex.Message));
            }
            //双线程引起不同步，导致文件数总显示不对，并发引起的问题，找了半天。
            lock (SysObject)
            {
                NumMD5 += tempList.Count();
            }

            return tempList;
        }
        #endregion

        #region 比对文件
        /// <summary>
        /// 递归对比文件差异
        /// </summary>
        /// <param name="collection1"></param>
        /// <param name="collection2"></param>
        /// <returns></returns>
        public bool Compare(ObservableCollection<FNode> collection1, ObservableCollection<FNode> collection2)
        {
            try
            {
                var more = from f1 in collection1
                           where !(from f2 in collection2
                                   select f2.FName).Contains(f1.FName)
                           select f1;

                var less = from f2 in collection2
                           where !(from f1 in collection1
                                   select f1.FName).Contains(f2.FName)
                           select f2;

                var updated = from f1 in collection1
                              from f2 in collection2
                              where f1.FName == f2.FName && f1.IsFile == true && f2.IsFile == true && f1.FMD5 != f2.FMD5
                              orderby f1.FFullName
                              select new
                              {
                                  f1,
                                  f2
                              };
                var children = from f1 in collection1
                               from f2 in collection2
                               where f1.FName == f2.FName && f1.IsFile == false && f2.IsFile == false
                               select new
                               {
                                   f1,
                                   f2
                               };
                foreach (var item in more)
                {
                    //添加"多出"标识
                    item.DifTag = false;
                    DifFiles1.Add(item);
                    DifFiles2.Add(new FNode() { IsFile = null });
                }
                foreach (var item in less)
                {
                    //添加"多出"标识
                    item.DifTag = false;
                    DifFiles1.Add(new FNode() { IsFile = null });
                    DifFiles2.Add(item);
                }
                foreach (var item in updated)
                {
                    //变更为“修改”差异标识
                    item.f1.DifTag = item.f2.DifTag = true;
                    //将改变的jar，zip文件捕获，后续解压缩二次比较
                    if (item.f1.FFullName.Contains(".zip") || item.f1.FFullName.Contains(".jar"))
                    {
                        ZipFiles1.Add(item.f1);
                        ZipFiles2.Add(item.f2);
                        continue;
                    }
                    DifFiles1.Add(item.f1);
                    DifFiles2.Add(item.f2);
                }
                //当前已校对文件数(包含文件夹)
                NumDif += collection1.Count + collection2.Count;

                foreach (var item in children)
                {
                    Compare(item.f1.Child, item.f2.Child);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.ToString());
                Log(string.Format(@"
// 异常信息
// 校对时出现异常
// {0}
", ex.Message));
            }
            return true;

        }

        #endregion

        #region 日志，计时
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="log"></param>
        private void Log(string log)
        {
            lock (SysObject)
            {
                Logs += log + "\r\n";
            }
        }

        /// <summary>
        /// 计时
        /// </summary>
        private void Count(object obj, EventArgs e)
        {
            Time++;
        }
        #endregion
        #endregion

        #region 界面UI接口
        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="parameter"></param>
        private void MinWindowExecute(object parameter)
        {
            if ((m_View as Window) != null)
                (m_View as Window).WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 最大化窗口
        /// </summary>
        /// <param name="parameter"></param>
        private void MaxWindowExecute(object parameter)
        {
            Window win = (m_View as Window);
            if (win != null)
            {
                win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }
        /// <summary>
        ///关闭窗口 
        /// </summary>
        /// <param name="parameter"></param>
        private void CloseExecute(object parameter)
        {
            SaveIgnores();
            ClearTemp();
            m_View.DataContext = null;
            (m_View as Window).Hide();
        }

        /// <summary>
        /// 后台清空临时文件夹
        /// </summary>
        private void ClearTemp()
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((s, a) =>
            {
                //删除临时目录文件
                try
                {
                    DirectoryInfo dir1 = new DirectoryInfo(Environment.CurrentDirectory + @"/Temp1");
                    DirectoryInfo dir2 = new DirectoryInfo(Environment.CurrentDirectory + @"/Temp2");
                    if (Directory.Exists("Temp1"))
                    {
                        dir1.Delete(true);
                    }
                    if (Directory.Exists("Temp2"))
                    {
                        dir2.Delete(true);
                    }
                }
                catch (Exception)
                {
                }
            }));
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;
            bg.RunWorkerAsync();
        }

        private void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_View.Close();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveIgnores()
        {
            if (!File.Exists("Compare.cfg"))
            {
                File.Create("Compare.cfg");
            }

            string config = string.Empty;
            config = Config ?? string.Empty;
            //foreach (var key in Ignores)
            //{
            //    config += key + "|";
            //}
            //config = config.TrimEnd('|');
            CommonMethod.Write("立思辰文件比对工具2.0--by ybx", "过滤关键词", config, Environment.CurrentDirectory + @"\Compare.cfg");
            CommonMethod.Write("立思辰文件比对工具2.0--by ybx", "目标1", FilePath1, Environment.CurrentDirectory + @"\Compare.cfg");
            CommonMethod.Write("立思辰文件比对工具2.0--by ybx", "目标2", FilePath2, Environment.CurrentDirectory + @"\Compare.cfg");
        }

        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="obj"></param>
        private void DragMoveExecute(object obj)
        {
            Window win = (m_View as Window);
            if (win != null)
            {
                win.DragMove();
            }
        }
        #endregion
    }
}
