using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilesCompare.CompareHelper
{
    public class CopyFileWithDir
    {
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="strFullName"></param>
        /// <param name="strToPath"></param>
        public static void CopyFile(string strFullName, string directory, string strToPath)
        {
            if (!directory.EndsWith("\\"))
            {
                directory += "\\";
            }
            if (!strToPath.EndsWith("\\"))
            {
                strToPath += "\\";
            }

            string fileName = Path.GetFileName(strFullName);
            //取得要拷贝的文件夹名
            string folderName = strFullName.Substring(strFullName.IndexOf(directory) + directory.Length);
            folderName = folderName.Substring(0, folderName.Length - fileName.Length);

            //如果源文件夹不存在，则创建
            if (!Directory.Exists(strToPath + folderName))
            {
                Directory.CreateDirectory(strToPath + folderName);
            }

            //开始拷贝文件,true表示覆盖同名文件
            File.Copy(strFullName, strToPath + folderName + fileName, true);
        }
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="strFromPath"></param>
        /// <param name="strToPath"></param>
        public static void CopyFolder(string strFullName, string directory, string strToPath)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";
            if (!strToPath.EndsWith("\\"))
                strToPath += "\\";
            //取得要拷贝的目录名
            string dirName = Path.GetFileName(strFullName);
            //取得要拷贝的父层文件夹名
            string folderName = strFullName.Substring(strFullName.IndexOf(directory) + directory.Length);
            folderName = folderName.Substring(0, folderName.Length - dirName.Length);

            //如果文件夹不存在，则创建
            if (!Directory.Exists(strToPath + folderName + dirName))
            {
                Directory.CreateDirectory(strToPath + folderName + dirName);
            }

            //开始拷贝文件,true表示覆盖同名文件
            foreach (string file in Directory.GetFiles(strFullName))
            {
                CopyFile(file, directory, strToPath);
            }
            foreach (string folder in Directory.GetDirectories(strFullName))
            {
                CopyFolder(folder, directory, strToPath);
            }
        }
    }
}
