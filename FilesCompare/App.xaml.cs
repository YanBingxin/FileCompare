﻿using FilesCompare.IView;
using FilesCompare.IViewModel;
using FilesCompare.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.IO;
using FilesCompare.CompareHelper;
using System.Security.Principal;

namespace FilesCompare
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //加载失败程序集后从资源获取ICSharpCode.SharpZipLib.dll
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args.Name.Contains("ICSharpCode.SharpZipLib"))
                    return Assembly.Load(FilesCompare.Properties.Resources.ICSharpCode_SharpZipLib);

                throw new Exception("not found assembly");
            };

            base.OnStartup(e);

            IMainWindowView view = new MainWindow();
            IMainWindowViewModel vm = new MainWindowViewModel(view);

            view.Height = SystemParameters.PrimaryScreenHeight * 0.75;
            view.Width = SystemParameters.PrimaryScreenWidth * 0.75;

            view.Show();
            if (e != null && e.Args != null && e.Args.Count() > 0)
                (vm as MainWindowViewModel).ImportExecute(e.Args[0]);
        }
    }
}
