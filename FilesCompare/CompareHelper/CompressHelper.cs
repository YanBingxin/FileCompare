using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilesCompare.CompareHelper
{
    /// <summary>
    /// ID:解压缩jar，zip
    /// Describe:解压缩jar包，zip文件
    /// Author:ybx
    /// Date:2016-6-29 11:48:41
    /// </summary>
    public class CompressHelper
    {
        /// <summary>
        /// 解压缩 zip 文件
        /// </summary>
        /// <param name="zipFileName">要解压的 zip 文件</param>
        /// <param name="extractLocation">zip 文件的解压目录</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        public static void UnZipDir(string zipFileName, string location, bool overWrite)
        {
            string exLog = string.Empty;//异常日志
            if (!location.EndsWith(@"/"))
                location = location + @"/";

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFileName)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)//判断下一个zip 接口是否未空
                {
                    string directoryName = string.Empty;//下级目录名
                    string fileName = Path.GetFileName(theEntry.Name);//压缩文件名

                    if (!string.IsNullOrEmpty(theEntry.Name))
                        directoryName = Path.GetDirectoryName(theEntry.Name) + @"/";

                    //创建目录
                    Directory.CreateDirectory(location + directoryName);
                    //创建文件
                    try
                    {
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            if ((File.Exists(location + directoryName + fileName) && overWrite) || (!File.Exists(location + directoryName + fileName)))
                            {
                                FileStream streamWriter = File.Create(location + directoryName + fileName);
                                int size = 51200;
                                byte[] data = new byte[51200];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        exLog += ex.Message + "\r\n" + "在" + zipFileName + "压缩文件中\r\n" + directoryName + fileName + "\r\n";
                    }
                }
                s.Close();
            }
            if (string.IsNullOrEmpty(exLog))
                return;
            throw new Exception(exLog);
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="dirPath">压缩文件夹的路径</param>
        /// <param name="fileName">生成的zip文件路径</param>
        /// <param name="level">压缩级别 0 - 9 0是存储级别 9是最大压缩</param>
        /// <param name="bufferSize">读取文件的缓冲区大小</param>
        public static void CompressDirectory(string dirPath, string fileName, int level = 6, int bufferSize = 51200)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            byte[] buffer = new byte[bufferSize];
            using (ZipOutputStream s = new ZipOutputStream(File.Create(fileName)))
            {
                s.SetLevel(level);
                CompressDirectory(dirPath, dirPath, s, buffer);
                s.Finish();
                s.Close();
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="root">压缩文件夹路径</param>
        /// <param name="path">压缩文件夹内当前要压缩的文件夹路径</param>
        /// <param name="s"></param>
        /// <param name="buffer">读取文件的缓冲区大小</param>
        private static void CompressDirectory(string root, string path, ZipOutputStream s, byte[] buffer)
        {
            root = root.TrimEnd("\\".ToCharArray()) + "\\";
            string[] fileNames = Directory.GetFiles(path);
            string[] dirNames = Directory.GetDirectories(path);
            string relativePath = path.Replace(root, "");
            if (relativePath != "")
            {
                relativePath = relativePath.Replace("\\\\", "\\") + "\\";
            }
            int sourceBytes;
            foreach (string file in fileNames)
            {
                try
                {
                    ZipEntry entry = new ZipEntry(relativePath + Path.GetFileName(file));
                    entry.DateTime = DateTime.Now;
                    s.PutNextEntry(entry);
                    using (FileStream fs = File.OpenRead(file))
                    {
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                catch (Exception ex)
                {
                    string exs = ex.Message;
                }
            }

            foreach (string dirName in dirNames)
            {
                string relativeDirPath = dirName.Replace(root, "");
                ZipEntry entry = new ZipEntry(relativeDirPath.Replace("\\\\", "\\") + "\\");
                s.PutNextEntry(entry);
                CompressDirectory(root, dirName, s, buffer);
            }
        }
    }
}

