using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FilesCompare.Common;
using FilesCompare.Model;

namespace FilesCompare.CompareHelper
{
    public class DiffHelper
    {
        /// <summary>
        /// 比对文件代码，.class会反编译
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        public static void Diff(FNode f1, FNode f2)
        {
            string cmd = string.Empty;//命令
            string diskName = System.Windows.Forms.Application.StartupPath[0] + ":";//盘符名
            string curRunPath = System.Windows.Forms.Application.StartupPath + @"\Diffuse";//当前程序路径
            string newf1FullName = string.IsNullOrEmpty(f1.FName) ? f1.FFullName : f1.FFullName.Replace(f1.FName, "");//java文件1全路径
            string newf2FullName = string.IsNullOrEmpty(f2.FName) ? f2.FFullName : f2.FFullName.Replace(f2.FName, "");//java文件2全路径

            //反编译.class文件为.java文件
            if (f1.FFullName.EndsWith(".class") || f2.FFullName.EndsWith(".class"))
            {
                cmd += diskName;
                cmd += "&cd " + curRunPath;
                if (!string.IsNullOrEmpty(newf1FullName))
                    cmd += "&" + @"jad -o -d " + newf1FullName + " -s java " + f1.FFullName;
                if (!string.IsNullOrEmpty(newf2FullName))
                    cmd += "&" + @"jad -o -d " + newf2FullName + " -s java " + f2.FFullName;

                string res;
                CommonMethod.RunCmd(cmd, out res);
            }

            //打开对比工具
            using (Process p = new Process())
            {
                string name1 = f1.FFullName.Replace(".class", ".java");
                string name2 = f2.FFullName.Replace(".class", ".java");

                cmd = string.Format(@"echo 代码分析中... &{2}\diffuse {0} {1} &echo 代码分析完毕。", name1, name2, System.Windows.Forms.Application.StartupPath + "\\Diffuse");
                p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                p.StartInfo.Arguments = @"/c " + cmd;
                p.StartInfo.CreateNoWindow = true;
                p.Start();//启动程序
                Console.WriteLine("代码分析中...");
            }

        }
    }
}
