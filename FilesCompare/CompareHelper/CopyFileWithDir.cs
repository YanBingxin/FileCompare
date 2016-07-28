using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace FilesCompare.CompareHelper
{
    public class CopyFileWithDir
    {
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="strFullName"></param>
        /// <param name="strToPath"></param>
        public static void CopyFile(string strFullName, string directory, string strToPath, bool isZip = false)
        {
            string exLog = string.Empty;//异常日志
            string fileName = string.Empty;
            string folderName = string.Empty;
            try
            {
                if (!directory.EndsWith("\\"))
                {
                    directory += "\\";
                }
                if (!strToPath.EndsWith("\\"))
                {
                    strToPath += "\\";
                }

                fileName = Path.GetFileName(strFullName);

                //取得要拷贝的文件夹名
                if (isZip)
                {
                    folderName = directory;
                    folderName += strFullName.Substring(strFullName.IndexOf(folderName) + folderName.Length);
                    if (folderName.Contains(fileName))
                        folderName = folderName.Substring(0, folderName.Length - fileName.Length);
                }
                else
                {
                    folderName = strFullName.Substring(strFullName.IndexOf(directory) + directory.Length);
                    if (folderName.Contains(fileName))
                        folderName = folderName.Substring(0, folderName.Length - fileName.Length);
                }

                //如果源文件夹不存在，则创建
                if (!Directory.Exists(strToPath + folderName))
                {
                    Directory.CreateDirectory(strToPath + folderName);
                }
                //开始拷贝文件,true表示覆盖同名文件
                File.Copy(strFullName, strToPath + folderName + fileName, true);
                Console.WriteLine("原:" + strFullName);
                Console.WriteLine("到:" + strToPath + folderName + fileName + "\r\n");
            }
            catch (Exception ex)
            {
                if (MessageBox.Show("原:\r\n" + strFullName + "\r\n导出到:\r\n" + strToPath + folderName + fileName + "时,\r\n" + "发生异常:\r\n" + ex.Message + "," + "\r\n是否继续导出？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    throw ex;
            }
        }
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="strFromPath"></param>
        /// <param name="strToPath"></param>
        public static void CopyFolder(string strFullName, string directory, string strToPath, bool isZip = false)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";
            if (!strToPath.EndsWith("\\"))
                strToPath += "\\";
            //取得要拷贝的目录名
            string dirName = Path.GetFileName(strFullName);
            string folderName = string.Empty;
            try
            {
                //取得要拷贝的父层文件夹名
                if (isZip)
                {
                    folderName = directory;
                    folderName += strFullName.Substring(strFullName.IndexOf(folderName) + folderName.Length);
                    if (folderName.Contains(dirName))
                        folderName = folderName.Substring(0, folderName.Length - dirName.Length);
                }
                else
                {
                    folderName = strFullName.Substring(strFullName.IndexOf(directory) + directory.Length);
                    if (folderName.Contains(dirName))
                        folderName = folderName.Substring(0, folderName.Length - dirName.Length);
                }

                //如果文件夹不存在，则创建
                if (!Directory.Exists(strToPath + folderName + dirName))
                {
                    Directory.CreateDirectory(strToPath + folderName + dirName);
                }

                //开始拷贝文件,true表示覆盖同名文件
                foreach (string file in Directory.GetFiles(strFullName))
                {
                    CopyFile(file, directory, strToPath, isZip);
                }
                foreach (string folder in Directory.GetDirectories(strFullName))
                {
                    CopyFolder(folder, directory, strToPath, isZip);
                }
            }
            catch (Exception ex)
            {
                if (MessageBox.Show("导出\r\n" + strToPath + folderName + dirName + "时,\r\n发生异常：\r\n" + ex.Message + "," + "\r\n是否继续导出？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    throw ex;
            }
        }

        /// <summary>
        /// 创造空文件夹保证目录结构一直（修正多出缺少项比对结果导出）
        /// </summary>
        /// <param name="strFromPath"></param>
        /// <param name="strToPath"></param>
        public static void CopyEmptyFolder(string strFullName, string directory, string strToPath, bool isZip = false)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";
            if (!strToPath.EndsWith("\\"))
                strToPath += "\\";
            //取得要拷贝的目录名
            string dirName = Path.GetFileName(strFullName);
            //取得要拷贝的父层文件夹名
            string folderName = string.Empty;
            try
            {
                if (isZip)
                {
                    folderName = directory;
                    folderName += strFullName.Substring(strFullName.IndexOf(folderName) + folderName.Length);
                    if (folderName.Contains(dirName))
                        folderName = folderName.Substring(0, folderName.Length - dirName.Length);
                }
                else
                {
                    folderName = strFullName.Substring(strFullName.IndexOf(directory) + directory.Length);
                    if (folderName.Contains(dirName))
                        folderName = folderName.Substring(0, folderName.Length - dirName.Length);
                }


                //如果文件夹不存在，则创建
                if (!Directory.Exists(strToPath + folderName + dirName))
                {
                    Directory.CreateDirectory(strToPath + folderName + dirName);
                }

            }
            catch (Exception ex)
            {
                if (MessageBox.Show("导出\r\n" + strToPath + folderName + dirName + "时,\r\n发生异常:\r\n" + ex.Message + "," + "\r\n是否继续导出？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    throw ex;
            }
        }
    }
}
