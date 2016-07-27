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
using System.Windows.Forms;
using FilesCompare.View;

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
        /// <summary>
        /// 导出命令
        /// </summary>
        public ICommand ExportCommand { set; get; }
        /// <summary>
        /// 导入命令
        /// </summary>
        public ICommand ImportCommand { set; get; }
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
        /// ZIPdll库文件路径
        /// </summary>
        public string ZipHelperDll
        {
            get { return System.Environment.CurrentDirectory + "\\ICSharpCode.SharpZipLib.dll"; }
        }
        /// <summary>
        /// Diffdll库文件路径
        /// </summary>
        public string DiffHelperDll
        {
            get { return System.Environment.CurrentDirectory + "\\Diffuse.cpr"; }
        }
        /// <summary>
        /// 导入分析临时文件夹
        /// </summary>
        public string ImportPath
        {
            get
            {
                return System.Environment.CurrentDirectory + "\\Import\\" + TEMPNUMBER.ToString();
            }
        }
        /// <summary>
        /// 导出路径
        /// </summary>
        public string ExportPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + "\\" + "Export\\" + TEMPNUMBER.ToString() + "\\";
            }
        }
        /// <summary>
        /// 导出结果文件名
        /// </summary>
        public string ExportFileName
        {
            get
            {
                return "校对结果" + TEMPNUMBER.ToString() + ".cpr";
            }
        }
        #region 结果搜索

        /// <summary>
        /// 搜索应用于文件
        /// </summary>
        private bool _searchOnFiles;
        /// <summary>
        /// 获取或设置搜索应用于文件
        /// </summary>
        public bool SearchOnFiles
        {
            get
            {
                return _searchOnFiles;
            }
            set
            {
                if (_searchOnFiles == value)
                {
                    return;
                }
                _searchOnFiles = value;
                RaisePropertyChanged("SearchOnFiles");
                LoadResult(_searchPara);
            }
        }

        /// <summary>
        /// 搜索应用于文件夹
        /// </summary>
        private bool _searchOnFolders;
        /// <summary>
        /// 获取或设置搜索应用于文件夹
        /// </summary>
        public bool SearchOnFolders
        {
            get
            {
                return _searchOnFolders;
            }
            set
            {
                if (_searchOnFolders == value)
                {
                    return;
                }
                _searchOnFolders = value;
                RaisePropertyChanged("SearchOnFolders");
                LoadResult(_searchPara);
            }
        }

        /// <summary>
        /// 搜索根据压缩文件(夹)
        /// </summary>
        private bool _searchByUCFiles;
        /// <summary>
        /// 搜索根据压缩文件(夹)
        /// </summary>
        public bool SearchByUCFiles
        {
            get
            {
                return _searchByUCFiles;
            }
            set
            {
                if (_searchByUCFiles == value)
                {
                    return;
                }
                _searchByUCFiles = value;
                RaisePropertyChanged("SearchOnUCFiles");
                LoadResult(_searchPara);
            }
        }


        /// <summary>
        /// 搜索参数
        /// </summary>
        private string _searchPara;
        /// <summary>
        /// 获取或设置搜索参数
        /// </summary>
        public string SearchPara
        {
            get
            {
                return _searchPara;
            }
            set
            {
                if (_searchPara == value)
                {
                    return;
                }
                _searchPara = value;
                RaisePropertyChanged("SearchPara");
                LoadResult(_searchPara);
            }
        }

        #endregion

        #region 黑名单
        /// <summary>
        /// 黑名单是否应用于普通(非解压后)文件
        /// </summary>
        private bool _ignoreOnFiles = true;
        /// <summary>
        /// 获取或设置黑名单是否应用于普通(非解压后)文件
        /// </summary>
        public bool IgnoreOnFiles
        {
            get
            {
                return _ignoreOnFiles;
            }
            set
            {
                if (_ignoreOnFiles == value)
                {
                    return;
                }
                _ignoreOnFiles = value;
                RaisePropertyChanged(() => IgnoreOnFiles);
            }
        }

        /// <summary>
        /// 黑名单是否应用于普通(非解压后)文件夹
        /// </summary>
        private bool _ignoreOnFolders = false;
        /// <summary>
        /// 获取或设置黑名单是否应用于普通(非解压后)文件夹
        /// </summary>
        public bool IgnoreOnFolders
        {
            get
            {
                return _ignoreOnFolders;
            }
            set
            {
                if (_ignoreOnFolders == value)
                {
                    return;
                }
                _ignoreOnFolders = value;
                RaisePropertyChanged(() => IgnoreOnFolders);
            }
        }

        /// <summary>
        /// 黑名单是否应用于解压后文件
        /// </summary>
        private bool _ignoreOnUCFiles = false;
        /// <summary>
        /// 获取或设置黑名单是否应用于解压后文件
        /// </summary>
        public bool IgnoreOnUCFiles
        {
            get
            {
                return _ignoreOnUCFiles;
            }
            set
            {
                if (_ignoreOnUCFiles == value)
                {
                    return;
                }
                _ignoreOnUCFiles = value;
                RaisePropertyChanged(() => IgnoreOnUCFiles);
            }
        }

        /// <summary>
        /// 黑名单是否应用于解压后文件夹
        /// </summary>
        private bool _ignoreOnUCFolders = false;
        /// <summary>
        /// 获取或设置黑名单是否应用于解压后文件夹
        /// </summary>
        public bool IgnoreOnUCFolders
        {
            get
            {
                return _ignoreOnUCFolders;
            }
            set
            {
                if (_ignoreOnUCFolders == value)
                {
                    return;
                }
                _ignoreOnUCFolders = value;
                RaisePropertyChanged(() => IgnoreOnUCFolders);
            }
        }

        /// <summary>
        /// 黑名单字符串
        /// </summary>
        private string _ignore = string.Empty;
        /// <summary>
        /// 获取或设置黑名单字符串
        /// </summary>
        public string Ignore
        {
            get
            {
                return _ignore;
            }
            set
            {
                _ignore = value;
                RaisePropertyChanged(() => Ignore);
            }
        }

        private ObservableCollection<StringWraper> _ignores;
        /// <summary>
        /// 黑名单关键字
        /// </summary>
        public ObservableCollection<StringWraper> Ignores
        {
            get
            {
                return _ignores ?? (_ignores = new ObservableCollection<StringWraper>());
            }
            set
            {
                _ignores = value;
            }
        }
        #endregion

        #region 白名单
        /// <summary>
        /// 白明单是否应用文件
        /// </summary>
        private bool _preferOnFiles = true;
        /// <summary>
        /// 获取或设置白明单是否应用于文件
        /// </summary>
        public bool PreferOnFiles
        {
            get
            {
                return _preferOnFiles;
            }
            set
            {
                if (_preferOnFiles == value)
                {
                    return;
                }
                _preferOnFiles = value;
                RaisePropertyChanged(() => PreferOnFiles);
            }
        }

        /// <summary>
        /// 白明单是否应用于解压后文件
        /// </summary>
        private bool _preferOnUCFiles = false;
        /// <summary>
        /// 获取或设置白明单是否应用于解压后文件
        /// </summary>
        public bool PreferOnUCFiles
        {
            get
            {
                return _preferOnUCFiles;
            }
            set
            {
                if (_preferOnUCFiles == value)
                {
                    return;
                }
                _preferOnUCFiles = value;
                RaisePropertyChanged(() => PreferOnUCFiles);
            }
        }

        /// <summary>
        /// 白名单配置
        /// </summary>
        private string _prefer = string.Empty;
        /// <summary>
        /// 获取或设置白名单配置
        /// </summary>
        public string Prefer
        {
            get
            {
                return _prefer;
            }
            set
            {
                _prefer = value;
                RaisePropertyChanged("Prefer");
            }
        }

        private ObservableCollection<StringWraper> _prefers;
        /// <summary>
        /// 白名单关键字
        /// </summary>
        public ObservableCollection<StringWraper> Prefers
        {
            get
            {
                return _prefers ?? (_prefers = new ObservableCollection<StringWraper>());
            }
            set
            {
                _prefers = value;
            }
        }
        #endregion

        #region 统计数量
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
        #endregion

        #region 界面显示状态属性
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
        #endregion

        #region 缓存
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
            LoadConfig();
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
            ExportCommand = new RelayCommand(ExportExecute);
            ImportCommand = new RelayCommand(ImportExecute);
        }

        /// <summary>
        /// 初始数据
        /// </summary>
        private void InitData()
        {
            btnContent = string.Format("开始分析");
            using (FileStream fs = new FileStream(ZipHelperDll, FileMode.Create))
            {
                fs.Write(Properties.Resources.ICSharpCode_SharpZipLib, 0, Properties.Resources.ICSharpCode_SharpZipLib.Length);
                fs.Flush();
                fs.Close();
            }
            using (FileStream fs = new FileStream(DiffHelperDll, FileMode.Create))
            {
                fs.Write(Properties.Resources.Diffuse, 0, Properties.Resources.Diffuse.Length);
                fs.Flush();
                fs.Close();
            }

            try
            {
                CompressHelper.UnZipDir(DiffHelperDll, Environment.CurrentDirectory, false);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region 方法处理
        #region 校对文件处理
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
            CompareFiles(prarmeter);
        }
        /// <summary>
        /// 初始化比较所用数据
        /// </summary>
        private void InitDataForCompare()
        {
            Log("日期:" + DateTime.Now.ToExtString("yyyy-MM-dd HH:mm:ss"));
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
        /// 检索文件目录计算MD5
        /// </summary>
        private void CompareFiles(object pararmeter)
        {
            Log("初步分析准备中...");
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((sender, e) =>
            {
                Parallel.Invoke(new Action(() =>
                {
                    Log("目标1文件系统加载中...");
                    NormalFiles1 = pararmeter == null ? LoadFiles(FilePath1) : LoadFiles(FilePath1, "", "");
                    Log("目标1文件系统加载完毕。");
                }), new Action(() =>
                {
                    Log("目标2文件系统加载中...");
                    NormalFiles2 = pararmeter == null ? LoadFiles(FilePath2) : LoadFiles(FilePath2, "", "");
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
            //LoadResult(_searchPara);
            Log("压缩文件(.jar.zip)分析系统启动中...");
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((s, a) =>
            {
                Parallel.Invoke(new Action(() =>
                {
                    if (ZipFiles1.Count == 0)
                        return;
                    Log("目标1压缩文件解压中...");
                    UnZipFiles(ZipFiles1, "Temp1");
                    Log("目标1压缩文件解压完毕。");
                    UnCompressFiles1 = LoadFiles(Environment.CurrentDirectory + "\\Temp1" + "\\" + TEMPNUMBER.ToString(), string.Empty, string.Empty);
                    Log("目标1解压文件加载完毕。");
                }), new Action(() =>
                {
                    if (ZipFiles2.Count == 0)
                        return;
                    Log("目标2压缩文件解压中...");
                    UnZipFiles(ZipFiles2, "Temp2");
                    Log("目标2压缩文件解压完毕。");
                    UnCompressFiles2 = LoadFiles(Environment.CurrentDirectory + "\\Temp2" + "\\" + TEMPNUMBER.ToString(), string.Empty, string.Empty);
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
            Log("文件分析系统校对完毕。");
            //由于白名单无法对文件夹过滤，导致结果出现部分文件夹级结果，若白名单应用，则过滤掉结果中的文件夹
            CheckResFolders(PreferOnFiles, PreferOnUCFiles);
            //缓冲加载结果，防卡死
            LoadResult(SearchPara, true);
#if Release
            NumDif = NumMD5;//无意义，为了使使用者安心。由于分析包含目录结构，实际校对数<=总数。(例如多出文件夹，将不比较子目录文件)
#endif
            timer.Stop();
            Log("计时器已终止。");
            Log("结果加载中...");
            WinWait win = new WinWait();
            win.Message = "已校对完毕";
            win.ShowDialogEx();
        }
        /// <summary>
        /// 过滤结果中不适用白名单的文件结果
        /// </summary>
        /// <param name="PreferOnFiles"></param>
        /// <param name="PreferOnUCFiles"></param>
        private void CheckResFolders(bool PreferOnFiles, bool PreferOnUCFiles)
        {
            if (PreferOnFiles && !string.IsNullOrEmpty(Prefer))//过滤普通文件结果
            {
                for (int i = 0; i < DifFiles1.Count; i++)
                {
                    if (DifFiles1[i].IsFile == false || DifFiles2[i].IsFile == false && (DifFiles1[i].JarParentName == string.Empty && DifFiles2[i].JarParentName == string.Empty))
                    {
                        DifFiles1[i] = DifFiles2[i] = new FNode() { IsFile = null };
                    }
                }
            }
            if (PreferOnUCFiles && !string.IsNullOrEmpty(Prefer))
            {
                for (int i = 0; i < DifFiles1.Count; i++)
                {
                    if (DifFiles1[i].IsFile == false || DifFiles2[i].IsFile == false && (DifFiles1[i].JarParentName != string.Empty && DifFiles2[i].JarParentName != string.Empty))
                    {
                        DifFiles1[i] = DifFiles2[i] = new FNode() { IsFile = null };
                    }
                }
            }
        }

        #endregion

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
                    Log("解压：" + file.FFullName);
                    UnzipFileName = file.FFullName;
                    string folderName = file.FFullName.Replace(rootName, location + "/" + TEMPNUMBER.ToString());
                    CompressHelper.UnZipDir(file.FFullName, folderName, false);
                }
                catch (Exception e)
                {
                    Log(string.Format(@"
// 异常信息
// 解压时出现异常
// 文件名:{0}
// 详细信息:
// {1}
                    ", file.FFullName, e.Message));
                }
            }
        }
        #endregion

        #region 过滤文件
        /// <summary>
        /// 检查是否是要过滤的文件
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        private bool IsIgnore(string fname)
        {
            foreach (var key in Ignores)
            {
                if (!string.IsNullOrEmpty(key.Value) && fname.Contains(key.Value))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 检查是否是要比对的文件
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        private bool IsPrefer(string fname)
        {
            if (Prefers.Count == 0 || string.IsNullOrEmpty(Prefer))
            {
                return true;
            }
            foreach (var key in Prefers)
            {
                if (!string.IsNullOrEmpty(key.Value) && fname.Contains(key.Value))
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
                                                            where (IgnoreOnFiles == false || IsIgnore(f) == false) && (PreferOnFiles == false || IsPrefer(f))
                                                            select new FNode()
                                                            {
                                                                FFullName = f,
                                                                FName = f.Replace(pathName + "\\", ""),
                                                                IsFile = true
                                                            }).Concat(from d in Directory.GetDirectories(pathName)
                                                                      where IgnoreOnFolders == false || IsIgnore(d) == false
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
        /// 检索jar包下所有字文件(夹)附加jar包标示
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        private ObservableCollection<FNode> LoadFiles(string pathName, string parentPath, string jarName)
        {
            if (string.IsNullOrEmpty(jarName))
            {
                jarName = GetJarName(parentPath);
            }

            ObservableCollection<FNode> tempList = new ObservableCollection<FNode>();
            //检索当前目录所有文件和目录节点
            try
            {
                tempList = new ObservableCollection<FNode>((from f in Directory.GetFiles(pathName)
                                                            where (IgnoreOnUCFiles == false || IsIgnore(f) == false) && (PreferOnUCFiles == false || IsPrefer(f))//对解压后文件根据配置应用黑名单和白名单校验
                                                            select new FNode()
                                                            {
                                                                FFullName = f,
                                                                FName = f.Replace(pathName + "\\", ""),
                                                                JarParentName = jarName,
                                                                IsFile = true
                                                            }).Concat(from d in Directory.GetDirectories(pathName)
                                                                      where IgnoreOnUCFolders == false || IsIgnore(d) == false
                                                                      select new FNode()
                                                                      {
                                                                          FFullName = d,
                                                                          FName = d.Replace(pathName + "\\", ""),
                                                                          Child = LoadFiles(d, d, jarName),
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
        /// <summary>
        /// 取来自的压缩文件名称
        /// </summary>
        /// <param name="parentPath"></param>
        /// <returns></returns>
        private string GetJarName(string parentPath)
        {
            if (string.IsNullOrEmpty(parentPath))
                return string.Empty;
            string p1 = Environment.CurrentDirectory + "\\" + "Temp1" + "\\" + TEMPNUMBER.ToString();
            string p2 = Environment.CurrentDirectory + "\\" + "Temp2" + "\\" + TEMPNUMBER.ToString();

            if (parentPath.Contains(p1))
            {
                parentPath = parentPath.Replace(p1, FilePath1);
            }
            if (parentPath.Contains(p2))
            {
                parentPath = parentPath.Replace(p2, FilePath2);
            }

            if (parentPath.Contains(".jar"))
            {
                return parentPath.Substring(0, parentPath.IndexOf(".jar") + ".jar".Length);
            }
            if (parentPath.Contains(".zip"))
            {
                return parentPath.Substring(0, parentPath.IndexOf(".zip") + ".zip".Length);
            }
            return string.Empty;
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
                    if (item.f1.FFullName.EndsWith(".zip") || item.f1.FFullName.EndsWith(".jar"))
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
                Log(string.Format(@"
// 异常信息
// 校对时出现异常
// {0}
", ex.Message));
            }
            return true;

        }

        #endregion

        #region 搜索结果
        /// <summary>
        /// 后台异步加载结果
        /// </summary>
        /// <param name="searchPara">搜索符号</param>
        /// <param name="isLog">是否记录日志</param>
        public void LoadResult(string searchPara, bool isLog = false)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((s, e) =>
            {
                //默认搜索字符串为空，全部加载
                if (isLog)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Result1.Clear();
                        Result2.Clear();
                    }));

                    for (int i = 0; i < DifFiles1.Count; i++)
                    {
                        if (DifFiles1[i].FName == string.Empty && DifFiles2[i].FName == string.Empty)
                        {
                            continue;
                        }
                        if (i > 0 && i % 500 == 0)
                        {
                            Thread.Sleep(100);
                        }
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Result1.Add(DifFiles1[i]);
                            Result2.Add(DifFiles2[i]);
                        }));
                    }
                }
                else//有搜索字符时执行搜索过滤
                {
                    lock (SysObject)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Result1.Clear();
                            Result2.Clear();
                        }));

                        for (int i = 0; i < DifFiles1.Count; i++)
                        {
                            if (DifFiles1[i].FName == string.Empty && DifFiles2[i].FName == string.Empty)
                            {
                                continue;
                            }
                            if (string.IsNullOrEmpty(searchPara)
                                || (SearchOnFiles == true && (DifFiles1[i].FName.Contains(searchPara) || DifFiles2[i].FName.Contains(searchPara)))
                                || (SearchOnFolders == true && (DifFiles1[i].FFullName.Contains(searchPara) || DifFiles2[i].FFullName.Contains(searchPara)))
                                || (SearchByUCFiles == true && (DifFiles1[i].JarParentName.Contains(searchPara) || DifFiles2[i].JarParentName.Contains(searchPara))))
                            {
                                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    Result1.Add(DifFiles1[i]);
                                    Result2.Add(DifFiles2[i]);
                                }));
                            }
                        }
                    }
                }
                //是否记录日志
                if (isLog)
                {
                    int more = DifFiles1.Where(f => f.DifTag == false).Count();
                    int less = DifFiles2.Where(f => f.DifTag == false).Count();
                    int changed = DifFiles1.Where(f => f.DifTag == true).Count();
                    Log(string.Format(@"合计  多出:{0},缺少:{1},差异:{2}", more, less, changed));
                    Log("结果加载完毕。");
                    Log(string.Empty);
                }
            }));
            bg.RunWorkerCompleted += Completed;
            bg.RunWorkerAsync();
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
        }
        #endregion

        #region 日志，计时
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="log"></param>
        private void Log(string log)
        {
            //lock (SysObject)
            //{
            Logs += log + "\r\n";
            // }
        }

        /// <summary>
        /// 计时
        /// </summary>
        private void Count(object obj, EventArgs e)
        {
            Time++;
        }
        #endregion

        #region 加载，保存配置文件
        /// <summary>
        /// 加载过滤关键字
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                //管理.cpr文件类型
                //FileTypeRelative.Relative(".cpr");

                string tempLever = GetValue("临时文件级数");
                string ignoreOnFiles = GetValue("黑名单-包含文件");
                string ignoreOnFolders = GetValue("黑名单-包含文件夹");
                string ignoreOnUCFiles = GetValue("黑名单-包含解压缩后文件");
                string ignoreOnUCFolders = GetValue("黑名单-包含解压缩后文件夹");
                string preferOnFiles = GetValue("白名单-包含文件");
                string preferOnUCFiles = GetValue("白名单-包含解压缩后文件");
                string searchOnFiles = GetValue("搜索参数-根据文件名搜索");
                string searchOnFolders = GetValue("搜索参数-根据文件夹名搜索");
                string searchByUCFiles = GetValue("搜索参数-根据压缩文件名搜索");

                Ignore = GetValue("黑名单");
                Prefer = GetValue("白名单");
                FilePath1 = GetValue("目标1");
                FilePath2 = GetValue("目标2");
                SearchPara = GetValue("搜索参数-搜索符");
                Logs = GetValue("日志").Replace("#^#", "\r\n");
                ConfigToDataGrid();

                IgnoreOnFiles = string.IsNullOrEmpty(ignoreOnFiles) ? true : bool.Parse(ignoreOnFiles);
                PreferOnFiles = string.IsNullOrEmpty(preferOnFiles) ? true : bool.Parse(preferOnFiles);
                SearchOnFiles = string.IsNullOrEmpty(searchOnFiles) ? true : bool.Parse(searchOnFiles);
                SearchByUCFiles = string.IsNullOrEmpty(searchByUCFiles) ? true : bool.Parse(searchByUCFiles);

                TEMPNUMBER = Convert.ToInt32(tempLever);
                IgnoreOnFolders = bool.Parse(ignoreOnFolders);
                IgnoreOnUCFiles = bool.Parse(ignoreOnUCFiles);
                IgnoreOnUCFolders = bool.Parse(ignoreOnUCFolders);
                PreferOnUCFiles = bool.Parse(preferOnUCFiles);
                SearchOnFolders = bool.Parse(searchOnFolders);
            }
            catch (Exception)
            {
            }
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

            SetValue("临时文件级数", TEMPNUMBER.ToString());
            SetValue("黑名单", Ignore);
            SetValue("白名单", Prefer);
            SetValue("黑名单-包含文件", IgnoreOnFiles.ToString());
            SetValue("黑名单-包含文件夹", IgnoreOnFolders.ToString());
            SetValue("黑名单-包含解压缩后文件", IgnoreOnUCFiles.ToString());
            SetValue("黑名单-包含解压缩后文件夹", IgnoreOnUCFolders.ToString());
            SetValue("白名单-包含文件", PreferOnFiles.ToString());
            SetValue("白名单-包含解压缩后文件", PreferOnUCFiles.ToString());
            SetValue("目标1", FilePath1);
            SetValue("目标2", FilePath2);
            SetValue("搜索参数-搜索符", SearchPara);
            SetValue("搜索参数-根据文件名搜索", SearchOnFiles.ToString());
            SetValue("搜索参数-根据文件夹名搜索", SearchOnFolders.ToString());
            SetValue("搜索参数-根据压缩文件名搜索", SearchByUCFiles.ToString());
            SetValue("日志", Logs.Replace("\r\n", "#^#"));
        }

        /// <summary>
        /// 通过读取配置设置黑白名单
        /// </summary>
        private void ConfigToDataGrid()
        {
            Ignores.Clear();
            Prefers.Clear();
            foreach (var key in Ignore.Split('|'))
            {
                if (string.IsNullOrEmpty(key))
                    continue;
                Ignores.Add(new StringWraper() { Value = key });
            }

            foreach (var key in Prefer.Split('|'))
            {
                if (string.IsNullOrEmpty(key))
                    continue;
                Prefers.Add(new StringWraper() { Value = key });
            }
            //当黑白名单过少时，未显示美观，自动填充到9个
            if (Ignores.Count < 9)
            {
                for (int i = Ignores.Count; i < 9; i++)
                {
                    Ignores.Add(new StringWraper() { Value = "" });
                }
            }
            if (Prefers.Count < 9)
            {
                for (int i = Prefers.Count; i < 9; i++)
                {
                    Prefers.Add(new StringWraper() { Value = "" });
                }
            }
        }

        /// <summary>
        /// 通过设置datagrid更新config
        /// </summary>
        public void DataGridToConfig()
        {
            Ignore = string.Empty;
            Prefer = string.Empty;

            foreach (var item in Ignores)
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                Ignore += item.Value + "|";
            }
            foreach (var item in Prefers)
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                Prefer += item.Value + "|";
            }

            Ignore = Ignore.TrimEnd('|');
            Prefer = Prefer.TrimEnd('|');
        }

        /// <summary>
        /// 根据配置键获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetValue(string key)
        {
            return CommonMethod.Read("立思辰文件比对工具3.0--by ybx", key, Environment.CurrentDirectory + @"\Compare.cfg");
        }

        /// <summary>
        /// 根据配置键设置值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private void SetValue(string key, string value)
        {
            CommonMethod.Write("立思辰文件比对工具3.0--by ybx", key, value, Environment.CurrentDirectory + @"\Compare.cfg");
        }
        #endregion

        #region 导入导出
        /// <summary>
        /// 导出分析结果
        /// </summary>
        /// <param name="obj"></param>
        private void ExportExecute(object obj)
        {
            //保存地址
            string resultPath = GetResPath();
            if (string.IsNullOrEmpty(resultPath))
                return;
            Log("正在导出结果...");
            UnzipFileName = "导出结果中...";
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((s, a) =>
            {
                //复制到导出文件夹
                try
                {
                    Parallel.Invoke(delegate { CopyResults(Result1, FilePath1, "新"); }, delegate { CopyResults(Result2, FilePath2, "旧"); });
                }
                catch (System.AggregateException ex)
                {
                    string aa = "";
                }
                UnzipFileName = "结果文件合并中...";
                CompressHelper.CompressDirectory(ExportPath, resultPath);
            }));

            bg.RunWorkerCompleted += ExportCompeleted;
            bg.RunWorkerAsync();
        }

        /// <summary>
        /// 获取结果路径
        /// </summary>
        /// <returns></returns>
        private string GetResPath()
        {
            string res = string.Empty;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.AddExtension = true;
                dlg.DefaultExt = ".cpr";
                dlg.Filter = "校对文件 (*.cpr)|*.cpr";
                dlg.FileName = ExportFileName;
                dlg.Title = "请选择导出结果文件位置：";
                if (dlg.ShowDialog() == DialogResult.OK)
                    res = dlg.FileName;
            }
            return res;
        }

        /// <summary>
        /// 导出完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("结果导出完毕。");
            UnzipFileName = "导出结果完毕";
            WinWait win = new WinWait();
            win.Message = "结果导出完毕";
            win.ShowDialogEx();
        }

        /// <summary>
        /// 复制所有结果到目标文件夹
        /// </summary>
        /// <param name="collection">文件集合</param>
        /// <param name="rootPath">根目录</param>
        /// <param name="id">额外文件夹标识</param>
        private void CopyResults(ObservableCollection<FNode> collection, string rootPath, string id)
        {
            if (string.IsNullOrEmpty(rootPath))
                return;

            string originName = Path.GetFileName(rootPath);
            string targetPath = ExportPath + id + "\\" + originName;

            foreach (var fd in collection)
            {
                UnzipFileName = fd.FFullName.Replace(rootPath, "..");

                //若为空，为保证导入后校对结果，需将上级目录创建
                if (string.IsNullOrEmpty(fd.FFullName))
                {
                    int index = collection.IndexOf(fd);
                    CopyEmptyFolder(Result1[index], Result2[index], targetPath);
                    continue;
                }
                //普通文件
                if (string.IsNullOrEmpty(fd.JarParentName))
                {
                    if (fd.IsFile == true)
                        CopyFileWithDir.CopyFile(fd.FFullName, rootPath, targetPath);
                    if (fd.IsFile == false)
                        CopyFileWithDir.CopyFolder(fd.FFullName, rootPath, targetPath);
                }
                else//压缩解压后文件
                {
                    if (fd.IsFile == true)
                        CopyFileWithDir.CopyFile(fd.FFullName, fd.JarParentName.Replace(rootPath + "\\", ""), targetPath, true);
                    if (fd.IsFile == false)
                        CopyFileWithDir.CopyFolder(fd.FFullName, fd.JarParentName.Replace(rootPath + "\\", ""), targetPath, true);
                }
            }
        }
        /// <summary>
        /// 创建一个空的父目录保证上级目录相同
        /// </summary>
        /// <param name="fNode1"></param>
        /// <param name="fNode2"></param>
        /// <param name="targetPath"></param>
        private void CopyEmptyFolder(FNode fNode1, FNode fNode2, string targetPath)
        {
            FNode node = string.IsNullOrEmpty(fNode1.FFullName) ? fNode2 : fNode1;
            string path = node.FFullName;
            string root = Result1.Contains(node) ? FilePath1 : FilePath2;
            string parentDirPath = Directory.GetParent(path).FullName;
            bool isZip = !string.IsNullOrEmpty(node.JarParentName);
            if (isZip)
                root = node.JarParentName.Replace(root + "\\", "");
            CopyFileWithDir.CopyEmptyFolder(parentDirPath, root, targetPath, isZip);
        }
        /// <summary>
        /// 导入分析结果
        /// </summary>
        /// <param name="obj"></param>
        private void ImportExecute(object obj)
        {
            string importPath = GetImportPath();
            if (string.IsNullOrEmpty(importPath))
                return;
            try
            {
                BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((sender, e) =>
                {
                    CompressHelper.UnZipDir(importPath, ImportPath, false);
                    string file1 = Directory.GetDirectories(ImportPath + "\\新")[0];
                    string file2 = Directory.GetDirectories(ImportPath + "\\旧")[0];
                    FilePath1 = file1;
                    FilePath2 = file2;
                }));
                bg.RunWorkerCompleted += ImportCompeleted;
                bg.RunWorkerAsync();
                Log("正在分析校对信息...");
                UnzipFileName = "正在分析校对信息...";
            }
            catch (Exception)
            {
                (new WinWait("该结果文件不包含校对信息")).ShowDialogEx();
            }
        }
        /// <summary>
        /// 导入完毕开始比对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportCompeleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UnzipFileName = "导入完毕";
            CompareExecute("导入校对");
            Log("导入完毕");
        }
        /// <summary>
        /// 获取导入路径
        /// </summary>
        /// <returns></returns>
        private string GetImportPath()
        {
            string res = string.Empty;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.AddExtension = true;
                dlg.DefaultExt = ".cpr";
                dlg.Filter = "校对文件 (*.cpr)|*.cpr";
                dlg.Title = "请选择要导入文件位置：";
                if (dlg.ShowDialog() == DialogResult.OK)
                    res = dlg.FileName;
            }
            return res;
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
                    DirectoryInfo dir3 = new DirectoryInfo(Environment.CurrentDirectory + @"/Export");
                    DirectoryInfo dir4 = new DirectoryInfo(Environment.CurrentDirectory + @"/Import");
                    DirectoryInfo dir5 = new DirectoryInfo(Environment.CurrentDirectory + @"/Diffuse");

                    if (Directory.Exists("Temp1"))
                        dir1.Delete(true);
                    if (Directory.Exists("Temp2"))
                        dir2.Delete(true);
                    if (Directory.Exists("Export"))
                        dir3.Delete(true);
                    if (Directory.Exists("Import"))
                        dir4.Delete(true);
                    if (Directory.Exists("Diffuse"))
                        dir5.Delete(true);
                    if (File.Exists(DiffHelperDll))
                        File.Delete(DiffHelperDll);
                    if (File.Exists(ZipHelperDll))
                        File.Delete(ZipHelperDll);
                }
                catch (Exception ex)
                {
                    string mes = ex.Message;
                }
            }));
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;
            bg.RunWorkerAsync();
        }

        private void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommonMethod.Write("立思辰文件比对工具2.0--by ybx", "临时文件级数", "0", Environment.CurrentDirectory + @"\Compare.cfg");
            m_View.Close();
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
